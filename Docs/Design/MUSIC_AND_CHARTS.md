# Music and chart resources

Retrieved: October 11, 2026; status updated October 14. The creator authorized this collection for noncommercial development packages and selected downloaded music for gameplay. Studio music belongs to interfaces. Runtime switching and a complete-song DSP timer are saved; chart loading/scheduling and judgement remain pending.

## Downloaded collection

Source: [StepMania official repository](https://github.com/stepmania/stepmania/tree/825467bcd81c812b33ad684dc04dd151b2d5dec3/Songs/StepMania%205), pinned to revision `825467bcd81c812b33ad684dc04dd151b2d5dec3`.

| Track | Artist in chart | Formats | Header BPM | Header offset |
|---|---|---|---|---|
| Goin' Under | NegaRen | OGG, SM, SSC, artwork | 210 | 0 seconds |
| Mecha-Tribe Assault | Kommisar | OGG, SSC, artwork | 180 | -0.028 seconds |
| Springtime | Kommisar | MP3, SSC, artwork | 181.685 | -0.090 seconds |

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

## Required festival implementation

The creator selected one complete song per round, starting at audio time zero and settling at its end; no 90-second crop or game-song loop. Retain StudioDaylight_Loop.wav as interface music. Start with Goin' Under / dance-single Beginner: NegaRen (artist), Fraxtil (chart), constant 210 BPM, OFFSET 0, complete audio duration 123.428571 seconds. This chart uses taps only; the other available starter charts include holds/mines or chart-specific tempo changes.

Prepared assets:

| Asset | Purpose |
|---|---|
| Assets/Audio/Music/StepMania/GoinUnder.ogg | Byte-identical copy imported as an AudioClip outside StreamingAssets |
| Assets/Audio/Music/StepMania/GameMusic_GoinUnder.prefab | Assigned game AudioSource: non-looping, no autoplay, 2D, pitch 1 |
| Assets/StreamingAssets/Rhythm/Prepared/GoinUnder_Beginner.json | Complete 93-event timeline derived from 95 lane taps, including two merged chords; source links/hashes and credits |
| Assets/Audio/Music/StepMania/NOTICE.txt | Runtime-copy provenance and reference to the preserved original notices |

The first event is beat 32 / 9.142857 seconds; the last is beat 416 / 118.857143 seconds. Keep the introductory and ending passages. Timing data is relative to the unchanged complete audio. The JSON records sample count 5,443,200 and sample rate 44,100, source revision, original audio/chart paths, hashes, URLs, chart author and adaptation. Lanes are retained as provenance and are not balloon coordinates.

Reproduce and test preparation with Python 3 from the repository root:

```powershell
python Tools/Rhythm/prepare_exhibition_track.py
python -m unittest discover -s Tools/Rhythm -p 'test_*.py' -v
```

The tool validates the pinned source hashes and refuses changed prepared destinations. It handles only constant-tempo 4/4 dance-single Beginner tap rows and rejects unsupported tempo changes, stops, delays, warps, fake regions, holds, mines and malformed rows. It honors chart timing overrides. One same-time tap row becomes one balloon event, preserving original lanes. A general SSC/SM importer is not implemented.

For this subset, event time is `beat * 60 / BPM - source OFFSET`; the project first-beat offset has the opposite sign. [StepMania's timing implementation](https://github.com/stepmania/stepmania/blob/5_1-new/src/TimingData.cpp) initializes beat-zero elapsed time from the negative source offset. [Its SSC format notes](https://github.com/stepmania/stepmania/blob/5_1-new/Docs/SimfileFormats/ssc_msd5.txt) distinguish song and chart timing sections. No StepMania global user calibration is copied into project data.

Load prepared JSON at runtime from Application.streamingAssetsPath; [Unity copies StreamingAssets without standard asset import](https://docs.unity3d.com/6000.0/Documentation/Manual/StreamingAssets.html), so it is not a drag-in TextAsset. The separate audio copy is intentionally importable and can be assigned in the Inspector. Original audio/charts/notices remain unchanged in the bundle.

Begin with the [menu/game music lesson](RHYTHM_FIRST_LESSON.md), then use one scheduled DSP timeline for song time, target previews, accepted-shot event judgement, carried rewards and song-end settlement. The [design](GAME_DESIGN.md) defines behavior and the [roadmap](ROADMAP.md) defines gates. Runtime integration remains learner work; prepared files alone do not make a playable rhythm game.

## Windows development package

Unity copies the original directory to BalloonBarrage_Data/StreamingAssets/Rhythm/StepMania and the prepared timeline to StreamingAssets/Rhythm/Prepared. NOTICE.txt explains restrictions; source-manifest.json records original hashes. The imported AudioClip is wired through the saved scene. The controller switches interface/game music and uses scheduled DSP timing; the song menu and runtime chart targets/judgement remain pending.

See [the build guide](../Project/WEBSITE_AND_RELEASES.md) for the isolated Windows development build and package verification command.
