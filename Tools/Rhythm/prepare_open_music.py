"""Reproduce the CC0 expansion from hash-pinned downloads; never alter audio."""
from __future__ import annotations

import argparse
import hashlib
import json
from pathlib import Path
import urllib.request
from zipfile import ZipFile

from prepare_exhibition_track import ogg_duration, write_if_new_or_identical

ROOT = Path(__file__).resolve().parents[2]
CACHE = ROOT / ".local-reference/OpenExpansion20261028"
MANIFEST = Path(__file__).with_name("open-music-sources.json")
AUDIO = ROOT / "Assets/Audio/Music/OpenMusic20261028"
DATA = ROOT / "Assets/StreamingAssets/Rhythm/OpenMusic20261028"
ART = ROOT / "Assets/StudioArtPack/OpenExpansion20261028"


def digest(data: bytes) -> str:
    return hashlib.sha256(data).hexdigest()


def fetch(entry: dict, download: bool) -> bytes:
    path = CACHE / entry["filename"]
    if not path.exists():
        if not download:
            raise ValueError(f"Missing cache: {path}; use --download")
        CACHE.mkdir(parents=True, exist_ok=True)
        request = urllib.request.Request(entry["downloadUrl"], headers={"User-Agent": "BalloonBarrage asset preparation"})
        with urllib.request.urlopen(request, timeout=120) as response:
            data = response.read()
        if digest(data) != entry["sha256"]:
            raise ValueError(f"Upstream download changed: {entry['filename']}")
        path.write_bytes(data)
    data = path.read_bytes()
    if len(data) != entry["bytes"] or digest(data) != entry["sha256"]:
        raise ValueError(f"Pinned download mismatch: {path}")
    return data


def prepare(download: bool = False) -> None:
    manifest = json.loads(MANIFEST.read_text(encoding="utf-8"))
    profiles = []
    for entry in manifest["songs"]:
        audio = fetch(entry, download)
        samples, rate = ogg_duration(audio)
        duration = samples / rate
        bpm = entry["bpm"]
        beats = entry["roundBars"] * 4
        round_duration = beats * 60 / bpm
        assert round_duration <= duration and 75 <= round_duration <= 90
        events = [{"id": index, "beat": beat, "timeSeconds": round(beat * 60 / bpm, 9)}
                  for index, beat in enumerate(range(4, beats - 2, 2))]
        chart = {
            "title": entry["title"], "artist": "MintoDog", "chartAuthor": "BalloonBarrage project",
            "difficulty": "Practice", "bpm": bpm, "durationSeconds": round_duration,
            "audioDurationSeconds": duration, "audioSampleCount": samples, "audioSampleRate": rate,
            "source": entry, "license": "CC0-1.0", "events": events,
            "adaptation": "Project-authored two-beat practice grid using the tempo in the author's filename, phase zero. Four-beat lead-in, no audio edits. Not an upstream authored chart; listening calibration remains unverified.",
        }
        write_if_new_or_identical(AUDIO / entry["filename"], audio)
        write_if_new_or_identical(DATA / (entry["id"] + ".json"), (json.dumps(chart, indent=2) + "\n").encode())
        profiles.append({
            "trackId": entry["id"] + "-practice-v1", "title": entry["title"], "artist": "MintoDog",
            "audioPath": (AUDIO / entry["filename"]).relative_to(ROOT).as_posix(),
            "chartRelativePath": "Rhythm/OpenMusic20261028/" + entry["id"] + ".json",
            "sourceCueInSeconds": 0, "sourceCueOutSeconds": round_duration,
            "roundDurationSeconds": round_duration, "sourceEventStartId": 0,
            "sourceEventEndIdExclusive": len(events),
        })
        print(f"{entry['title']}: {len(events)} events, {round_duration:.3f}s, unchanged {duration:.3f}s source")
    write_if_new_or_identical(DATA / "profiles.json", (json.dumps({"songs": profiles}, indent=2) + "\n").encode())
    write_if_new_or_identical(DATA / "source-manifest.json", MANIFEST.read_bytes())
    notice = ("CC0 music expansion, retrieved October 28, 2026.\n\n"
              "Cozy Puzzle In-Game 1 & 2 and Magic Puzzle In-Game 1 & 2 by MintoDog.\n"
              "Author-uploaded OpenGameArt pages explicitly identify each recording as CC0.\n"
              "License: https://creativecommons.org/publicdomain/zero/1.0/\n"
              "Audio is byte-identical. The project selects 80-87 second windows and authors practice grids.\n"
              "No upstream chart or source DAW project was supplied. Timing/listening calibration is unverified.\n"
              "See source-manifest.json for source pages, exact download URLs, byte lengths and SHA-256.\n"
              "These CC0 assets do not change the separate noncommercial StepMania restrictions.\n")
    write_if_new_or_identical(DATA / "NOTICE.txt", notice.encode())
    write_if_new_or_identical(DATA / "CC0-1.0.txt", (ROOT / "Tools/Release/Notices/CC0-1.0.txt").read_bytes())
    write_if_new_or_identical(AUDIO / "NOTICE.txt", notice.encode())
    entry = manifest["art"]
    fetch(entry, download)
    copied = []
    with ZipFile(CACHE / entry["filename"]) as archive:
        if archive.testzip() is not None:
            raise ValueError("Corrupt furniture archive")
        for name in ["License.txt"] + [f"Models/{model}.{extension}" for model in
                ("pottedPlant", "plantSmall1", "plantSmall2", "plantSmall3", "books") for extension in ("obj", "mtl")]:
            data = archive.read(name)
            write_if_new_or_identical(ART / "KenneyFurniture" / name, data)
            copied.append({"path": name, "bytes": len(data), "sha256": digest(data)})
    write_if_new_or_identical(ART / "KenneyFurniture/source-manifest.json",
                             (json.dumps({**entry, "files": copied}, indent=2) + "\n").encode())


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--download", action="store_true")
    prepare(parser.parse_args().download)
