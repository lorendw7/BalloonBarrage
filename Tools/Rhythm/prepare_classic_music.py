"""Prepare unchanged, author-published ParagonX9 recordings and BY-SA practice grids."""
import argparse
import json
from pathlib import Path

from prepare_exhibition_track import mp3_duration, write_if_new_or_identical
from prepare_open_music import ROOT, fetch

DATA = ROOT / "Assets/StreamingAssets/Rhythm/Classics20261029"
AUDIO = ROOT / "Assets/Audio/Music/Classics20261029"
MANIFEST = Path(__file__).with_name("classic-music-sources.json")


def prepare(download=False):
    manifest = json.loads(MANIFEST.read_text(encoding="utf-8"))
    profiles = []
    for entry in manifest["songs"]:
        audio = fetch(entry, download)
        samples, rate = mp3_duration(audio)
        duration = samples / rate
        bpm = entry["bpm"]
        events = [{"id": index, "beat": beat, "timeSeconds": round(beat * 60 / bpm, 9)}
                  for index, beat in enumerate(range(8, int((duration - 3) * bpm / 60), 2))]
        chart = {
            "title": entry["title"], "artist": "ParagonX9", "chartAuthor": "BalloonBarrage project",
            "difficulty": "Practice", "bpm": bpm, "durationSeconds": duration,
            "audioSampleCount": samples, "audioSampleRate": rate,
            "license": "CC-BY-SA-3.0", "licenseUrl": manifest["licenseUrl"], "source": entry,
            "adaptation": "Project-authored two-beat practice grid, phase zero; eight-beat lead-in and three-second ending. Complete unchanged recording, no looping or cropping. This chart and the project's synchronized-sequence contributions are CC BY-SA 3.0. Not an upstream or other game's chart. Listening calibration remains unverified.",
            "events": events,
        }
        write_if_new_or_identical(AUDIO / entry["filename"], audio)
        write_if_new_or_identical(DATA / "Originals" / entry["filename"], audio)
        write_if_new_or_identical(DATA / (entry["id"] + ".json"), (json.dumps(chart, indent=2) + "\n").encode())
        profiles.append({
            "trackId": entry["id"] + "-full-practice-v1", "title": entry["title"], "artist": "ParagonX9",
            "audioPath": (AUDIO / entry["filename"]).relative_to(ROOT).as_posix(),
            "chartRelativePath": "Rhythm/Classics20261029/" + entry["id"] + ".json",
            "sourceCueInSeconds": 0, "sourceCueOutSeconds": duration, "roundDurationSeconds": duration,
            "sourceEventStartId": 0, "sourceEventEndIdExclusive": len(events),
        })
        print(f"{entry['title']}: {len(events)} practice events; full {duration:.3f}s recording")
    write_if_new_or_identical(DATA / "profiles.json", (json.dumps({"songs": profiles}, indent=2) + "\n").encode())
    write_if_new_or_identical(DATA / "source-manifest.json", MANIFEST.read_bytes())
    license_text = fetch(manifest["licenseFile"], download)
    write_if_new_or_identical(DATA / "CC-BY-SA-3.0.txt", license_text)
    notice = ("ParagonX9 classics, retrieved October 29, 2026 (Asia/Tokyo).\n\n"
              "Chaoz Fantasy (2007), Chaoz Impact (2008), Chaoz Airflow (2009), by ParagonX9.\n"
              "Copyright ParagonX9. Author: https://paragonx9.newgrounds.com/\n"
              "License: Creative Commons Attribution-ShareAlike 3.0 Unported\n"
              "https://creativecommons.org/licenses/by-sa/3.0/\n\n"
              "Author pages, exact original CDN download URLs, sizes and SHA-256: source-manifest.json.\n"
              "Recordings remain byte-identical; complete playback, no crops or loops.\n"
              "Project-authored practice grids and project contributions to their synchronized sequences\n"
              "are offered under the same CC BY-SA 3.0 license. No upstream game charts are included.\n"
              "Retain attribution and share adaptations alike. No author endorsement is implied.\n"
              "Unmodified source recordings remain freely accessible under Originals/; no DRM is applied.\n"
              "Other independent software/assets retain their own notices. In particular StepMania songs\n"
              "remain restricted to noncommercial packages; that restriction does not relicense these recordings.\n"
              "Grid phase/listening calibration remains unverified. See each chart's adaptation field.\n")
    write_if_new_or_identical(DATA / "NOTICE.txt", notice.encode())
    write_if_new_or_identical(AUDIO / "NOTICE.txt", notice.encode())


if __name__ == "__main__":
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--download", action="store_true")
    prepare(parser.parse_args().download)
