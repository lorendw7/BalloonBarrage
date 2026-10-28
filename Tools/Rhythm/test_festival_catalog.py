"""Verify the exact twenty-profile festival catalog and its derived-event contract."""
import struct
import re
import unittest

import prepare_festival_catalog as festival


class FestivalCatalogTests(unittest.TestCase):
    @classmethod
    def setUpClass(cls):
        cls.tracks = festival.tracks()
        cls.catalog = festival.read_json(festival.DATA / "catalog.json")
        cls.analysis = festival.read_json(festival.DATA / "signal-analysis.json")["tracks"]
        cls.profiles = cls.catalog["songs"]

    def chart(self, profile):
        return festival.read_json(festival.ROOT / "Assets/StreamingAssets" / profile["chartRelativePath"])

    def test_all_ten_songs_have_distinct_festival_and_complete_profiles(self):
        self.assertEqual(len(self.profiles), 20)
        self.assertEqual(len({p["trackId"] for p in self.profiles}), 20)
        self.assertEqual([p["mode"] for p in self.profiles], ["festival"] * 10 + ["standard"] * 10)
        for track in self.tracks:
            selected = [p for p in self.profiles if p["title"] == track["title"]]
            self.assertEqual(len(selected), 2)
            for profile in selected:
                data = (festival.ROOT / profile["audioAssetPath"]).read_bytes()
                samples, rate = festival.playable_samples(festival.ROOT / profile["audioAssetPath"])
                full_duration = samples / rate
                self.assertLessEqual(profile["sourceCueOutSeconds"], full_duration + 1e-8)
                self.assertAlmostEqual(profile["roundDurationSeconds"], profile["sourceCueOutSeconds"] - profile["sourceCueInSeconds"], places=7)
                if profile["mode"] == "festival":
                    self.assertGreaterEqual(profile["roundDurationSeconds"], 60)
                    self.assertLessEqual(profile["roundDurationSeconds"], 75)
                else:
                    self.assertEqual(profile["sourceCueInSeconds"], 0)
                    self.assertAlmostEqual(profile["roundDurationSeconds"], full_duration, places=7)

    def test_demos_and_answers_are_disjoint_from_normal_events(self):
        for profile in self.profiles:
            chart = self.chart(profile)
            self.assertEqual(len(chart["demonstrations"]), 1)
            demo = chart["demonstrations"][0]
            answer = [e for e in chart["events"] if "answerIndex" in e]
            self.assertEqual(len(answer), 3)
            self.assertEqual([e["placementSlot"] for e in answer], demo["slots"])
            watch_end = demo["startSeconds"] + len(demo["slots"]) * demo["stepSeconds"]
            self.assertGreaterEqual(demo["startSeconds"], 0.6)
            self.assertGreaterEqual(demo["answerStartSeconds"] - watch_end, festival.MIN_GAP - 1e-7)
            for i, event in enumerate(answer):
                expected = profile["sourceCueInSeconds"] + demo["answerStartSeconds"] + i * demo["stepSeconds"]
                self.assertAlmostEqual(event["timeSeconds"], expected, places=7)
            final_answer = demo["answerStartSeconds"] + 2 * demo["stepSeconds"]
            for event in chart["events"]:
                relative = event["timeSeconds"] - profile["sourceCueInSeconds"]
                if "answerIndex" not in event:
                    self.assertGreaterEqual(relative, final_answer + festival.LEAD + .09)
            # The demonstration itself cannot add duplicate logical target ids.
            self.assertEqual([e["id"] for e in chart["events"]], list(range(len(chart["events"]))))

    def test_bounded_density_and_round_event_windows(self):
        for profile in self.profiles:
            chart = self.chart(profile)
            events = chart["events"]
            times = [e["timeSeconds"] for e in events]
            self.assertEqual(times, sorted(set(times)))
            self.assertEqual(profile["sourceEventStartId"], 0)
            self.assertEqual(profile["sourceEventEndIdExclusive"], len(events))
            self.assertGreater(times[0] - profile["sourceCueInSeconds"], 1)
            self.assertLess(times[-1], profile["sourceCueOutSeconds"] - 1)
            self.assertTrue(all(b - a >= festival.MIN_GAP - 1e-7 for a, b in zip(times, times[1:])))
            peak = max(sum(0 <= t - previous <= festival.LEAD + festival.CLEANUP for previous in times) for t in times)
            self.assertLessEqual(peak, 16, profile["title"])
            self.assertEqual(peak, chart["estimatedPeakActiveWithoutHits"])
            self.assertTrue(all(e["authoredPlacement"] and -2 <= e["placementSlot"] <= 2 for e in events))

    def test_density_estimate_uses_production_prefab_expiry_and_scene_capacity(self):
        prefab = (festival.ROOT / "Assets/StudioArtPack/BalloonColors/Prefabs/PlayableBalloon_ThreeColors.prefab").read_text()
        scene = (festival.ROOT / "Assets/Scenes/PrototypeScene.unity").read_text()
        cleanup = float(re.search(r"cleanupDelaySeconds: ([\d.]+)", prefab)[1])
        capacity = int(re.search(r"maxBalloons: (\d+)", scene)[1])
        self.assertEqual(festival.CLEANUP, cleanup)
        for profile in self.profiles:
            self.assertLessEqual(self.chart(profile)["estimatedPeakActiveWithoutHits"], capacity)

    def test_sections_are_contiguous_and_source_times_are_not_double_shifted(self):
        for profile in self.profiles:
            chart = self.chart(profile)
            sections = chart["sections"]
            self.assertEqual(sections[0]["startSeconds"], 0)
            self.assertAlmostEqual(sections[-1]["endSeconds"], profile["roundDurationSeconds"], places=8)
            for left, right in zip(sections, sections[1:]):
                self.assertEqual(left["endSeconds"], right["startSeconds"])
            self.assertEqual({s["pattern"] for s in sections}, {0, 1, 2})
            for section in sections:
                self.assertLess(section["startSeconds"], section["endSeconds"])
                self.assertIn(section["direction"], (-1, 1))
            for event in chart["events"]:
                time = event["timeSeconds"] - profile["sourceCueInSeconds"]
                self.assertEqual(sum(s["startSeconds"] <= time < s["endSeconds"] for s in sections), 1)

    def test_pinned_sources_and_source_event_times_remain_unchanged(self):
        for track in self.tracks:
            expected = track["source"].get("sha256", track["source"].get("sourceAudioSha256"))
            actual = festival.sha(festival.ROOT / track["audioAssetPath"])
            self.assertEqual(expected, actual)
            self.assertEqual(self.analysis[track["id"]]["audioSha256"], actual)
            for profile in (p for p in self.profiles if p["title"] == track["title"]):
                chart = self.chart(profile)
                self.assertEqual(chart["audioSha256"], actual)
                self.assertEqual(chart["sourceChartSha256"], festival.sha(festival.ROOT / track["sourceChart"]))
                if track["stepmania"]:
                    original = {e["id"]: e["timeSeconds"] for e in track["chart"]["events"]}
                    for event in chart["events"]:
                        if "sourceEventId" in event:
                            self.assertEqual(event["timeSeconds"], original[event["sourceEventId"]])

    def test_regeneration_is_identical_and_meta_guids_are_unique(self):
        for mode in ("festival", "standard"):
            for track in self.tracks:
                path, chart, profile = festival.prepare_chart(track, self.analysis[track["id"]], mode)
                self.assertEqual(path.read_bytes(), festival.json_bytes(chart), path.name)
                self.assertIn(profile, self.profiles)
        guids = []
        for path in festival.DATA.rglob("*"):
            if path.is_file() and path.suffix != ".meta":
                meta = path.with_name(path.name + ".meta")
                self.assertTrue(meta.is_file(), str(path))
                guid = next(line.split(": ", 1)[1] for line in meta.read_text().splitlines() if line.startswith("guid:"))
                guids.append(guid)
        self.assertEqual(len(guids), len(set(guids)))

    def test_licenses_and_unverified_human_evidence_remain_explicit(self):
        self.assertFalse(self.catalog["humanListeningCalibrated"])
        self.assertFalse(self.catalog["paintRibbonImplemented"])
        for profile in self.profiles:
            chart = self.chart(profile)
            self.assertFalse(chart["humanListeningCalibrated"])
            self.assertIn("unverified", chart["adaptation"])
            if profile["artist"] == "ParagonX9":
                self.assertEqual(chart["license"], "CC-BY-SA-3.0")
                self.assertIn("same", (festival.DATA / "NOTICE.txt").read_text())
                self.assertEqual((festival.DATA / "CC-BY-SA-3.0.txt").read_bytes(),
                                 (festival.ROOT / "Assets/StreamingAssets/Rhythm/Classics20261029/CC-BY-SA-3.0.txt").read_bytes())

    def test_vorbis_final_eos_trims_earlier_oversized_granule(self):
        def page(granule, flags=0, payload=b"x"):
            header = bytearray(27)
            header[:4] = b"OggS"
            header[5] = flags
            struct.pack_into("<q", header, 6, granule)
            struct.pack_into("<I", header, 14, 11)
            header[26] = 1
            return bytes(header) + bytes([len(payload)]) + payload
        identification = bytearray(16)
        identification[:7] = b"\x01vorbis"
        struct.pack_into("<I", identification, 12, 44100)
        stream = page(0, 2, identification) + page(4692672) + page(4630500, 4)
        self.assertEqual(festival.ogg_playable_samples(stream), (4630500, 44100))
        with self.assertRaisesRegex(ValueError, "EOS"):
            festival.ogg_playable_samples(stream[:-29])
        mecha = next(t for t in self.tracks if t["id"] == "stepmania-mecha-tribe-beginner")
        self.assertEqual(mecha["duration"], 105.0)
        self.assertEqual(mecha["playableSampleCount"], 4630500)
        self.assertGreater(mecha["historicalPreparedDurationSeconds"], mecha["duration"])

    def test_duration_audit_distinguishes_container_pcm_and_playable_bounds(self):
        audit = festival.read_json(festival.DATA / "duration-audit.json")
        self.assertEqual(len(audit["tracks"]), 10)
        for evidence in audit["tracks"]:
            self.assertEqual(evidence["audioSha256"], festival.sha(festival.ROOT / evidence["audioAssetPath"]))
            self.assertAlmostEqual(evidence["playableDurationSeconds"],
                                   evidence["playableSampleCount"] / evidence["playableSampleRate"], places=8)
            self.assertTrue(evidence["containerProbe"]["streams"])
        mecha = next(t for t in audit["tracks"] if t["trackId"] == "stepmania-mecha-tribe-beginner")
        self.assertEqual(mecha["playableDurationSeconds"], 105.0)
        self.assertEqual(mecha["authority"], "final-vorbis-eos-granule")
        self.assertNotEqual(mecha["resampledFfmpegDecodedDurationSeconds"], mecha["playableDurationSeconds"])


if __name__ == "__main__":
    unittest.main()
