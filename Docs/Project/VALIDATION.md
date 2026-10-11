# Validation record

Updated: October 11, 2026 (Asia/Tokyo). Historical detail remains in Git history; prior checks do not automatically validate new gameplay.

## Current checks

- Repository static audit passed: 459 unique Unity metadata GUIDs, no missing/orphaned metadata, and valid local links across README and Docs (15 files).
- Sixteen pinned StepMania reference files downloaded and passed SHA-256 verification. Downloads remain ignored and outside Assets; no external song was added to a release.
- The website online build generated three language pages from five canonical artworks and reported no eligible public Windows package at lookup time.
- All six website tests passed: translation keys, release selection, empty/failure states, local links, canonical artwork and preview HTTP routing. The first sandboxed run could not connect to localhost; rerunning with networking enabled passed.
- Documentation was consolidated into English, retaining fourteen recorded image-generation prompts, credits and unresolved issues. Gameplay changes are saved learner work; this cleanup did not author new gameplay.
- A snapshot of the saved gameplay sources compiled with Unity's bundled compiler and the project's generated assembly references. This is a source compilation check, not a Unity Test Runner run or Windows build.

## Gameplay checkpoint

RoundController is attached with a five-second test duration. Shooting, refill and scoring use its play state. Results/retry implementation is being saved by the learner; consult the committed source for the exact checkpoint. The learner previously reported the five-second stop test passed. Ten consecutive retries and the 90-second exhibition configuration still require acceptance.

## Prior evidence

The October 10 V6 record reports isolated Unity compilation/rendering and five PlayMode checks covering target refill/caps, held fire at five and two shots per second, refined assets/capture, and an actual projectile hit with one score award. Closeups and four-corner aiming were inspected. The room-reflection reload reference was fixed with an explicit Custom cubemap.

Those checks predate the round-control changes. No current Windows build, measured target-PC performance, or exhibition soak test is claimed.

## Outstanding gates

- The inherited strict scan reported four unresolved GUIDs in DefaultVolumeProfile.asset and seven in PC_Renderer.asset. Investigate in Unity; successful rendering does not clear the full reference audit. Do not delete settings blindly.
- Complete results/retry acceptance, including cleanup, explicit static score reset and ten consecutive rounds.
- Verify Japanese start/help, pause/quit, font coverage and menu click handling.
- Measure a 30-minute exhibition-PC run and extract/test a complete offline Windows package.
- Finish asset/license verification before distribution. Downloaded study songs are not approved release assets.
- Rhythm, waves, upgrades, runtime language switching and controller/co-op tests await implementation.

## Repeatable checks

Run from the repository root:

```powershell
powershell -ExecutionPolicy Bypass -File Tools/ProjectMaintenance/Verify-Project.ps1
powershell -ExecutionPolicy Bypass -File Tools/Rhythm/Download-References.ps1
node Website/scripts/build.mjs --offline
node --test Website/tests/site.test.mjs
```

Use Unity Test Runner for foundation EditMode tests. ProjectValidation.Run performs deeper scene/prefab/YAML checks in an isolated Unity project; it previously failed on the inherited references. Metadata and website tests do not replace runtime/build acceptance. Keep generated logs and verification copies outside the committed tree.
