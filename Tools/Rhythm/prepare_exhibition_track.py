"""Prepare the pinned Goin' Under Beginner chart; never modify source assets."""
from __future__ import annotations

import hashlib
import json
import math
from pathlib import Path
import re
import struct

ROOT = Path(__file__).resolve().parents[2]
SOURCE = ROOT / "Assets/StreamingAssets/Rhythm/StepMania"
SONG = "Songs/StepMania 5/Goin' Under"
AUDIO = SONG + "/Goin' Under.ogg"
CHART = SONG + "/Goin' Under.ssc"
RUNTIME_AUDIO = ROOT / "Assets/Audio/Music/StepMania/GoinUnder.ogg"
RUNTIME_CHART = ROOT / "Assets/StreamingAssets/Rhythm/Prepared/GoinUnder_Beginner.json"


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


def constant_chart(text: str) -> dict:
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
    bpm_changes = [item.strip() for item in timing["BPMS"].split(",") if item.strip()]
    if len(bpm_changes) != 1:
        raise ValueError("This preparation tool requires one constant BPM")
    first_beat, bpm = map(float, bpm_changes[0].split("="))
    offset = float(timing["OFFSET"] or "0")
    if first_beat != 0 or not math.isfinite(bpm) or bpm <= 0 or not math.isfinite(offset):
        raise ValueError("Invalid initial BPM/offset")
    signatures = timing["TIMESIGNATURES"].split("=")
    if signatures and timing["TIMESIGNATURES"]:
        if len(signatures) != 3 or tuple(map(float, signatures)) != (0, 4, 4):
            raise ValueError("Only the pinned 4/4 signature is supported")
    events = []
    for measure, body in enumerate(chart["NOTES"].split(",")):
        rows = [line.strip() for line in body.splitlines() if line.strip()]
        if not rows:
            raise ValueError(f"Empty measure {measure}")
        for row_index, row in enumerate(rows):
            if len(row) != 4 or set(row) - {"0", "1"}:
                raise ValueError("Only four-lane tap rows are supported; holds/mines need a separate adaptation")
            lanes = [lane for lane, symbol in enumerate(row) if symbol == "1"]
            if lanes:
                beat = measure * 4 + row_index * 4 / len(rows)
                # StepMania beat-zero time is -OFFSET relative to audio start.
                seconds = beat * 60 / bpm - offset
                if seconds < 0:
                    raise ValueError("Pre-audio notes need an explicit lead-in policy")
                events.append({"id": len(events), "beat": round(beat, 9),
                               "timeSeconds": round(seconds, 9), "lanes": lanes})
    if not events:
        raise ValueError("The selected chart has no tap events")
    return {"title": song["TITLE"], "artist": song["ARTIST"],
            "chartAuthor": chart.get("CREDIT", song.get("CREDIT", "")),
            "difficulty": "Beginner", "stepType": "dance-single", "bpm": bpm,
            "sourceOffsetSeconds": offset, "firstBeatOffsetSeconds": -offset,
            "events": events}


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


def write_if_new_or_identical(path: Path, data: bytes) -> None:
    if path.exists() and path.read_bytes() != data:
        raise ValueError(f"Refusing to overwrite a changed prepared asset: {path}")
    path.parent.mkdir(parents=True, exist_ok=True)
    if not path.exists():
        path.write_bytes(data)


def main() -> None:
    manifest = json.loads((ROOT / "Tools/Rhythm/references.json").read_text(encoding="utf-8-sig"))
    entries = {item["path"]: item for item in manifest["files"]}
    originals = {}
    for relative in (AUDIO, CHART):
        data = (SOURCE / relative).read_bytes()
        item = entries[relative]
        if len(data) != item["bytes"] or sha256(data) != item["sha256"]:
            raise ValueError(f"Source differs from the pinned manifest: {relative}")
        originals[relative] = data
    prepared = constant_chart(originals[CHART].decode("utf-8-sig"))
    samples, rate = ogg_duration(originals[AUDIO])
    duration = samples / rate
    if any(event["timeSeconds"] >= duration for event in prepared["events"]):
        raise ValueError("Chart note lies outside the audio duration")
    prepared.update({"schemaVersion": 1, "trackId": "stepmania-goin-under-beginner",
                     "audioAssetPath": RUNTIME_AUDIO.relative_to(ROOT).as_posix(),
                     "durationSeconds": round(duration, 9), "audioSampleCount": samples,
                     "audioSampleRate": rate, "loop": False,
                     "sourceRevision": manifest["revision"],
                     "sourceAudioPath": "Assets/StreamingAssets/Rhythm/StepMania/" + AUDIO,
                     "sourceChartPath": "Assets/StreamingAssets/Rhythm/StepMania/" + CHART,
                     "sourceAudioSha256": entries[AUDIO]["sha256"],
                     "sourceChartSha256": entries[CHART]["sha256"],
                     "sourceAudioUrl": entries[AUDIO]["url"],
                     "sourceChartUrl": entries[CHART]["url"],
                     "usage": manifest["usage"],
                     "adaptation": "Full song. One timed balloon event per tap row; simultaneous lanes share one event. Original lanes retained as provenance, not screen coordinates. Gameplay not implemented."})
    output = (json.dumps(prepared, indent=2, ensure_ascii=False) + "\n").encode("utf-8")
    # Check both destinations before writing either, so a conflict cannot partially replace content.
    for path, data in ((RUNTIME_AUDIO, originals[AUDIO]), (RUNTIME_CHART, output)):
        if path.exists() and path.read_bytes() != data:
            raise ValueError(f"Prepared destination changed: {path}")
    write_if_new_or_identical(RUNTIME_AUDIO, originals[AUDIO])
    write_if_new_or_identical(RUNTIME_CHART, output)
    print(f"PREPARED_TRACK_OK: {prepared['title']}, {len(prepared['events'])} events, "
          f"{duration:.6f}s, first note {prepared['events'][0]['timeSeconds']:.6f}s")


if __name__ == "__main__":
    main()
