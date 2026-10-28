"""Prepare twenty derived, bounded festival/complete-song charts without editing audio.

Normal generation uses the committed signal-analysis snapshot and Python stdlib.
Use --analyze to refresh that snapshot with FFmpeg and NumPy installed. Signal
analysis estimates phase and phrase energy; it is not a human listening test.
"""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path
import shutil
import struct
import subprocess
import uuid

from prepare_exhibition_track import mp3_duration

ROOT = Path(__file__).resolve().parents[2]
DATA = ROOT / "Assets/StreamingAssets/Rhythm/Festival20261029"
MIN_GAP = 0.72
LEAD = 2.8
CLEANUP = 5.0


def json_bytes(value):
    return (json.dumps(value, indent=2, ensure_ascii=False) + "\n").encode("utf-8")


def read_json(path):
    return json.loads(path.read_text(encoding="utf-8"))


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def ogg_playable_samples(data):
    """Use the final EOS granule, which can trim an earlier oversized granule.

    Mecha-Tribe's final page trims to 4,630,500 samples (105 s) even though an
    earlier page announces 4,692,672. Taking max() exposes non-playable padding.
    FFmpeg's decoded PCM count can also disagree for this unusual timestamp
    sequence, so the Vorbis EOS is the authority for the imported clip bound.
    """
    position, serial, rate, final = 0, None, None, None
    while position < len(data):
        if data[position:position + 4] != b"OggS" or position + 27 > len(data):
            raise ValueError("Invalid Ogg page")
        count = data[position + 26]
        header_end = position + 27 + count
        if header_end > len(data):
            raise ValueError("Truncated Ogg page header")
        page_end = header_end + sum(data[position + 27:header_end])
        if page_end > len(data):
            raise ValueError("Truncated Ogg page payload")
        page_serial = struct.unpack_from("<I", data, position + 14)[0]
        if serial is None:
            serial = page_serial
        if page_serial != serial:
            raise ValueError("Chained/multiple Ogg streams require an explicit policy")
        payload = data[header_end:page_end]
        if payload.startswith(b"\x01vorbis") and len(payload) >= 16:
            rate = struct.unpack_from("<I", payload, 12)[0]
        granule = struct.unpack_from("<q", data, position + 6)[0]
        if data[position + 5] & 4:
            if page_end != len(data) or granule <= 0:
                raise ValueError("Invalid or non-final Ogg EOS")
            final = granule
        position = page_end
    if rate is None or final is None:
        raise ValueError("Vorbis identification and final EOS are required")
    return final, rate


def playable_samples(path):
    data = path.read_bytes()
    return ogg_playable_samples(data) if path.suffix.lower() == ".ogg" else mp3_duration(data)


def tracks():
    """Existing source definitions remain the authority for timing and rights."""
    result = []
    folder = ROOT / "Assets/StreamingAssets/Rhythm/Prepared"
    names = ("GoinUnder_Beginner", "Springtime_Beginner", "MechaTribeAssault_Beginner")
    for name in names:
        path = folder / (name + ".json")
        chart = read_json(path)
        result.append({"id": chart["trackId"], "title": chart["title"],
                       "artist": chart["artist"], "audioAssetPath": chart["audioAssetPath"],
                       "sourceChart": path.relative_to(ROOT).as_posix(),
                       "license": "Upstream noncommercial restrictions; exact version unspecified",
                       "source": {key: chart[key] for key in ("sourceAudioUrl", "sourceAudioSha256", "sourceChartUrl", "sourceChartSha256", "usage")},
                       "chart": chart, "bpm": chart["bpm"],
                       "duration": chart["durationSeconds"], "stepmania": True})
    for directory, manifest_name in (("OpenMusic20261028", "open-music-sources.json"),
                                     ("Classics20261029", "classic-music-sources.json")):
        manifest = read_json(Path(__file__).with_name(manifest_name))
        for entry in manifest["songs"]:
            path = ROOT / "Assets/StreamingAssets/Rhythm" / directory / (entry["id"] + ".json")
            chart = read_json(path)
            result.append({"id": entry["id"], "title": entry["title"],
                           "artist": entry["author"], "audioAssetPath": f"Assets/Audio/Music/{directory}/{entry['filename']}",
                           "sourceChart": path.relative_to(ROOT).as_posix(), "license": entry["license"],
                           "source": entry, "chart": chart, "bpm": entry["bpm"],
                           "duration": chart.get("audioDurationSeconds", chart["durationSeconds"]),
                           "stepmania": False})
    for track in result:
        track["historicalPreparedDurationSeconds"] = track["duration"]
        samples, rate = playable_samples(ROOT / track["audioAssetPath"])
        track["playableSampleCount"], track["playableSampleRate"] = samples, rate
        track["duration"] = samples / rate
    return result


