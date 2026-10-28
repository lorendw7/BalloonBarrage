"""Verify distribution provenance, unchanged audio, and playable profile bounds."""
import hashlib
import json
import tempfile
import unittest
from pathlib import Path
from unittest.mock import patch

import prepare_open_music as open_music
from prepare_exhibition_track import audio_duration

ROOT = Path(__file__).resolve().parents[2]


class OpenMusicTests(unittest.TestCase):
    def test_pinned_recordings_and_profile_windows(self):
        identities = set()
        for directory, manifest_name in (("OpenMusic20261028", "open-music-sources.json"),
                                          ("Classics20261029", "classic-music-sources.json")):
            manifest = json.loads((Path(__file__).parent / manifest_name).read_text())
            profiles = json.loads((ROOT / "Assets/StreamingAssets/Rhythm" / directory / "profiles.json").read_text())["songs"]
            self.assertEqual(len(profiles), len(manifest["songs"]))
            for entry, profile in zip(manifest["songs"], profiles):
                data = (ROOT / profile["audioPath"]).read_bytes()
                self.assertEqual(len(data), entry["bytes"])
                self.assertEqual(hashlib.sha256(data).hexdigest(), entry["sha256"])
                samples, rate = audio_duration(data, entry["filename"])
                self.assertLessEqual(profile["sourceCueOutSeconds"], samples / rate)
                chart = json.loads((ROOT / "Assets/StreamingAssets" / profile["chartRelativePath"]).read_text())
                events = chart["events"]
                self.assertEqual([e["id"] for e in events], list(range(len(events))))
                times = [e["timeSeconds"] for e in events]
                self.assertEqual(times, sorted(set(times)))
                self.assertGreaterEqual(times[0], 1)
                self.assertLess(times[-1], profile["roundDurationSeconds"])
                self.assertEqual(len(events), profile["sourceEventEndIdExclusive"])
                self.assertEqual(entry["license"], chart["license"])
                identities.add(profile["trackId"])
                if directory.startswith("Classics"):
                    self.assertEqual(data, (ROOT / "Assets/StreamingAssets/Rhythm" / directory / "Originals" / entry["filename"]).read_bytes())
                    self.assertAlmostEqual(profile["roundDurationSeconds"], samples / rate)
        self.assertEqual(len(identities), 7)

    def test_altered_cache_is_rejected(self):
        with tempfile.TemporaryDirectory() as directory, patch.object(open_music, "CACHE", Path(directory)):
            Path(directory, "song.ogg").write_bytes(b"changed")
            entry = {"filename": "song.ogg", "bytes": 7, "sha256": "0" * 64}
            with self.assertRaisesRegex(ValueError, "mismatch"):
                open_music.fetch(entry, False)

    def test_kenney_source_bytes_are_preserved(self):
        folder = ROOT / "Assets/StudioArtPack/OpenExpansion20261028/KenneyFurniture"
        manifest = json.loads((folder / "source-manifest.json").read_text())
        self.assertEqual(manifest["license"], "CC0-1.0")
        for entry in manifest["files"]:
            data = (folder / entry["path"]).read_bytes()
            self.assertEqual(hashlib.sha256(data).hexdigest(), entry["sha256"])
            self.assertEqual(len(data), entry["bytes"])


if __name__ == "__main__":
    unittest.main()
