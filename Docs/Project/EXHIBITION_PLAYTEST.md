# Short exhibition playtest

Use the current saved `Assets/Scenes/PrototypeScene.unity` for the latest implementation review. The public demo.3 Windows/macOS files are prerelease candidates, not target-hardware-accepted festival builds. Record the exact source commit or package filename and hash; do not combine their results. [Roadmap](../Design/ROADMAP.md) owns dates and acceptance gates; [Game design](../Design/GAME_DESIGN.md) owns behavior.

## Creator session: about five minutes

1. Start one complete song. Spend the opening section aiming freely and holding fire; later try a few deliberate timed taps. Continue after misses.
2. Listen during continuous fire: can you still hear the song and recognize the on-beat accent? Note anything sharp, repetitive or tiring.
3. Shoot the same wall area repeatedly. Confirm each new direct-hit mark appears above earlier marks and above background graffiti, including once after Retry. Near the end, check whether balloons, reticle and timing cues remain clear against accumulated paint.
4. Watch the approach fill across several events and the ending state. Let the song finish. Check final score, best score, Retry, one pause/resume and return of the pointer. Report any stuck input or paint surviving into the next round.

Do not change several Inspector values during this first observation. Describe the strongest issue so the next adjustment can be compared with the same song and device. This short session does not replace the longer exhibition-PC rehearsal.

## Reply template

- Tested: current Unity source plus commit / exact package filename plus SHA-256; date, PC/display and audio device.
- Fun/comfort: 1–5; pressure: 1–5.
- Music during held fire: clear / partly masked / hard to hear.
- Beat bonus: recognizable / unclear.
- Late-song targets: easy to see / partly obscured.
- Paint overwrite: later marks above earlier marks and graffiti / describe the failure.
- Approach fill: clear and synchronized / unclear or stale after pause/Retry.
- Most noticeable issue: one sentence, with approximate song time if possible.
- Results / Retry / pause: passed / describe the failure.

These are user reports, not automated test results. Record them with the tested revision/package in [Validation](VALIDATION.md) after the creator responds. No successful report is assumed in advance.

## Focused checks after the committed additions

Run these only after the corresponding increment is saved; do not claim them from the earlier baseline report.

1. Make five accepted balloon hits, including two rewarded on-beat hits. Results must show five successful hits, two on-beat hits and 70 points.
2. Let one unresolved target reach its accepted expiry between hits. Current streak must reset while maximum streak remains; wall shots, empty shots and pause must not reset it.
3. Retry. Current-round successful hits, on-beat hits and streak values must return to zero while the saved best score remains.
4. Select Japanese, navigate Start/help/gameplay/pause/results, then select English and repeat. All static and dynamic labels must refresh, the selected language must survive Retry and relaunch, and no Japanese glyph may appear as a missing box.
5. During held fire and late-song paint density, confirm the custom muzzle/trail/impact/streak additions do not hide balloons, reticle or cue, mask the music, change projectile collision, or create persistent transient objects.

## Hardware details before rehearsal

Confirm the intended exhibition computer, display resolution, mouse and audio output. Confirm booth location/hours, venue sound rules and an operator before the final rehearsal. These details can arrive later and do not block presentation preparation.

Use the [exhibition operator runbook](EXHIBITION_RUNBOOK.md) for package preflight, offline setup, player handoff and recovery rehearsal. Keep gameplay observations here and operational incidents in the anonymous session log; do not merge either with automated evidence.

The Japanese-first [control card](../../Tools/Exhibition/controls-ja.html) is standalone HTML, with no external fonts, scripts or network requests. Open in a browser and print at actual size. It uses installed Japanese fonts; verify Japanese glyphs and the printed size on the exhibition machine before use. It is player-facing exhibition copy, not a replacement for saved in-game UI.
