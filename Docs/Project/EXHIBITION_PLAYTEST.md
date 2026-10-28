# Short exhibition playtest

Use the current saved `Assets/Scenes/PrototypeScene.unity` for the latest implementation review. Public demo.6 is the current packaged fallback, while any feedback-driven shorter multi-song build is a separate candidate until its own acceptance passes. Record the exact source commit or package filename and hash; do not combine their results. [Roadmap](../Design/ROADMAP.md) owns dates and acceptance gates; [Game design](../Design/GAME_DESIGN.md) owns behavior.

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
- Round length: too short / comfortable / too long; record the selected song/profile and measured playable duration.
- Barrage variety: sweep, fan and cluster were recognizable / describe any pattern that felt repetitive or unreadable.
- Pop-effect alignment: center and edge hits stayed attached / describe the screen area and pattern where an offset appeared.
- Results / Retry / pause: passed / describe the failure.

These are user reports, not automated test results. Record them with the tested revision/package in [Validation](VALIDATION.md) after the creator responds. No successful report is assumed in advance.

## Feedback-driven candidate checks

Run these only after the corresponding learner increment is saved. Stop at the first failed dependency and keep demo.6 available.

0. Click Start, choose Goin' Under, Springtime or Mecha-Tribe Assault from the dropdown, hear its short preview, then confirm Play. Check Back restores the title without starting a round. Number-key song shortcuts are removed. During play, press C and confirm the gun/pigment color advances once per press without changing the balloon's assigned color. Confirm increased target visibility, readable row spacing and the 16-target bound.
1. Reproduce center and edge balloon hits from the center, left and right of the screen. The pigment puff, latex fragments and any gold accent must originate at the accepted contact and remain independent after the balloon disappears; one hit must not emit twice.
2. Measure the selected round from scheduled gameplay start to natural results. Audio, timer, included events and settlement must agree on 75–90 seconds. Pause for at least five seconds, resume, then Retry and confirm a fresh origin and clean transient effects.
3. Complete each selectable song twice. Confirm the displayed title/credit, audible track, prepared event set, barrage set, result identity and best record agree; switch songs, Retry and relaunch once to detect mixed state.
4. Observe the lateral sweep, shallow fan and staggered cluster during held fire and deliberate taps. Each should be recognizable without hiding the reticle, timing cue or next target. Record maximum visible targets and any unavoidable overlap rather than silently accepting skipped events.
5. During the densest pattern, confirm fragments and pigment variations stay bounded, the song/on-beat accent remains audible and no transient effect survives natural results, Retry or return to title.

If segmented playback permission or compatible second-song data is unresolved, do not improvise a crop at the booth. Test the unmodified whole-song candidate or use demo.6.

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
