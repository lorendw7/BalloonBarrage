import unittest
from prepare_exhibition_track import (
    audio_duration,
    build_profile_catalog,
    build_track,
    GOIN_UNDER,
    KYUDAISAI_PROFILES,
    mp3_duration,
    ROOT,
    SOURCE,
    SPRINGTIME,
    MECHA_TRIBE,
    TRACKS,
    tap_chart,
)


def fixture(offset="-0.028", extra="", notes="1000\n0000\n0000\n0000"):
    return (f"#TITLE:Fixture;#ARTIST:Test;#BPMS:0=180;#OFFSET:{offset};#STOPS:;"
            f"#NOTEDATA:;#STEPSTYPE:dance-single;#DIFFICULTY:Beginner;"
            f"#CREDIT:Author;{extra}#NOTES:{notes};")


class PreparedTrackTests(unittest.TestCase):
    def test_negative_offset_places_beat_zero_after_audio_start(self):
        chart = tap_chart(fixture())
        self.assertAlmostEqual(chart["events"][0]["timeSeconds"], .028)

    def test_subdivision_and_chord_share_one_event(self):
        chart = tap_chart(fixture("0", notes="0000\n1100\n0000\n0000\n0000\n0000\n0000\n0000"))
        self.assertEqual(len(chart["events"]), 1)
        self.assertEqual(chart["events"][0]["lanes"], [0, 1])
        self.assertAlmostEqual(chart["events"][0]["beat"], .5)
        self.assertAlmostEqual(chart["events"][0]["timeSeconds"], 1 / 6, places=8)

    def test_chart_timing_overrides_song_header(self):
        chart = tap_chart(fixture("0", extra="#BPMS:0=120;#OFFSET:-0.1;", notes="0000\n1000\n0000\n0000"))
        self.assertAlmostEqual(chart["events"][0]["timeSeconds"], .6)

    def test_chart_bpm_segments_are_integrated(self):
        chart = tap_chart(fixture(
            "0",
            extra="#BPMS:0=120,4=60;",
            notes="0000\n0000\n0000\n0000,\n0000\n1000\n0000\n0000",
        ))
        self.assertEqual(chart["bpmSegments"], [
            {"beat": 0.0, "bpm": 120.0},
            {"beat": 4.0, "bpm": 60.0},
        ])
        self.assertAlmostEqual(chart["events"][0]["timeSeconds"], 3.0)

    def test_unsupported_timing_is_rejected(self):
        for extra in ("#STOPS:4=1;", "#DELAYS:4=.1;", "#WARPS:4=2;", "#FAKES:4=2;"):
            with self.subTest(extra=extra), self.assertRaises(ValueError):
                tap_chart(fixture(extra=extra))

    def test_hold_mine_and_malformed_rows_are_rejected(self):
        for row in ("2000", "M000", "100", "10000"):
            with self.subTest(row=row), self.assertRaises(ValueError):
                tap_chart(fixture(notes=row))

    def test_mp3_duration_counts_layer_three_frames(self):
        header = bytes.fromhex("fffb9000")
        frame = header + bytes(417 - len(header))
        samples, rate = mp3_duration(frame * 3)
        self.assertEqual(samples, 3 * 1152)
        self.assertEqual(rate, 44100)

    def test_explicit_balloon_adaptation_keeps_heads_and_omits_tails_and_mines(self):
        chart = tap_chart(fixture("0", notes="1200\n030M\n0040\n0030"), adapt_sustains=True)
        self.assertEqual([event["lanes"] for event in chart["events"]], [[0, 1], [2]])

    def test_pinned_tracks_are_complete_and_within_audio(self):
        expectations = (
            (GOIN_UNDER, 93, 95, "Fraxtil"),
            (SPRINGTIME, 210, 211, "Kommisar"),
        )
        for spec, event_count, tap_count, author in expectations:
            with self.subTest(track=spec.track_id):
                chart = tap_chart((SOURCE / spec.source_chart).read_text(encoding="utf-8-sig"))
                samples, rate = audio_duration(
                    (SOURCE / spec.source_audio).read_bytes(), spec.audio_filename)
                self.assertEqual(len(chart["events"]), event_count)
                self.assertEqual(sum(len(event["lanes"]) for event in chart["events"]), tap_count)
                self.assertEqual(chart["chartAuthor"], author)
                self.assertGreater(samples / rate, chart["events"][-1]["timeSeconds"])
                self.assertEqual(
                    chart["events"],
                    sorted(chart["events"], key=lambda event: event["timeSeconds"]),
                )

    def test_candidate_profiles_are_short_contiguous_and_non_scoring(self):
        import json

        manifest = json.loads((ROOT / "Tools/Rhythm/references.json").read_text(
            encoding="utf-8-sig"))
        entries = {item["path"]: item for item in manifest["files"]}
        prepared = {}
        for spec in TRACKS:
            chart, _ = build_track(spec, manifest, entries)
            prepared[spec.track_id] = chart
        catalog = build_profile_catalog(prepared)
        self.assertEqual(len(catalog["profiles"]), len(KYUDAISAI_PROFILES))
        self.assertEqual(
            {pattern["patternId"] for pattern in catalog["patterns"]},
            {"lateral-sweep", "shallow-fan", "staggered-cluster"},
        )
        self.assertTrue(all(
            pattern["logicalTargetsPerEvent"] == 1
            for pattern in catalog["patterns"]
        ))
        for profile in catalog["profiles"]:
            with self.subTest(profile=profile["roundProfileId"]):
                self.assertGreaterEqual(profile["roundDurationSeconds"], 75)
                self.assertLessEqual(profile["roundDurationSeconds"], 90)
                self.assertEqual(
                    sum(section["eventCount"] for section in profile["patternSections"]),
                    profile["eventCount"],
                )
                self.assertEqual(
                    [section["patternId"] for section in profile["patternSections"]],
                    ["lateral-sweep", "shallow-fan", "staggered-cluster"],
                )


if __name__ == "__main__":
    unittest.main()