def analyze_track(track):
    import numpy as np
    executable = shutil.which("ffmpeg")
    if not executable:
        raise RuntimeError("--analyze requires FFmpeg on PATH")
    rate, hop, frame = 11025, 128, 1024
    audio_path = ROOT / track["audioAssetPath"]
    probe = subprocess.run([shutil.which("ffprobe") or "ffprobe", "-v", "error", "-show_entries",
                            "stream=duration,start_time:format=duration,start_time", "-of", "json", str(audio_path)],
                           check=True, capture_output=True, text=True)
    probe_metadata = json.loads(probe.stdout)
    pcm = subprocess.run([executable, "-v", "error", "-i", str(audio_path), "-ac", "1", "-ar", str(rate), "-f", "f32le", "-"],
                         check=True, capture_output=True).stdout
    samples = np.frombuffer(pcm, dtype="<f4")
    windows = np.lib.stride_tricks.sliding_window_view(samples, frame)[::hop]
    spectrum = np.log1p(20 * np.abs(np.fft.rfft(windows * np.hanning(frame), axis=1)))
    flux = np.maximum(np.diff(spectrum, axis=0), 0).mean(axis=1)
    # Frame-centre times; local smoothing avoids giving a single FFT hop undue weight.
    times = (np.arange(len(flux)) * hop + hop + frame / 2) / rate
    flux = np.convolve(flux, np.array([0.2, 0.6, 0.2]), mode="same")
    scale = max(float(np.percentile(flux, 90)), 1e-8)
    flux = np.minimum(flux / scale, 3)
    period = 60 / track["bpm"]
    phases = np.linspace(0, period, 128, endpoint=False)
    valid_start, valid_end = 2.0, max(3, len(samples) / rate - 2)

    def score_phase(phase, start, end):
        beats = np.arange(phase, end, period)
        beats = beats[beats >= start]
        return float(np.mean(np.interp(beats, times, flux))) if len(beats) else 0.0

    scores = [score_phase(float(p), valid_start, valid_end) for p in phases]
    best = int(np.argmax(scores))
    phase = float(phases[best])
    local_phases = []
    for i in range(3):
        a = valid_start + (valid_end - valid_start) * i / 3
        b = valid_start + (valid_end - valid_start) * (i + 1) / 3
        local_phases.append(float(phases[int(np.argmax([score_phase(float(p), a, b) for p in phases]))]))
    drifts = [min(abs(p - phase), period - abs(p - phase)) for p in local_phases]
    # A compact onset snapshot permits deterministic regeneration without FFmpeg.
    sample_times = np.arange(0, float(times[-1]), 0.025)
    envelope = np.interp(sample_times, times, flux)
    return {"audioSha256": sha(audio_path), "bpm": track["bpm"],
            "estimatedBeatPhaseSeconds": round(phase, 9),
            "beatScore": round(scores[best], 6),
            "halfBeatScore": round(score_phase((phase + period / 2) % period, valid_start, valid_end), 6),
            "maxThirdPhaseDisagreementSeconds": round(max(drifts), 6),
            "decodedDurationSeconds": round(len(samples) / rate, 9),
            "containerProbe": probe_metadata,
            "onsetEnvelopeStepSeconds": 0.025,
            "onsetEnvelope": [round(float(v), 4) for v in envelope],
            "method": "FFmpeg mono 11025Hz; 1024-sample Hann FFT, 128 hop; positive log-spectral flux; fixed documented BPM, 128-phase search. Source-chart timings override inferred phase for StepMania.",
            "humanListeningCalibrated": False}


