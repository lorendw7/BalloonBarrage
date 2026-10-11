# Music and chart resources

Retrieved: October 11, 2026. These are public song/chart references for local study. They are not integrated gameplay content.

## Downloaded collection

Source: [StepMania official repository](https://github.com/stepmania/stepmania/tree/825467bcd81c812b33ad684dc04dd151b2d5dec3/Songs/StepMania%205), pinned to revision `825467bcd81c812b33ad684dc04dd151b2d5dec3`.

| Track | Artist in chart | Formats | Header BPM | Header offset |
|---|---|---|---|---|
| Goin' Under | NegaRen | OGG, SM, SSC, artwork | 210 | 0 seconds |
| Mecha-Tribe Assault | Kommisar | OGG, SSC, artwork | 180 | -0.028 seconds |
| Springtime | Kommisar | MP3, SSC, artwork | 181.685 | -0.090 seconds |

These values are song-level headers, not proof of constant timing throughout every chart. SSC can contain chart-specific timing. Preserve full timing data and original chart credits when inspecting it.

Sixteen files, including upstream README and song instructions, are stored at `.local-reference/Rhythm/StepMania`. The [manifest](../../Tools/Rhythm/references.json) records exact URLs, byte lengths and SHA-256 hashes. Keep original folder relationships so music and artwork references remain resolvable.

To reproduce or verify the local collection from the repository root:

```powershell
powershell -ExecutionPolicy Bypass -File Tools/Rhythm/Download-References.ps1
```

The downloader verifies existing files, rejects altered copies, validates new downloads before moving them into place, and does not overwrite changed references. It needs network access for missing files. Study downloads are ignored by Git and remain outside `Assets` so Unity does not import them or include them in a build.

## Usage boundary and other sources

The [upstream licensing statement](https://github.com/stepmania/stepmania/tree/825467bcd81c812b33ad684dc04dd151b2d5dec3#licensing-terms) distinguishes engine code from included songs and describes songs as CC-NC. Its README does not identify a precise license version. Treat the collection as local reference material; obtain clear music, chart and artwork permissions before shipping any part in this project.

The [osu! Featured Artists catalog](https://osu.ppy.sh/beatmaps/artists) is another discovery source, with pre-timed downloads licensed for osu! and related content. That statement does not establish reuse rights in BalloonBarrage. No osu! downloads are included in this collection.

Public availability is separate from a distribution license. For a release candidate, record track/composer, chart author, source URL, license text/version, attribution requirements, permission to edit/distribute, audio format, timing metadata and hash. Resolve each component before adding it to `Assets`.

## Recommended first implementation

Use the project's existing `StudioDaylight_Loop.wav`: original synthesis, 108 BPM, 32 bars, approximately 71.111 seconds. Reuse the existing file rather than importing a duplicate. A simple beat grid avoids an external chart importer for the first demo. Verify first-beat alignment by listening and measurement; metadata BPM alone does not confirm playable synchronization.

Proposed project metadata: stable track ID, audio asset reference, BPM, first-beat offset in seconds, loop length, calibration offset, and optional beat-indexed intensity markers. This is a design proposal, not an implemented file format.

1. Learn phrase lengths and note-density changes from the local references.
2. Define explicit project offset semantics and a single audio timeline.
3. Display a beat cue and compare manual taps/held fire.
4. Judge at firing, carry the result to impact, and grant a single hit bonus.
5. Test pause/resume, loop boundaries, calibration and retry before adding waves/upgrades.

If an importer is later justified, begin with one format and fixtures for BPM changes, stops, negative offsets, subdivisions, chart-specific timing and malformed input. Convert into a neutral beat/time representation; lane notes do not automatically define balloon placement or player aim. Reject unsupported timing explicitly rather than silently shifting events. See the [roadmap](ROADMAP.md) for sequencing.
