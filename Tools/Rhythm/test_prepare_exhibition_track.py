import unittest
from prepare_exhibition_track import constant_chart, ogg_duration, SOURCE, AUDIO, CHART


def fixture(offset="-0.028", extra="", notes="1000\n0000\n0000\n0000"):
    return (f"#TITLE:Fixture;#ARTIST:Test;#BPMS:0=180;#OFFSET:{offset};#STOPS:;"
            f"#NOTEDATA:;#STEPSTYPE:dance-single;#DIFFICULTY:Beginner;"
            f"#CREDIT:Author;{extra}#NOTES:{notes};")


class PreparedTrackTests(unittest.TestCase):
    def test_negative_offset_places_beat_zero_after_audio_start(self):
        chart = constant_chart(fixture())
        self.assertAlmostEqual(chart["events"][0]["timeSeconds"], .028)

    def test_subdivision_and_chord_share_one_event(self):
        chart = constant_chart(fixture("0", notes="0000\n1100\n0000\n0000\n0000\n0000\n0000\n0000"))
        self.assertEqual(len(chart["events"]), 1)
        self.assertEqual(chart["events"][0]["lanes"], [0, 1])
        self.assertAlmostEqual(chart["events"][0]["beat"], .5)
        self.assertAlmostEqual(chart["events"][0]["timeSeconds"], 1 / 6, places=8)

    def test_chart_timing_overrides_song_header(self):
        chart = constant_chart(fixture("0", extra="#BPMS:0=120;#OFFSET:-0.1;", notes="0000\n1000\n0000\n0000"))
        self.assertAlmostEqual(chart["events"][0]["timeSeconds"], .6)

    def test_unsupported_timing_is_rejected(self):
        for extra in ("#BPMS:0=180,4=90;", "#STOPS:4=1;", "#DELAYS:4=.1;", "#WARPS:4=2;", "#FAKES:4=2;"):
            with self.subTest(extra=extra), self.assertRaises(ValueError):
                constant_chart(fixture(extra=extra))

    def test_hold_mine_and_malformed_rows_are_rejected(self):
        for row in ("2000", "M000", "100", "10000"):
            with self.subTest(row=row), self.assertRaises(ValueError):
                constant_chart(fixture(notes=row))

    def test_pinned_track_is_complete_and_within_audio(self):
        chart = constant_chart((SOURCE / CHART).read_text(encoding="utf-8-sig"))
        samples, rate = ogg_duration((SOURCE / AUDIO).read_bytes())
        self.assertEqual(len(chart["events"]), 93)
        self.assertEqual(sum(len(event["lanes"]) for event in chart["events"]), 95)
        self.assertEqual(chart["chartAuthor"], "Fraxtil")
        self.assertGreater(samples / rate, chart["events"][-1]["timeSeconds"])
        self.assertEqual(chart["events"], sorted(chart["events"], key=lambda event: event["timeSeconds"]))


if __name__ == "__main__":
    unittest.main()