def beat_time(track, beat, analysis):
    if not track["stepmania"]:
        return analysis["estimatedBeatPhaseSeconds"] + beat * 60 / track["bpm"]
    chart = track["chart"]
    segments = chart.get("bpmSegments", [{"beat": 0, "bpm": track["bpm"]}])
    seconds = chart.get("firstBeatOffsetSeconds", 0)
    for index, segment in enumerate(segments):
        if beat <= segment["beat"]:
            break
        stop = min(beat, segments[index + 1]["beat"] if index + 1 < len(segments) else beat)
        seconds += (stop - segment["beat"]) * 60 / segment["bpm"]
    return seconds


def signal_strength(analysis, time):
    values = analysis["onsetEnvelope"]
    index = max(0, min(len(values) - 1, round(time / analysis["onsetEnvelopeStepSeconds"])))
    return values[index]


def festival_window(track, analysis):
    # Whole-bar lengths near 65 seconds; 8-bar candidate starts provide repeatable
    # beat-aligned windows, selected for activity rather than asserted song form.
    bars = min(range(8, 65), key=lambda b: abs(b * 4 * 60 / track["bpm"] - 65))
    beats = bars * 4
    candidates = []
    for start_beat in range(0, int(track["duration"] * track["bpm"] / 60), 32):
        start, end = beat_time(track, start_beat, analysis), beat_time(track, start_beat + beats, analysis)
        length = end - start
        if start < 0 or not (60 <= length <= 75) or end > track["duration"] - 0.25:
            continue
        mean = sum(signal_strength(analysis, start + i * length / 64) for i in range(64)) / 64
        ending = signal_strength(analysis, end - 0.1)
        # Prefer strong sections with a quieter boundary; very late entries add
        # no benefit at the booth, so ties favour earlier material.
        candidates.append((mean - 0.08 * ending - 0.0001 * start, start_beat, start, end))
    if not candidates:
        raise ValueError(f"No coherent bounded window candidate: {track['title']}")
    _, start_beat, start, end = max(candidates)
    return start_beat, start, end


def sections_for(track, analysis, cue_in, duration, start_beat):
    boundaries = [0.0]
    next_beat = start_beat + 32
    while True:
        time = beat_time(track, next_beat, analysis) - cue_in
        if time >= duration - 2:
            break
        if time > boundaries[-1] + 1:
            boundaries.append(time)
        next_beat += 32
    boundaries.append(duration)
    sequence = (0, 1, 2, 0, 1, 2, 0, 1)
    return [{"startSeconds": round(a, 9), "endSeconds": round(b, 9),
             "pattern": sequence[i % len(sequence)], "direction": -1 if i % 3 == 2 else 1}
            for i, (a, b) in enumerate(zip(boundaries, boundaries[1:]))]


