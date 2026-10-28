# Music and chart resources

Retrieved: October 11, 2026; status updated October 28. The creator authorized this collection for noncommercial development packages and selected downloaded music for gameplay. Studio music belongs to interfaces. Public demo.6 retains complete-song Goin' Under / Beginner. The current source candidate wires short Goin' Under and Springtime profiles, pre-start selection, cue seeking, event filtering and record isolation; three pattern/effect decisions and candidate play/package acceptance remain open.

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

The revised Kyudaisai candidate supports three song choices and 75–90-second rounds. Springtime / Beginner has 210 tap-event rows and chart-specific BPM segments at beats 304 and 311; its short window ends at beat 272. Mecha-Tribe Assault / Beginner uses an explicit balloon adaptation: taps and hold/roll heads create one target per row, while tails and mines are omitted. Its complete derived chart has 158 events; the 80-second profile includes 137 events between beats 16 and 256. All source charts/audio remain unchanged.

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

Use one scheduled DSP timeline for song time, target previews, accepted-shot event judgement, carried rewards and the selected round-profile settlement. The source candidate implements this profile clock and pauses it by subtracting accumulated paused DSP duration. The [design](GAME_DESIGN.md) defines behavior and the [roadmap](ROADMAP.md) defines gates. Static wiring alone does not make either candidate profile accepted or distributable.

## Windows development package

Unity copies the original directory and prepared charts into StreamingAssets. NOTICE.txt and the source manifest preserve restrictions and provenance. PrototypeScene serializes three imported clips/profiles. Start opens a dropdown, changing its selection previews the corresponding audio and loads its chart; Play confirms the round and Back restores the title. Number-key selection and separate title-screen song buttons are removed. Clip, chart window, duration and record identity change together. No current candidate player package has been accepted.

See [the build guide](../Project/WEBSITE_AND_RELEASES.md) for the isolated Windows development build and package verification command.
