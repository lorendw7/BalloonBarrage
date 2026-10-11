# Game design

Updated: October 11, 2026. Delivery order and acceptance live in the [roadmap](ROADMAP.md); source and download details live in [Music and charts](MUSIC_AND_CHARTS.md).

## Confirmed direction

BalloonBarrage combines free shooting, balloon crowds, beat bonuses, and roguelite variety in a warm graffiti studio. Players can fire at any time, including held fire. Accurate rhythm earns an additional score or damage benefit; it does not unlock permission to shoot. The learner implements gameplay.

Windows solo is the first platform and mode. A fixed camera and shared gallery avoid movement and split-screen complexity. Controllers and local two-player play follow a complete solo demo. Mobile is deferred; online multiplayer, independent dual mice, and mobile co-op are outside current scope.

## Exhibition slice

The minimum October showcase is a 90-second score challenge: Japanese start/instructions, mouse aiming and held fire, replenishing balloons, time-up, final score, and quick retry. Include pause, return to title, quit, and offline play. Reset score/time and clear projectiles/targets every round. The current scene duration is five seconds for testing.

This slice establishes a reliable loop. Rhythm judging and roguelite upgrades are subsequent milestones and must not be advertised as implemented.

## Rhythm proposal

Begin with the existing original 108 BPM track and score bonuses. Define one audio clock, track start, first-beat offset, judgement windows, and calibration offset. Judge the shot when fired; carry its result to impact so projectile travel distance does not change timing accuracy. Award the bonus only for a qualifying hit and prevent duplicate resolution.

Held-fire cadence can repeatedly miss the beat even when the player starts accurately. Compare manual taps with held fire during playtests before choosing bonus and combo rules. Keep baseline free shooting useful, provide visible timing feedback, and tune accessibility independently of damage.

Pause/resume and retry must restore music and judgement together. Measure latency on the exhibition PC. Use a constant-tempo beat grid first; importing foreign chart formats is a later, separately tested task.

## First rhythm/roguelite demo

Proposed scope: one track, three to five waves, at least three eligible upgrades, and a complete results/retry loop. Between waves, pause combat for one-of-three upgrade selection. Start with normal one-hit balloons; introduce armored, splitter, chain, and elite targets incrementally.

Candidate upgrades include cadence, spread, piercing, chains, area burst, freeze, critical hits, and shields. Define caps, eligible offers, ownership, and chain termination before adding combinations. Reset run upgrades on retry; initially persist only settings and best scores. A recorded random seed may aid debugging without promising cross-version determinism.

Escaped-target pressure and final-wave victory are proposals for later playtests. They are not rules of the timed exhibition challenge.

## Co-op and localization

Future local co-op uses keyboard/mouse plus one controller, or two controllers, with explicit per-player device pairing. Each player owns a reticle, weapon cadence, and upgrades. Use both shape and P1/P2 labels: coral ring and teal diamond. Shared score, no friendly fire, and no XP competition are the starting proposal.

Pause on device disconnect and offer reconnection or solo continuation. Both players finish upgrade selection before combat resumes; the pause initiator owns menu navigation. Menu interaction must not fire into the scene.

Game UI targets Chinese, Japanese, and English. Text data exists; runtime switching and font coverage are pending. Exhibition readability prioritizes Japanese. See [localization](../Art/LOCALIZATION.md).

## Performance approach

Target 1080p at 60 FPS on the exhibition PC, subject to measurement. Start with three targets, trial 8–12, then profile 30/60/100 only as the game requires them. These counts are experiments, not proven capacity.

Record CPU/GPU frame time, allocations, live object counts, audio voices, and particle/decal counts. Cap targets and feedback first; add pooling only when a measured bottleneck justifies it. Pooling must reset scale, collider state, pop flags, and ownership. Resolve simultaneous hits once and bound chains. Decorative strings and props should not add gameplay colliders or unnecessary realtime shadows.
