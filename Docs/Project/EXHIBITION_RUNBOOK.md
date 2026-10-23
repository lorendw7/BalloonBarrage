# Exhibition operator runbook

Use this runbook for the accepted Windows package at the Kyudaisai booth. It covers setup, player handoff and recovery; it does not establish gameplay acceptance. The [Roadmap](../Design/ROADMAP.md) owns dates and cuts, [Validation](VALIDATION.md) owns evidence, and [Packaging](PACKAGING_LESSON.md) owns release creation.

## Required kit

- Accepted primary ZIP, its `.sha256` file and a separately tested fallback ZIP.
- Freshly extracted primary and fallback folders on local storage; do not run from inside a ZIP.
- The intended Windows PC, display, mouse, audio output, power adapters and offline backup storage.
- Printed Japanese-first [control card](../../Tools/Exhibition/controls-ja.html).
- A copy of the anonymous [session log template](../../Tools/Exhibition/session-log-template.csv), or equivalent paper log, for failures, approximate song time and recovery action.

## Before opening

1. Disable sleep, disruptive notifications and automatic restarts for the exhibit period. Do not disable venue-required security controls.
2. Connect power, display, mouse and the actual audio output. Confirm the intended resolution and Windows scaling.
3. Verify the extracted package without modifying it:

   ```powershell
   powershell -ExecutionPolicy Bypass -File Tools/Exhibition/Test-ExhibitionPackage.ps1 `
     -BuildPath "D:\Exhibit\BalloonBarrage-primary" -ReleasePackage
   ```

4. Disconnect or block network access if the venue run is intended to be offline, then launch `BalloonShooter.exe`.
5. Complete Start, Japanese/English switching, held fire, one ordinary hit, one timed bonus, Esc pause/resume, natural results with trusted counters/streak, Retry and Quit. Confirm music remains audible and the custom spray feedback remains readable during held fire.
6. Keep the accepted primary and fallback folders unchanged after this check. Record which package/hash passed.

## Player handoff

Plan one complete turn for about three to four minutes: a short explanation, the 123.429-second song and a brief results/paint moment.

1. Reset to the title/results handoff state agreed during rehearsal.
2. Point to the Japanese control card. Explain mouse aim and held left-click first; describe the beat bonus as optional.
3. Let the player finish after misses. Do not restart merely for a low score.
4. At results, allow a short look at the painted room, then offer Retry or end the turn.
5. Between players, check pointer control, audio and whether the expected fresh-round state appears.

## Recovery order

| Symptom | First action | If it persists |
|---|---|---|
| Pointer or firing appears stuck | Release the mouse, press Esc, Resume, then click once only after gameplay is active | Quit normally and relaunch the same accepted folder |
| Audio is absent or routed incorrectly | Pause, check the selected Windows output and physical volume, then Resume | Relaunch; switch to the rehearsed fallback if the primary still fails |
| Frozen UI, missing target flow or unusable Retry | Wait briefly for a known transition, then attempt Esc/Quit | Close the player, record the approximate song time, relaunch; use fallback after a repeat |
| Visual corruption or severe sustained slowdown | End the current turn and relaunch the primary package | Use the tested fallback and record display settings plus observed paint density |
| Primary package will not launch | Recheck that the whole extracted directory is present and rerun preflight | Use the separately extracted, tested fallback; do not repair files at the booth |

Do not edit files, clear records, install updates or build a new player during the exhibition. A repeated failure moves the station to the rehearsed fallback until there is time for an offline diagnosis.

## End-of-day record

Record the package filename and hash, PC/display/audio setup, approximate number of turns, every relaunch or fallback switch, and concise anonymous observations about clarity, comfort and recovery. These observations are not automated results and should not include player names or contact information.
