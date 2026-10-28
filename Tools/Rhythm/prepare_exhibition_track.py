"""Prepare compatible pinned StepMania Beginner charts without modifying sources."""
from __future__ import annotations

from dataclasses import dataclass
import hashlib
import json
import math
from pathlib import Path
import re
import struct

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "Assets/StreamingAssets/Rhythm/StepMania"


@dataclass(frozen=True)
class TrackSpec:
    track_id: str
    song_directory: str
    audio_filename: str
    chart_filename: str
    runtime_audio_filename: str
    runtime_chart_filename: str
    adaptation: str
    adapt_sustains: bool = False

    @property
    def source_audio(self) -> str:
        return f"{self.song_directory}/{self.audio_filename}"

    @property
    def source_chart(self) -> str:
        return f"{self.song_directory}/{self.chart_filename}"

    @property
    def runtime_audio(self) -> Path:
        return ROOT / "Assets/Audio/Music/StepMania" / self.runtime_audio_filename

    @property
    def runtime_chart(self) -> Path:
        return ROOT / "Assets/StreamingAssets/Rhythm/Prepared" / self.runtime_chart_filename


GOIN_UNDER = TrackSpec(
    track_id="stepmania-goin-under-beginner",
    song_directory="Songs/StepMania 5/Goin' Under",
    audio_filename="Goin' Under.ogg",
    chart_filename="Goin' Under.ssc",
    runtime_audio_filename="GoinUnder.ogg",
    runtime_chart_filename="GoinUnder_Beginner.json",
    adaptation=("Full song. One timed balloon event per tap row; simultaneous lanes share one event. "
                "Original lanes retained as provenance, not screen coordinates. Gameplay not implemented."),
)

SPRINGTIME = TrackSpec(
    track_id="stepmania-springtime-beginner",
    song_directory="Songs/StepMania 5/Springtime",
    audio_filename="Kommisar - Springtime.mp3",
    chart_filename="Springtime.ssc",
    runtime_audio_filename="Springtime.mp3",
    runtime_chart_filename="Springtime_Beginner.json",
    adaptation=("Full song. One timed balloon event per tap row using chart-specific BPM segments; "
                "simultaneous lanes share one event. Original lanes retained as provenance, not screen "
                "coordinates. Gameplay not implemented."),
)

MECHA_TRIBE = TrackSpec(
    track_id="stepmania-mecha-tribe-beginner",
    song_directory="Songs/StepMania 5/MechaTribe Assault",
    audio_filename="Mecha-Tribe Assault.ogg",
    chart_filename="Mecha-Tribe Assault.ssc",
    runtime_audio_filename="MechaTribeAssault.ogg",
    runtime_chart_filename="MechaTribeAssault_Beginner.json",
    adaptation="Balloon adaptation: tap/hold/roll heads become one target per row; sustain tails and mines are omitted. Original chart preserved.",
    adapt_sustains=True,
)

TRACKS = (GOIN_UNDER, SPRINGTIME, MECHA_TRIBE)
RUNTIME_PROFILE_CATALOG = (
    ROOT / "Assets/StreamingAssets/Rhythm/Prepared/Kyudaisai_Candidate.json"
)


@dataclass(frozen=True)
class RoundProfileSpec:
    profile_id: str
    track_id: str
    start_beat: float
    end_beat: float
    pattern_sections: tuple[tuple[str, float, float], ...]


KYUDAISAI_PROFILES = (
    RoundProfileSpec(
        profile_id="kyudaisai-mecha-tribe-v1",
        track_id=MECHA_TRIBE.track_id,
        start_beat=16,
        end_beat=256,
        pattern_sections=(("lateral-sweep", 16, 96), ("shallow-fan", 96, 176),
                          ("staggered-cluster", 176, 256)),
    ),
    RoundProfileSpec(
        profile_id="kyudaisai-goin-under-v1",
        track_id=GOIN_UNDER.track_id,
        start_beat=32,
        end_beat=320,
        pattern_sections=(
            ("lateral-sweep", 32, 128),
            ("shallow-fan", 128, 224),
            ("staggered-cluster", 224, 320),
        ),
    ),
    RoundProfileSpec(
        profile_id="kyudaisai-springtime-v1",
        track_id=SPRINGTIME.track_id,
        start_beat=16,
        end_beat=272,
        pattern_sections=(
            ("lateral-sweep", 16, 96),
            ("shallow-fan", 96, 176),
            ("staggered-cluster", 176, 272),
        ),
    ),
)