def prepare_chart(track, analysis, mode):
    start_beat, cue_in, cue_out = festival_window(track, analysis) if mode == "festival" else (0, 0.0, track["duration"])
    duration = cue_out - cue_in
    sections = sections_for(track, analysis, cue_in, duration, start_beat)
    # One opening demonstration: no lingering targets or unrelated preview can
    # overlap it. Two beats between answers leaves room for moving the aim.
    step_beats = 2 if 2 * 60 / track["bpm"] >= MIN_GAP else 4
    step = step_beats * 60 / track["bpm"]
    # Answer is aligned to an actual source beat; offset the demo from that.
    answer_beat = start_beat + 4 * step_beats
    answer_start = beat_time(track, answer_beat, analysis) - cue_in
    demo_start = answer_start - 4 * step
    if demo_start < 0.6:
        answer_beat += 4
        answer_start = beat_time(track, answer_beat, analysis) - cue_in
        demo_start = answer_start - 4 * step
    demo = {"startSeconds": round(demo_start, 9), "stepSeconds": round(step, 9),
            "answerStartSeconds": round(answer_start, 9), "slots": [-1, 0, 1]}
    response_end = answer_start + 2 * step
    events = []
    if track["stepmania"]:
        candidates = [{"timeSeconds": event["timeSeconds"], "sourceEventId": event["id"],
                       "sourceBeat": event["beat"]} for event in track["chart"]["events"]]
    else:
        candidates = []
        beat = 4
        while beat_time(track, beat, analysis) < cue_out - 1.2:
            # Alternating two-bar motifs avoid a uniform metronome grid while
            # retaining predictable strong-beat groups.
            motif = (0, 2, 4, 6) if (beat // 32) % 3 != 2 else (0, 3, 4, 6)
            for offset in motif:
                source_time = beat_time(track, beat + offset, analysis)
                candidates.append({"timeSeconds": source_time, "sourceBeat": beat + offset})
            beat += 8
    threshold = cue_in + response_end + LEAD + 0.1
    candidates = [event for event in candidates if threshold <= event["timeSeconds"] < cue_out - 1.2]
    # Strongest candidate wins within crowded source clusters, preserving its
    # exact source time. Never move an original StepMania judgement timestamp.
    thinned = []
    for event in sorted(candidates, key=lambda e: e["timeSeconds"]):
        if thinned and event["timeSeconds"] - thinned[-1]["timeSeconds"] < MIN_GAP - 1e-8:
            if signal_strength(analysis, event["timeSeconds"]) > signal_strength(analysis, thinned[-1]["timeSeconds"]):
                if len(thinned) < 2 or event["timeSeconds"] - thinned[-2]["timeSeconds"] >= MIN_GAP:
                    thinned[-1] = event
            continue
        thinned.append(event)
    # The only inserted scoring notes are explicitly project-authored answer
    # notes, distinct from the non-scoring demonstration markers.
    for i, slot in enumerate(demo["slots"]):
        events.append({"timeSeconds": cue_in + answer_start + i * step,
                       "authoredPlacement": True, "placementSlot": slot, "answerIndex": i})
    slots = (-2, -1, 0, 1, 2, 1, 0, -1)
    for index, event in enumerate(thinned):
        time = event["timeSeconds"] - cue_in
        section = next(section for section in sections if section["startSeconds"] <= time < section["endSeconds"])
        event.update(authoredPlacement=True, placementSlot=slots[index % len(slots)])
        events.append(event)
    for i, event in enumerate(events):
        event["id"] = i
        event["timeSeconds"] = round(event["timeSeconds"], 9)
    if not events or any(b["timeSeconds"] - a["timeSeconds"] < MIN_GAP - 1e-7 for a, b in zip(events, events[1:])):
        raise ValueError("Event cadence violated")
    peak = max(sum(0 <= event["timeSeconds"] - previous["timeSeconds"] <= LEAD + CLEANUP for previous in events)
               for event in events)
    if peak > 12:
        raise ValueError("Source congestion exceeds authored budget")
    profile_id = track["id"] + "-" + mode + "-phrase-v2"
    chart = {"schemaVersion": 2, "title": track["title"], "artist": track["artist"],
             "chartAuthor": "BalloonBarrage project; source authors retained in sourceChart",
             "difficulty": "Relaxed phrase", "bpm": track["bpm"],
             "durationSeconds": track["duration"], "roundDurationSeconds": round(duration, 9),
             "license": track["license"], "sourceChart": track["sourceChart"],
             "sourceChartSha256": sha(ROOT / track["sourceChart"]), "source": track["source"],
             "audioAssetPath": track["audioAssetPath"], "audioSha256": sha(ROOT / track["audioAssetPath"]),
             "adaptation": "Project-authored phrase/placement adaptation. Source StepMania event times are retained when selected; crowded rows are thinned and three new answer events are explicitly authored. Other songs use documented BPM and signal-estimated phase, alternating beat motifs. Opening noninteractive demonstration markers never score. Sections are beat-aligned 8-bar arrangements inferred from timing, not verified musical form. Audio bytes, speed and pitch unchanged. Human listening/aim calibration remains unverified.",
             "coordinateContract": "Event timeSeconds is source-audio time. Sections and demonstrations use round-relative seconds. Subtract profile.sourceCueInSeconds from event time once.",
             "humanListeningCalibrated": False,
             "estimatedPeakActiveWithoutHits": peak,
             "minimumEventGapSeconds": round(min(b["timeSeconds"] - a["timeSeconds"] for a, b in zip(events, events[1:])), 9),
             "sections": sections, "demonstrations": [demo], "events": events}
    if track["license"] == "CC-BY-SA-3.0":
        chart["licenseUrl"] = "https://creativecommons.org/licenses/by-sa/3.0/"
        chart["adaptation"] += " This derived chart and synchronized-sequence contributions are CC BY-SA 3.0."
    path = DATA / (track["id"] + "-" + mode + ".json")
    profile = {"trackId": profile_id, "title": track["title"], "artist": track["artist"],
               "mode": mode, "audioAssetPath": track["audioAssetPath"],
               "chartRelativePath": path.relative_to(ROOT / "Assets/StreamingAssets").as_posix(),
               "sourceCueInSeconds": round(cue_in, 9), "sourceCueOutSeconds": round(cue_out, 9),
               "roundDurationSeconds": round(duration, 9), "sourceEventStartId": 0,
               "sourceEventEndIdExclusive": len(events)}
    return path, chart, profile


def unity_meta(path, folder=False):
    path = Path(path)
    if path.with_name(path.name + ".meta").exists():
        return
    guid = uuid.uuid5(uuid.NAMESPACE_URL, "BalloonBarrage/" + path.relative_to(ROOT).as_posix()).hex
    body = f"fileFormatVersion: 2\nguid: {guid}\n"
    if folder:
        body += "folderAsset: yes\nDefaultImporter:\n"
    else:
        body += "TextScriptImporter:\n"
    body += "  externalObjects: {}\n  userData:\n  assetBundleName:\n  assetBundleVariant:\n"
    path.with_name(path.name + ".meta").write_text(body, encoding="utf-8", newline="\n")


def prepare(analyze=False):
    DATA.mkdir(parents=True, exist_ok=True)
    unity_meta(DATA, True)
    all_tracks = tracks()
    analysis_path = DATA / "signal-analysis.json"
    if analyze:
        snapshot = {"schemaVersion": 1, "date": "2026-10-29", "humanListeningCalibrated": False,
                    "tracks": {track["id"]: analyze_track(track) for track in all_tracks}}
        analysis_path.write_bytes(json_bytes(snapshot))
    snapshot = read_json(analysis_path)
    profiles = []
    generated = {}
    for mode in ("festival", "standard"):
        for track in all_tracks:
            analysis = snapshot["tracks"][track["id"]]
            if analysis["audioSha256"] != sha(ROOT / track["audioAssetPath"]):
                raise ValueError(f"Analysis no longer matches original audio: {track['title']}")
            path, chart, profile = prepare_chart(track, analysis, mode)
            generated[path] = json_bytes(chart)
            profiles.append(profile)
            print(f"{mode}: {track['title']}, {profile['roundDurationSeconds']:.3f}s, {len(chart['events'])} events, peak {chart['estimatedPeakActiveWithoutHits']}")
    generated[DATA / "catalog.json"] = json_bytes({"schemaVersion": 2,
        "catalogId": "kyudaisai-2026-phrase-v2", "defaultMode": "festival",
        "status": "prepared-derived-data-awaiting-runtime-and-package-verification",
        "humanListeningCalibrated": False, "paintRibbonImplemented": False,
        "songs": profiles})
    generated[DATA / "duration-audit.json"] = json_bytes({
        "schemaVersion": 1,
        "method": "Vorbis final EOS granule / original sample rate; MP3 parsed frame sample count / sample rate. FFprobe container estimates and resampled FFmpeg PCM counts are diagnostics, not substitutes for EOS trimming or Unity clip verification.",
        "knownCorrection": "Mecha-Tribe Assault has an earlier oversized granule of 4692672, but the final EOS granule is 4630500 at 44100Hz, so its playable bound is 105 seconds. Historical prepared metadata used the maximum granule (106.409795918s). Decoded FFmpeg PCM can disagree because of this stream's trimming/timestamp sequence. Original audio and historical source metadata remain byte-identical.",
        "tracks": [{"trackId": track["id"], "audioAssetPath": track["audioAssetPath"],
                    "audioSha256": sha(ROOT / track["audioAssetPath"]),
                    "playableSampleCount": track["playableSampleCount"],
                    "playableSampleRate": track["playableSampleRate"],
                    "playableDurationSeconds": round(track["duration"], 9),
                    "historicalPreparedDurationSeconds": track["historicalPreparedDurationSeconds"],
                    "authority": "final-vorbis-eos-granule" if track["audioAssetPath"].lower().endswith(".ogg") else "parsed-mp3-frame-samples",
                    "resampledFfmpegDecodedDurationSeconds": snapshot["tracks"][track["id"]]["decodedDurationSeconds"],
                    "containerProbe": snapshot["tracks"][track["id"]].get("containerProbe", {})}
                   for track in all_tracks]})
    generated[DATA / "NOTICE.txt"] = (
        "BalloonBarrage festival/complete-song phrase adaptations — October 29, 2026\n\n"
        "No source recording or original chart was modified. See each derived chart's source,\n"
        "sourceChart, hashes and adaptation fields, and the original preserved manifests/notices.\n"
        "Goin' Under by NegaRen (chart Fraxtil), Springtime by Kommisar (chart Kommisar),\n"
        "Mecha-Tribe Assault by Kommisar (chart Wyde): pinned StepMania noncommercial\n"
        "development permission only; not commercially cleared. Original notices govern.\n"
        "Cozy Puzzle In-Game 1/2 and Magic Puzzle In-Game 1/2 by MintoDog: CC0 1.0.\n"
        "Chaoz Fantasy, Chaoz Airflow and Chaoz Impact by ParagonX9: CC BY-SA 3.0.\n"
        "https://creativecommons.org/licenses/by-sa/3.0/\n"
        "These classics' derived charts and synchronized-sequence contributions are under\n"
        "the same CC BY-SA 3.0; retain attribution, modification statements and license.\n"
        "No endorsement is implied. Other independent software/assets keep their notices.\n"
        "The complete mixed package remains noncommercial due to the StepMania content.\n\n"
        "Changes: separate 60–75-second festival windows and complete-song standard profiles;\n"
        "source-tempo 8-bar section arrangements, bounded target density, alternating motifs,\n"
        "one opening watch/answer motif and explicit spatial placement. Analysis uses\n"
        "spectral onset energy to estimate grid phase and select active beat-bounded windows.\n"
        "Sections/phrase boundaries are inferred, not human-confirmed musical structure.\n"
        "The timing grid for Chaoz Impact remains provisional. Human listening, physical aim,\n"
        "venue calibration and exact playback-window quality remain unverified.\n"
        "Duration correction: Mecha-Tribe's final Vorbis EOS trims the playable recording\n"
        "to 105 seconds; historical maximum-granule metadata counted extra padding.\n"
        "See duration-audit.json. Original audio bytes and source charts remain unchanged.\n"
        "Demonstration markers do not score; long-note paint ribbon is not implemented.\n"
    ).encode("utf-8")
    generated[DATA / "CC-BY-SA-3.0.txt"] = (ROOT / "Assets/StreamingAssets/Rhythm/Classics20261029/CC-BY-SA-3.0.txt").read_bytes()
    for path, data in generated.items():
        path.write_bytes(data)
        unity_meta(path)
    unity_meta(analysis_path)
    return generated


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--analyze", action="store_true")
    prepare(parser.parse_args().analyze)
