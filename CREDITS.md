# Credits and provenance

## Existing third-party assets

| Asset | Recorded creator/source | Recorded license | Verification status |
|---|---|---|---|
| Balloon | Poly by Google, via [Poly Pizza](https://poly.pizza/m/d1gDDhM7pTf) | [CC-BY 3.0](https://creativecommons.org/licenses/by/3.0/) | October 17 page metadata identifies Balloon, Poly by Google, 1,048 triangles and CC-BY 3.0; title/creator/topology agree with the retained OBJ. Original download archive/hash is unavailable. |
| Blaster Kit | [Kenney](https://kenney.nl/assets/blaster-kit) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | Official pack page checked October 17; exact historical download record remains unavailable. |
| Impact Sounds | [Kenney](https://kenney.nl/assets/impact-sounds) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | Official pack page checked October 17; exact historical download record remains unavailable. |

The Balloon is adapted with project materials, coloring and presentation; the original creator does not endorse this game. This source identification is supported by page metadata, not an independent byte-identical redownload. Full CC-BY 3.0 and CC0 texts are retained in `Tools/Release/Notices`, downloaded from the official Creative Commons legal-code URLs. Do not describe all project assets as CC0. Preserve model/palette packages and third-party notices.

## Project-created content

Studio textures, sprays, menu images, latex fragments and paint concepts use built-in image generation. Exact recorded prompts are retained in the [art guides](Docs/Art/ASSET_CATALOG.md). Older concept/splat prompts are unavailable. Concept images do not establish licenses for imported models.

The October 11 Interface V2 background and transparent sprite sheet use built-in image generation; their exact prompts are recorded in [Interface V2](Docs/Art/INTERFACE_V2.md). UI layouts are editable Unity Canvas prefabs; the original LegacyRuntime font was subsequently replaced by the Lato clarity pass described below. Their preview PNGs are Unity camera renders, and displayed scores are illustrative samples.

RoomFinishV7 adds a generated oak-plank albedo texture, with its exact prompt in [Studio art pack](Docs/Art/STUDIO_ART_PACK.md). Cream wall and ivory ceiling materials are native Unity material settings. Floor UVs reuse the existing mesh shape; the refreshed reflection is a Unity bake.

ExhibitionV8 reuses those room materials and the BalloonPopV2 latex atlas. Its added shell/skirting uses Unity cubes, and paint droplets use a Unity sphere mesh with particle materials. No new AI-generated raster or external download was introduced in this pass; the wall/effect previews and refreshed reflection are Unity renders.

CoverFinishV9 adds mathematical mineral-plaster and leaf-vein textures, procedural rounded joinery, leaf geometry and a sideboard. Existing wood/cloth assets retain their provenance. Its offline lightmaps, reflection and previews are Unity renders; it adds no AI-generated raster or external download. The exact earlier prompts remain in the linked art guides. See [Cover finish V9](Docs/Art/COVER_FINISH_V9.md).

The courtyard is fictional and is not official Kyushu University or Qpic imagery. Frames, props, curved leaves, alcoves, refined gun/balloon surfaces, ropes and meshes use Unity primitives or procedural geometry. Native woven, normal and surface maps are mathematical patterns; fine oak retains its generated-image provenance. Room reflections and prototype previews are Unity camera captures.

ConceptVariantsV12 references the three original website concept images without editing them. Its gun, rounded beams and two additional balloon body silhouettes are native procedural geometry; the existing attributed pear/knot and prior weapon are retained. Wall paint textures derive from the existing GraffitiSpray atlas's stroke detail and alpha. Original artwork, attribution and exact recorded prompts are preserved. No new image generation or external download is introduced; new lightmaps/reflection/previews are Unity outputs. See [Concept variants V12](Docs/Art/CONCEPT_VARIANTS_V12.md).

V12's `StudioGunLit.shader` derives from Unity's installed URP 17.0.4 `Shaders/Lit.shader`, with a renamed shader and the forward-pass SSAO variant omitted for held-gun rendering. Its source SHA-256 and changes are recorded in `ConceptVariantsV12/Shaders/Provenance.txt`; the package's Unity Companion License is copied verbatim alongside it. This derivative retains Unity's license conditions and is not represented as CC0.

ConceptDetailV13 adds deterministic procedural mineral-floor texture data and native rounded/lathed art supplies, using Unity's sphere for small organic details. Saved canvas/floor marks reuse the V12 relief and original spray-alpha derivatives. Wood/woven surfaces and all earlier concepts retain their provenance and prompts. No new AI image or external download was introduced. Its lightmaps, reflection and preview images are Unity outputs. See [Concept studio detail V13](Docs/Art/CONCEPT_DETAIL_V13.md).

The five current WAVs in `Assets/Audio/Music` are original synthesized compositions produced with `Tools/Audio/compose_studio_music.py`. They use no third-party recordings, samples or songs. This statement applies to those files only and does not change any existing asset license.

`Assets/Audio/SFX/PaintShot_AirPuff.wav` is original mathematical synthesis from `Tools/Audio/synthesize_paint_shot.py`, using fixed seed 20261016. The accompanying JSON records format, measured levels and the output hash. It contains no external recordings, song samples or AI-generated audio. Existing music/model restrictions are unchanged.

## UI fonts

The October 14 interface clarity pass uses unmodified Lato Regular/Bold by Lukasz Dziedzic / tyPoland, distributed under SIL Open Font License 1.1. Font binaries, the complete OFL notice and pinned source URLs/SHA-256 hashes are in `Assets/StudioArtPack/InterfaceV2/Fonts`. Google Fonts revision: `5d3b76120a319730fda218cc7410174a462b32cb`. Source: [Google Fonts Lato](https://github.com/google/fonts/tree/5d3b76120a319730fda218cc7410174a462b32cb/ofl/lato). This license applies to the included font software; it does not alter song/model licenses. Lato fixes the current Latin-script UI; Japanese/Chinese font coverage remains separate localization work.

## Downloaded rhythm study references

[StepMania](https://github.com/stepmania/stepmania) supplies the public reference packs Goin' Under (NegaRen), Mecha-Tribe Assault (Kommisar), and Springtime (Kommisar). Upstream describes included songs as CC-NC, without a precise license version in its README. Chart-level credits remain in the original files.

At the user's request, the original collection is included in `Assets/StreamingAssets/Rhythm/StepMania` for noncommercial development packages. The upstream README, chart credits, source manifest and NOTICE.txt accompany the unmodified files. Do not sell packages containing these songs or claim commercial clearance; upstream does not specify a precise CC license version. The local reference directory remains a download cache. See [Music and charts](Docs/Design/MUSIC_AND_CHARTS.md) for provenance and packaging details.

The October 13 prepared gameplay track uses Goin' Under by NegaRen, dance-single Beginner chart by Fraxtil. `Assets/Audio/Music/StepMania/GoinUnder.ogg` is a byte-identical runtime copy of the pinned audio. `Assets/StreamingAssets/Rhythm/Prepared/GoinUnder_Beginner.json` is a derived event timeline: simultaneous tap lanes become one target event, with original lane metadata retained. It records the source revision, URLs, SHA-256 hashes and adaptation. No source audio or chart was edited; the same noncommercial restrictions and original notices apply. Asset preparation does not establish runtime integration.

## October 16 development captures

`Assets/Art/Previews/PrototypeScene_20261016.png` is an unedited 2560 × 1440 Unity camera render of the saved studio after fixed-room batching, without UI or balloons. `CreatorStats_20261016.png` is the creator-supplied Unity Game-view screenshot showing gameplay and Editor Stats. These are development evidence, not generated concept art or a standalone benchmark. All depicted assets retain their existing attribution and recorded prompts.

## October 17 surface-paint lesson assets

`Assets/StudioArtPack/SurfacePaint` reuses the existing GraffitiSpray_Atlas_v3 Teal_Spray slice without changing image pixels, import settings or the exact recorded prompt. The new material is a native copy of SprayUnlit, using the installed unmodified URP sprite-unlit shader. The small SpriteRenderer prefab and close-up preview are native Unity outputs; no new external download or generated image is introduced. Source paths/hashes and preparation settings are in its `Provenance.json`; the original prompt remains in [Spray assets](Docs/Art/SPRAY_ASSETS.md#recorded-generation-prompts). The preview is an asset placement render, not a recorded gameplay hit.

The three-color balloon lesson prefab is a native copy of the current production balloon with the existing cosmetic appearance component, one retained mesh and the already prepared teal/coral/gold latex materials. It adds no image, model, external download or new generated prompt. Original Poly by Google attribution and all prior provenance remain unchanged.

The October 17 best-score results row is native editable Unity UI using existing Lato Bold, colors and panel artwork. Results_BestScore_1080p.png is a Unity layout render with illustrative score values, not a recorded result. No new generated image, font, download or prompt was introduced; all original UI art prompts and Lato notices remain applicable.

The October 18 author credit reads **何单东 Shandong He**. Its Balloon Credit Sans Regular font is a renamed, approximately 19 KB subset of the official Noto Sans CJK SC Regular, retaining printable ASCII and the three name characters only. Original copyright and SIL Open Font License 1.1 are retained in `Assets/StudioArtPack/InterfaceV2/Fonts/AuthorCredit/OFL.txt`; pinned upstream commit/URLs, source/derived SHA-256 and fontTools preparation details are in its `Provenance.json`. The original download stays in ignored `.local-reference/Fonts/NotoCredit`. This narrow font does not establish general Chinese or Japanese UI coverage. The cream backing plate and author label are native Unity UI, with no new generated image or changed art prompt.
