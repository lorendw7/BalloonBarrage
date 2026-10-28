# Music and chart resources

Retrieved: October 11, 2026; status updated October 29. The creator authorized the pinned StepMania collection for noncommercial development packages. Interface music stays separate from gameplay. The new festival revision derives twenty profiles from ten preserved recordings, with bounded charts and an opening listen-then-answer motif. Original files, source manifests and notices remain intact. Runtime, listening and exact-package evidence live in [Validation](../Project/VALIDATION.md).

## October 29 expansion

The [open music/studio update](../Art/OPEN_MUSIC_STUDIO_UPDATE_20261029.md) adds four MintoDog CC0 tracks and three ParagonX9 CC BY-SA 3.0 classics through the existing song selector, for ten choices. New charts are project-authored practice grids with unverified listening calibration. Classics play fully and retain original MP3s, attribution and BY-SA licensing. Existing StepMania files and noncommercial restrictions remain preserved. The update guide owns the new track table, provenance and reproduction commands; [Validation](../Project/VALIDATION.md) owns completed runtime/package evidence.

## Festival and complete-song phrase catalog

`Assets/StreamingAssets/Rhythm/Festival20261029/catalog.json` lists ten festival profiles and ten standard profiles. Festival windows target 60–75 seconds and currently cluster near 65 seconds; standard profiles use the full playable recording. Each has a distinct `*-phrase-v2` identity and separate best-score key. Existing v1 data remains intact. [Game design](GAME_DESIGN.md#music-led-festival-variety) defines the experience; the [roadmap](ROADMAP.md) owns delivery.

| Song | Festival seconds | Complete-recording seconds |
|---|---|---|
| Goin' Under | 65.143 | 123.429 |
| Springtime | 64.727 | 129.045 |
| Mecha-Tribe Assault | 65.333 | 105.000 |
| Cozy Puzzle In-Game 1 | 65.085 | 130.170 |
| Cozy Puzzle In-Game 2 | 64.000 | 128.000 |
| Magic Puzzle In-Game 1 | 65.455 | 122.182 |
| Magic Puzzle In-Game 2 | 64.000 | 120.000 |
| Chaoz Fantasy | 65.046 | 133.904 |
| Chaoz Airflow | 64.457 | 253.780 |
| Chaoz Impact | 64.457 | 115.540 |

`duration-audit.json` records the complete-recording duration authority. Mecha-Tribe Assault has a lower final OGG EOS granule than an earlier page; use the final 4,630,500 samples at 44,100 Hz (105 seconds), rather than the historical maximum-granule value of 106.410 seconds. Its decoded PCM count also disagrees and is diagnostic only. Original bytes and historical source metadata remain preserved.

Derived charts use schema version 2:

- Event `timeSeconds` is **source-audio time**; cue-in is subtracted once by the reader. Event ids are unique within that derived chart. `authoredPlacement` and `placementSlot` identify spatial placement; section direction is applied once at runtime.
- `sections` use **round-relative** start/end times, numeric pattern 0/1/2 and direction ±1. Source tempo maps drive StepMania timing. Other tracks use the documented BPM plus signal-estimated phase.
- `demonstrations` use round-relative start, step duration, answer start and three slots. Only the three actual answer events score. Their preview waits until the noninteractive outline demonstration ends; musical judgement times remain fixed.
- Selected StepMania event timestamps remain exact and retain original event ids as provenance; crowded source rows are thinned. Three explicitly project-authored answer notes are additions to the derived chart, not claims about the original chart.
- New non-StepMania charts use recurring beat motifs and bounded spacing rather than the prior uniform practice grids. These are project-authored arrangements; eight-bar boundaries are inferred from timing and energy, not human-confirmed song form.

`signal-analysis.json` pins the analyzed audio hashes, decoded duration, fixed BPM, estimated phase, per-third disagreement and onset envelope. Its FFT method estimates timing/activity; it cannot establish human musical judgement, perceptual latency or physical aiming comfort. Chaoz Impact's BPM remains provisional. Human listening calibration remains explicitly false for every derived chart until separately observed.

Reproduce the committed data from its analysis snapshot with Python's standard library:

```powershell
python Tools/Rhythm/prepare_festival_catalog.py
python -m unittest discover -s Tools/Rhythm -p 'test_*.py' -v
```

Refreshing signal analysis additionally requires FFmpeg and NumPy:

```powershell
python Tools/Rhythm/prepare_festival_catalog.py --analyze
```

The explicit scene integration command is `FestivalExperienceInstaller.Apply` in a separate Unity project. It verifies matching audio/chart paths, twenty unique identities, ten profiles per mode and valid clip windows before saving. `FestivalRuntimeValidation.RunFast` labels controlled-clock traversal as accelerated. `RunFull` follows natural DSP time and supports duration-balanced shards in separate projects; see Validation for completed runs and exact evidence.

Derived classics charts and synchronized-sequence contributions retain CC BY-SA 3.0 attribution, license text and modification statements. MintoDog tracks remain CC0. StepMania source restrictions remain unchanged; the complete mixed package remains noncommercial. The new directory includes its own NOTICE and BY-SA text and links preserved source records. No new download, altered source audio or commercial-clearance claim is implied.

## Downloaded collection

Source: [StepMania official repository](https://github.com/stepmania/stepmania/tree/825467bcd81c812b33ad684dc04dd151b2d5dec3/Songs/StepMania%205), pinned to revision `825467bcd81c812b33ad684dc04dd151b2d5dec3`.

| Track | Artist in chart | Formats | Header BPM | Header offset | Source audio duration |
|---|---|---|---|---|---|
| Goin' Under | NegaRen | OGG, SM, SSC, artwork | 210 | 0 seconds | 123.428571 seconds |
| Mecha-Tribe Assault | Kommisar | OGG, SSC, artwork | 180 | -0.028 seconds | 105.000000 seconds |
| Springtime | Kommisar | MP3, SSC, artwork | 181.685 | -0.090 seconds | 129.044900 seconds |

These values are song-level headers, not proof of constant timing throughout every chart. SSC can contain chart-specific timing. Preserve full timing data and original chart credits when inspecting it.

Sixteen original files, including upstream README and song instructions, are now committed under `Assets/StreamingAssets/Rhythm/StepMania`. A download cache remains at `.local-reference/Rhythm/StepMania`. The [manifest](../../Tools/Rhythm/references.json) records exact URLs, byte lengths and SHA-256 hashes. Keep original folder relationships so music and artwork references remain resolvable.

To reproduce or verify the local collection from the repository root:

```powershell
powershell -ExecutionPolicy Bypass -File Tools/Rhythm/Download-References.ps1
```

The downloader verifies existing files, rejects altered copies, validates new downloads before moving them into place, and does not overwrite changed references. It needs network access for missing files. The downloader only updates the ignored cache. The approved StreamingAssets copy is included verbatim in Windows builds; verify it with `Tools/Rhythm/Verify-Bundle.ps1`. Unity metadata is not part of the original source hashes.

## Usage boundary and other sources

The [upstream licensing statement](https://github.com/stepmania/stepmania/tree/825467bcd81c812b33ad684dc04dd151b2d5dec3#licensing-terms) distinguishes engine code from included songs and describes songs as CC-NC. Its README does not identify a precise license version. Retain the upstream notices and original chart credits in this noncommercial development bundle. Do not sell packages containing these songs. A specific license version and commercial rights are not established; obtain separate permission before commercial or monetized use.

The [osu! Featured Artists catalog](https://osu.ppy.sh/beatmaps/artists) is another discovery source, with pre-timed downloads licensed for osu! and related content. That statement does not establish reuse rights in BalloonBarrage. No osu! downloads are included in this collection.

Public availability is separate from a distribution license. For a release candidate, record track/composer, chart author, source URL, license text/version, attribution requirements, permission to edit/distribute, audio format, timing metadata and hash. Resolve each component before adding it to `Assets`.

## Saved festival baseline and revised candidate

The saved demo.6 baseline plays one complete song per round, starting at audio time zero and settling at its end; it does not loop or crop gameplay audio. Retain StudioDaylight_Loop.wav as interface music. The connected track is Goin' Under / dance-single Beginner: NegaRen (artist), Fraxtil (chart), constant 210 BPM, OFFSET 0, complete audio duration 123.428571 seconds. This chart uses taps only; other charts in the collection must be checked for compatible step type, timing, holds and mines before preparation.

The preserved v1 candidate data contains three earlier 80–85-second profiles; the active v2 catalog above replaces those windows in the production scene. Springtime / Beginner has 210 tap-event rows and chart-specific BPM segments at beats 304 and 311; its short window ends at beat 272. Mecha-Tribe Assault / Beginner uses an explicit balloon adaptation: taps and hold/roll heads create one target per row, while tails and mines are omitted. Its complete derived chart has 158 events; the 80-second profile includes 137 events between beats 16 and 256. All source charts/audio remain unchanged.

Do not replace or modify the original files under `Assets/StreamingAssets/Rhythm/StepMania`. If a shorter exhibition window is cleared for use, store cue-in/cue-out at musical phrase boundaries and the included event range as derived round-profile metadata. Playback speed and pitch remain 1. Confirm permission for the selected noncommercial exhibition use and any segmented playback before packaging; otherwise use an unmodified whole track or demo.6. Each prepared choice requires a stable song/chart/round-profile identity so records cannot cross between different windows.

Prepared assets:

| Asset | Purpose |
|---|---|
| Assets/Audio/Music/StepMania/GoinUnder.ogg | Byte-identical copy imported as an AudioClip outside StreamingAssets |
| Assets/Audio/Music/StepMania/GameMusic_GoinUnder.prefab | Assigned game AudioSource: non-looping, no autoplay, 2D, pitch 1 |
| Assets/StreamingAssets/Rhythm/Prepared/GoinUnder_Beginner.json | Complete 93-event timeline derived from 95 lane taps, including two merged chords; source links/hashes and credits |
| Assets/Audio/Music/StepMania/Springtime.mp3 | Byte-identical copy imported as an AudioClip outside StreamingAssets |
| Assets/Audio/Music/StepMania/GameMusic_Springtime.prefab | Reusable non-looping, no-autoplay, 2D, pitch-1 AudioSource; PrototypeScene references the same imported clip through its selected profile |
| Assets/StreamingAssets/Rhythm/Prepared/Springtime_Beginner.json | Complete 210-event timeline derived from 211 lane taps, chart-specific BPM segments, source links/hashes and credits |
| Assets/Audio/Music/StepMania/MechaTribeAssault.ogg | Byte-identical pinned OGG; Kommisar, chart by Wyde |
| Assets/StreamingAssets/Rhythm/Prepared/MechaTribeAssault_Beginner.json | Complete 158-event head-only adaptation with pinned source URLs, hashes, restrictions and credits |
| Assets/StreamingAssets/Rhythm/Prepared/Kyudaisai_Candidate.json | Three 80–85-second profiles plus sweep/fan/cluster placement metadata; generated catalog can be regenerated by the preparation tool |
| Assets/Audio/Music/StepMania/NOTICE.txt | Runtime-copy provenance and reference to the preserved original notices |

The first event is beat 32 / 9.142857 seconds; the last is beat 416 / 118.857143 seconds. Keep the introductory and ending passages. Timing data is relative to the unchanged complete audio. The JSON records sample count 5,443,200 and sample rate 44,100, source revision, original audio/chart paths, hashes, URLs, chart author and adaptation. Lanes are retained as provenance and are not balloon coordinates.

Reproduce and test preparation with Python 3 from the repository root:

```powershell
python Tools/Rhythm/prepare_exhibition_track.py
python -m unittest discover -s Tools/Rhythm -p 'test_*.py' -v
```

The tool validates pinned source hashes and refuses changed prepared audio/chart destinations; the generated profile catalog is refreshed. It handles 4/4 dance-single Beginner rows and ascending BPM segments with chart-level timing overrides. Default parsing rejects holds and mines; only Mecha-Tribe Assault opts into head-only adaptation. Stops, delays, warps, fake regions, malformed rows and unsupported formats remain rejected. Audio durations are parsed without changing source bytes. Same-time lanes share one balloon event. A general SSC/SM importer is not implemented.

For this subset, event time is `beat * 60 / BPM - source OFFSET`; the project first-beat offset has the opposite sign. [StepMania's timing implementation](https://github.com/stepmania/stepmania/blob/5_1-new/src/TimingData.cpp) initializes beat-zero elapsed time from the negative source offset. [Its SSC format notes](https://github.com/stepmania/stepmania/blob/5_1-new/Docs/SimfileFormats/ssc_msd5.txt) distinguish song and chart timing sections. No StepMania global user calibration is copied into project data.

Load prepared JSON at runtime from Application.streamingAssetsPath; [Unity copies StreamingAssets without standard asset import](https://docs.unity3d.com/6000.0/Documentation/Manual/StreamingAssets.html), so it is not a drag-in TextAsset. The separate audio copy is intentionally importable and can be assigned in the Inspector. Original audio/charts/notices remain unchanged in the bundle.

Use one scheduled DSP timeline for song time, target previews, accepted-shot event judgement, carried rewards and the selected round-profile settlement. The source candidate implements this profile clock and pauses it by subtracting accumulated paused DSP duration. The [design](GAME_DESIGN.md) defines behavior and the [roadmap](ROADMAP.md) defines gates. Static wiring alone does not establish runtime or exact-package acceptance for any profile.

## Windows development package

Unity copies the original directory and prepared charts into StreamingAssets. NOTICE.txt and the source manifest preserve restrictions and provenance. PrototypeScene serializes twenty profiles sharing ten imported clips. Start opens mode selection (Festival/Standard) and the matching ten-song list. Changing selection previews the corresponding audio and loads its derived chart; Play confirms the round and Back restores the title. Number-key selection and separate title-screen song buttons are removed. Clip, chart window, duration and record identity change together. [Validation](../Project/VALIDATION.md) records the exact build, package, startup and natural-Editor evidence; physical and real-Mac acceptance remain separate.

See [the build guide](../Project/WEBSITE_AND_RELEASES.md) for the isolated Windows development build and package verification command.