def sha256(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def tags(text: str) -> dict[str, str]:
    text = re.sub(r"//[^\n]*", "", text)
    found = re.findall(r"#([A-Z0-9]+):([^;]*);", text, re.S)
    result: dict[str, str] = {}
    for key, value in found:
        if key in result:
            raise ValueError(f"Duplicate SSC tag: {key}")
        result[key] = value.strip()
    return result


def tap_chart(text: str, adapt_sustains: bool = False) -> dict:
    blocks = text.split("#NOTEDATA:;")
    song = tags(blocks[0])
    choices = [tags(block) for block in blocks[1:]]
    selected = [chart for chart in choices if chart.get("STEPSTYPE") == "dance-single"
                and chart.get("DIFFICULTY") == "Beginner"]
    if len(selected) != 1:
        raise ValueError("Expected exactly one dance-single Beginner chart")
    chart = selected[0]
    timing = {key: chart.get(key, song.get(key, "")) for key in
              ("BPMS", "OFFSET", "STOPS", "DELAYS", "WARPS", "FAKES", "TIMESIGNATURES")}
    if any(timing[key] for key in ("STOPS", "DELAYS", "WARPS", "FAKES")):
        raise ValueError("This preparation tool rejects stops, delays, warps and fake regions")
    raw_bpm_changes = [item.strip() for item in timing["BPMS"].split(",") if item.strip()]
    bpm_changes = []
    for item in raw_bpm_changes:
        pieces = item.split("=")
        if len(pieces) != 2:
            raise ValueError("Malformed BPM segment")
        beat, bpm = map(float, pieces)
        if not math.isfinite(beat) or not math.isfinite(bpm) or beat < 0 or bpm <= 0:
            raise ValueError("Invalid BPM segment")
        bpm_changes.append((beat, bpm))
    if not bpm_changes or bpm_changes[0][0] != 0:
        raise ValueError("The first BPM segment must start at beat zero")
    if any(current[0] <= previous[0] for previous, current in zip(bpm_changes, bpm_changes[1:])):
        raise ValueError("BPM segments must have unique ascending beats")
    offset = float(timing["OFFSET"] or "0")
    if not math.isfinite(offset):
        raise ValueError("Invalid offset")
    signatures = timing["TIMESIGNATURES"].split("=")
    if signatures and timing["TIMESIGNATURES"]:
        if len(signatures) != 3 or tuple(map(float, signatures)) != (0, 4, 4):
            raise ValueError("Only the pinned 4/4 signature is supported")
    def beat_seconds(beat: float) -> float:
        seconds = -offset
        for index, (segment_beat, segment_bpm) in enumerate(bpm_changes):
            if beat <= segment_beat:
                break
            next_beat = bpm_changes[index + 1][0] if index + 1 < len(bpm_changes) else beat
            seconds += (min(beat, next_beat) - segment_beat) * 60 / segment_bpm
            if beat <= next_beat:
                break
        return seconds

    events = []
    for measure, body in enumerate(chart["NOTES"].split(",")):
        rows = [line.strip() for line in body.splitlines() if line.strip()]
        if not rows:
            raise ValueError(f"Empty measure {measure}")
        for row_index, row in enumerate(rows):
            allowed = {"0", "1", "2", "3", "4", "M"} if adapt_sustains else {"0", "1"}
            if len(row) != 4 or set(row) - allowed:
                raise ValueError("Only four-lane tap rows are supported; holds/mines need a separate adaptation")
            lanes = [lane for lane, symbol in enumerate(row)
                     if symbol in ({"1", "2", "4"} if adapt_sustains else {"1"})]
            if lanes:
                beat = measure * 4 + row_index * 4 / len(rows)
                # StepMania beat-zero time is -OFFSET relative to audio start.
                seconds = beat_seconds(beat)
                if seconds < 0:
                    raise ValueError("Pre-audio notes need an explicit lead-in policy")
                events.append({"id": len(events), "beat": round(beat, 9),
                               "timeSeconds": round(seconds, 9), "lanes": lanes})
    if not events:
        raise ValueError("The selected chart has no tap events")
    result = {"title": song["TITLE"], "artist": song["ARTIST"],
              "chartAuthor": chart.get("CREDIT", song.get("CREDIT", "")),
              "difficulty": "Beginner", "stepType": "dance-single",
              "bpm": bpm_changes[0][1], "sourceOffsetSeconds": offset,
              "firstBeatOffsetSeconds": -offset, "events": events}
    if len(bpm_changes) > 1:
        result["bpmSegments"] = [
            {"beat": beat, "bpm": bpm} for beat, bpm in bpm_changes
        ]
    return result


def ogg_duration(data: bytes) -> tuple[int, int]:
    position = 0
    samples = -1
    rate = None
    serial = None
    while position < len(data):
        if data[position:position + 4] != b"OggS" or len(data) - position < 27:
            raise ValueError("Invalid Ogg page")
        count = data[position + 26]
        header_end = position + 27 + count
        if header_end > len(data):
            raise ValueError("Truncated Ogg page header")
        page_end = header_end + sum(data[position + 27:header_end])
        if page_end > len(data):
            raise ValueError("Truncated Ogg payload")
        page_serial = struct.unpack_from("<I", data, position + 14)[0]
        if serial is None:
            serial = page_serial
        if serial != page_serial:
            raise ValueError("Chained/multiple Ogg streams need explicit duration handling")
        granule = struct.unpack_from("<q", data, position + 6)[0]
        if granule >= 0:
            samples = max(samples, granule)
        packet = data[header_end:page_end]
        if packet.startswith(b"\x01vorbis"):
            if len(packet) < 16:
                raise ValueError("Truncated Vorbis identification")
            rate = struct.unpack_from("<I", packet, 12)[0]
        position = page_end
    if not rate or samples <= 0:
        raise ValueError("No usable Vorbis duration")
    return samples, rate


def mp3_duration(data: bytes) -> tuple[int, int]:
    position = 0
    if data.startswith(b"ID3"):
        if len(data) < 10 or any(byte & 0x80 for byte in data[6:10]):
            raise ValueError("Invalid ID3v2 header")
        tag_size = sum(byte << shift for byte, shift in zip(data[6:10], (21, 14, 7, 0)))
        position = 10 + tag_size + (10 if data[5] & 0x10 else 0)

    mpeg1_bitrates = (0, 32, 40, 48, 56, 64, 80, 96, 112, 128, 160, 192, 224, 256, 320, 0)
    mpeg2_bitrates = (0, 8, 16, 24, 32, 40, 48, 56, 64, 80, 96, 112, 128, 144, 160, 0)
    base_rates = (44100, 48000, 32000)
    samples = 0
    rate = None
    frame_count = 0

    while position + 4 <= len(data):
        if data[position:position + 3] == b"TAG" or data[position:position + 8] == b"APETAGEX":
            break
        header = int.from_bytes(data[position:position + 4], "big")
        if header & 0xFFE00000 != 0xFFE00000:
            if frame_count:
                raise ValueError("Unexpected data inside MP3 frame sequence")
            position += 1
            continue
        version_id = (header >> 19) & 0x3
        layer_id = (header >> 17) & 0x3
        bitrate_index = (header >> 12) & 0xF
        sample_rate_index = (header >> 10) & 0x3
        padding = (header >> 9) & 0x1
        if version_id == 1 or layer_id != 1 or sample_rate_index == 3:
            raise ValueError("Only MPEG Layer III audio is supported")
        bitrate_table = mpeg1_bitrates if version_id == 3 else mpeg2_bitrates
        bitrate = bitrate_table[bitrate_index]
        if not bitrate:
            raise ValueError("Free or invalid MP3 bitrate is unsupported")
        sample_rate = base_rates[sample_rate_index]
        if version_id == 2:
            sample_rate //= 2
        elif version_id == 0:
            sample_rate //= 4
        if rate is None:
            rate = sample_rate
        elif rate != sample_rate:
            raise ValueError("MP3 sample-rate changes are unsupported")
        samples_per_frame = 1152 if version_id == 3 else 576
        frame_size = ((144 if version_id == 3 else 72) * bitrate * 1000 // sample_rate) + padding
        if position + frame_size > len(data):
            raise ValueError("Truncated MP3 frame")
        samples += samples_per_frame
        frame_count += 1
        position += frame_size

    if not frame_count or not rate:
        raise ValueError("No usable MP3 frames")
    return samples, rate


def audio_duration(data: bytes, filename: str) -> tuple[int, int]:
    suffix = Path(filename).suffix.lower()
    if suffix == ".ogg":
        return ogg_duration(data)
    if suffix == ".mp3":
        return mp3_duration(data)
    raise ValueError(f"Unsupported prepared audio format: {suffix}")


def write_if_new_or_identical(path: Path, data: bytes) -> None:
    if path.exists() and path.read_bytes() != data:
        raise ValueError(f"Refusing to overwrite a changed prepared asset: {path}")
    path.parent.mkdir(parents=True, exist_ok=True)
    if not path.exists():
        path.write_bytes(data)


def build_track(spec: TrackSpec, manifest: dict, entries: dict[str, dict]) -> tuple[dict, list[tuple[Path, bytes]]]:
    originals = {}
    for relative in (spec.source_audio, spec.source_chart):
        data = (SOURCE / relative).read_bytes()
        item = entries[relative]
        if len(data) != item["bytes"] or sha256(data) != item["sha256"]:
            raise ValueError(f"Source differs from the pinned manifest: {relative}")
        originals[relative] = data
    prepared = tap_chart(originals[spec.source_chart].decode("utf-8-sig"), spec.adapt_sustains)
    samples, rate = audio_duration(originals[spec.source_audio], spec.audio_filename)
    duration = samples / rate
    if any(event["timeSeconds"] >= duration for event in prepared["events"]):
        raise ValueError("Chart note lies outside the audio duration")
    prepared.update({"schemaVersion": 1, "trackId": spec.track_id,
                     "audioAssetPath": spec.runtime_audio.relative_to(ROOT).as_posix(),
                     "durationSeconds": round(duration, 9), "audioSampleCount": samples,
                     "audioSampleRate": rate, "loop": False,
                     "sourceRevision": manifest["revision"],
                     "sourceAudioPath": "Assets/StreamingAssets/Rhythm/StepMania/" + spec.source_audio,
                     "sourceChartPath": "Assets/StreamingAssets/Rhythm/StepMania/" + spec.source_chart,
                     "sourceAudioSha256": entries[spec.source_audio]["sha256"],
                     "sourceChartSha256": entries[spec.source_chart]["sha256"],
                     "sourceAudioUrl": entries[spec.source_audio]["url"],
                     "sourceChartUrl": entries[spec.source_chart]["url"],
                     "usage": manifest["usage"],
                     "adaptation": spec.adaptation})
    output = (json.dumps(prepared, indent=2, ensure_ascii=False) + "\n").encode("utf-8")
    return prepared, [(spec.runtime_audio, originals[spec.source_audio]), (spec.runtime_chart, output)]


def build_profile_catalog(prepared_by_id: dict[str, dict]) -> dict:
    patterns = [
        {
            "patternId": "lateral-sweep",
            "placementSlots": [-2, -1, 0, 1, 2, 1, 0, -1],
            "logicalTargetsPerEvent": 1,
            "intent": "A readable side-to-side sequence across chart events.",
        },
        {
            "patternId": "shallow-fan",
            "placementSlots": [-2, 0, 2, -1, 1, 0],
            "logicalTargetsPerEvent": 1,
            "intent": "A shallow alternating fan without simultaneous duplicate scoring targets.",
        },
        {
            "patternId": "staggered-cluster",
            "placementSlots": [-1, 1, 0, 2, -2, 0],
            "logicalTargetsPerEvent": 1,
            "intent": "A compact staggered sequence; chart timing still owns every event.",
        },
    ]
    profiles = []
    for spec in KYUDAISAI_PROFILES:
        chart = prepared_by_id[spec.track_id]
        event_by_beat = {event["beat"]: event for event in chart["events"]}
        if spec.start_beat not in event_by_beat or spec.end_beat not in event_by_beat:
            raise ValueError(f"Round profile boundaries must be chart-event beats: {spec.profile_id}")
        cue_in = event_by_beat[spec.start_beat]["timeSeconds"]
        cue_out = event_by_beat[spec.end_beat]["timeSeconds"]
        included = [event for event in chart["events"]
                    if spec.start_beat <= event["beat"] < spec.end_beat]
        duration = cue_out - cue_in
        if not 75 <= duration <= 90:
            raise ValueError(f"Round profile duration is outside 75-90 seconds: {spec.profile_id}")
        times = [event["timeSeconds"] for event in included]
        peak = max(sum(1 for other in times if time - 7.8 < other <= time) for time in times)
        sections = []
        expected_start = spec.start_beat
        for pattern_id, start_beat, end_beat in spec.pattern_sections:
            if start_beat != expected_start or end_beat <= start_beat:
                raise ValueError(f"Pattern sections are not contiguous: {spec.profile_id}")
            section_events = [event for event in included if start_beat <= event["beat"] < end_beat]
            if not section_events:
                raise ValueError(f"Pattern section has no chart events: {spec.profile_id}/{pattern_id}")
            sections.append({
                "patternId": pattern_id,
                "startBeatInclusive": start_beat,
                "endBeatExclusive": end_beat,
                "sourceEventStartId": section_events[0]["id"],
                "sourceEventEndIdExclusive": section_events[-1]["id"] + 1,
                "eventCount": len(section_events),
            })
            expected_start = end_beat
        if expected_start != spec.end_beat:
            raise ValueError(f"Pattern sections do not cover the profile: {spec.profile_id}")
        profiles.append({
            "roundProfileId": spec.profile_id,
            "trackId": spec.track_id,
            "title": chart["title"],
            "artist": chart["artist"],
            "chartAuthor": chart["chartAuthor"],
            "audioAssetPath": chart["audioAssetPath"],
            "chartAssetPath": next(
                track.runtime_chart.relative_to(ROOT).as_posix()
                for track in TRACKS if track.track_id == spec.track_id
            ),
            "sourceCueInSeconds": round(cue_in, 9),
            "sourceCueOutSeconds": round(cue_out, 9),
            "roundDurationSeconds": round(duration, 9),
            "sourceBeatStartInclusive": spec.start_beat,
            "sourceBeatEndExclusive": spec.end_beat,
            "sourceEventStartId": included[0]["id"],
            "sourceEventEndIdExclusive": included[-1]["id"] + 1,
            "eventCount": len(included),
            "estimatedMaxActiveWith2_8SecondLeadAnd5SecondCleanup": peak,
            "patternSections": sections,
        })
    return {
        "schemaVersion": 1,
        "profileSetId": "kyudaisai-2026-candidate-v1",
        "status": "runtime-wiring-saved-awaiting-current-playtest",
        "distributionBoundary": (
            "Noncommercial development data only. Confirm segmented playback permission before "
            "packaging; otherwise use an unmodified whole track or demo.6."
        ),
        "eventContract": (
            "One logical scoring target per chart event. Placement patterns are deterministic data "
            "and must not add score events."
        ),
        "patterns": patterns,
        "profiles": profiles,
    }


def main() -> None:
    manifest = json.loads((ROOT / "Tools/Rhythm/references.json").read_text(encoding="utf-8-sig"))
    entries = {item["path"]: item for item in manifest["files"]}
    prepared_tracks = []
    destinations = []
    for spec in TRACKS:
        prepared, track_destinations = build_track(spec, manifest, entries)
        prepared_tracks.append(prepared)
        destinations.extend(track_destinations)
    profile_catalog = build_profile_catalog({track["trackId"]: track for track in prepared_tracks})
    profile_output = (
        json.dumps(profile_catalog, indent=2, ensure_ascii=False) + "\n"
    ).encode("utf-8")
    destinations.append((RUNTIME_PROFILE_CATALOG, profile_output))

    # Check every destination before writing any, so a conflict cannot partially replace content.
    for path, data in destinations:
        if path != RUNTIME_PROFILE_CATALOG and path.exists() and path.read_bytes() != data:
            raise ValueError(f"Prepared destination changed: {path}")
    for path, data in destinations:
        if path == RUNTIME_PROFILE_CATALOG:
            path.write_bytes(data)
        else:
            write_if_new_or_identical(path, data)

    for prepared in prepared_tracks:
        print(f"PREPARED_TRACK_OK: {prepared['title']}, {len(prepared['events'])} events, "
              f"{prepared['durationSeconds']:.6f}s, "
              f"first note {prepared['events'][0]['timeSeconds']:.6f}s")
    print(f"PREPARED_TRACKS_OK: {len(prepared_tracks)} tracks")
    print(f"PREPARED_PROFILES_OK: {len(profile_catalog['profiles'])} Kyudaisai profiles")


if __name__ == "__main__":
    main()
