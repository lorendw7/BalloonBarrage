# Credits and provenance

## October 29 demo.8 website captures

`Assets/Art/Previews/Festival20261029` preserves three native 3840 × 2160 Unity Editor Play Mode captures: mode/song selection, hollow demonstration markers and real response balloons. Files are copied byte-for-byte from the automated natural-DSP verification fixture for demo.8 source commit `2c3df2d494eb201939ebc69b797bed23b87aa464`. They are development-source captures, not standalone-player recordings or human-playtest evidence. [Provenance.json](Assets/Art/Previews/Festival20261029/Provenance.json) records exact hashes, fixture files, dimensions and context.

The website uses quality-98 WebP derivatives and a 1200 × 675 share JPEG. Derivative source/output hashes and settings are in `Website/media/provenance.json`. Existing room/model/font/music attribution and exact recorded artwork prompts remain unchanged. No generative editing or new image generation occurred.

## October 30 all-song showcase v4

The twenty separately versioned `*-showcase-v4.json` derivatives in `Assets/StreamingAssets/Rhythm/Festival20261029` extend mirror/trajectory/finale metadata to all ten recordings in both lengths. They select existing note IDs without changing their counts/timestamps or any audio bytes. Irregular responses are matched by ID. Historical v2/v3 derivatives remain intact; original author attribution, source hashes, license and noncommercial restrictions still apply. The derived-folder NOTICE records the new modification. No new generated images or prompts were added.

## October 30 Goin' Under showcase adaptation

The separately versioned `stepmania-goin-under-beginner-festival-showcase-v3.json` in `Assets/StreamingAssets/Rhythm/Festival20261029` adds a middle-song mirrored answer, one bounded arc/wave phrase and a three-target finale to the preserved Goin' Under Festival derivative. It reuses its 45 event IDs/timestamps and original audio, preserving the earlier v2 file. Its chart provenance and derived-folder NOTICE record this modification; original source files, licenses, noncommercial restrictions and attribution remain unchanged. The finale reuses existing credited pigment effects. No new generated artwork or prompts were introduced.

## October 29 festival phrase adaptations

`Assets/StreamingAssets/Rhythm/Festival20261029` contains project-authored short/full profiles, section/placement arrangements and demonstration-answer notes derived from the preserved ten-song library. Each chart records original source links/hashes, the selected source chart, modifications and license. Selected original StepMania event times are retained; thinned rows and three new answer events are identified as adaptations. No audio file or original chart was changed.

MintoDog's four recordings remain CC0. ParagonX9's three classics and their derived charts/synchronized-sequence contributions retain CC BY-SA 3.0, attribution, modification notices and included license text. The three original StepMania songs retain their noncommercial development restrictions, so the mixed package remains noncommercial. The directory's NOTICE and [Music and charts](Docs/Design/MUSIC_AND_CHARTS.md#festival-and-complete-song-phrase-catalog) link the exact provenance and preparation. Signal analysis estimates phase and activity; human calibration is unverified. No new image generation occurred, and the long-note paint ribbon is not implemented.

## October 28 official development captures

`Assets/Art/Previews/Official20261028` preserves 3840 x 2160 native Unity Play Mode PNGs for the title, deep-teal song selection and current gameplay. The gameplay frame uses natural Springtime chart events before stress-test targets are inserted. Captures come from the current working source in an isolated verification project, use 4x MSAA and contain no generative edits. UI canvases are rendered to the capture camera; these are automated development captures, not screenshots of the published demo.6 player or evidence of a physical playtest. Existing artwork, model, font and music attribution remains applicable. Website WebP/share derivatives retain exact source/output hashes and preparation settings in `Website/media/provenance.json`.

## October 28 studio expansion

`Assets/StudioArtPack/StudioExpansion20261028/Textures` contains three new opaque images made with OpenAI built-in image generation: `StudioRhythmPrint.png`, `BotanicalPigmentPrint.png`, and `CourtyardAutumnView.png`. The PNGs are copied byte-for-byte from the generated outputs. Their exact prompts, output identifiers, dimensions and SHA-256 hashes are retained in [Textures/Provenance.json](Assets/StudioArtPack/StudioExpansion20261028/Textures/Provenance.json). The two prints are text-free decorative art; the courtyard is fictional and is not official campus imagery. These are generated artwork, not gameplay captures.

The companion modular props use project-authored native Unity geometry and materials. Their inspection renders retain the attribution of all existing studio assets visible around them. Integration, regeneration and evidence limits are recorded in [Studio expansion](Docs/Art/STUDIO_EXPANSION_20261028.md). Website derivatives retain source/output hashes in `Website/media/provenance.json`. This addition does not change existing third-party model, font or noncommercial song restrictions.

## October 19 concept-style production gun

`Assets/StudioArtPack/PaintBlaster/ConceptRefinement20261019` contains native procedural Unity meshes/materials based on the existing `PaintBlaster_Concept_v1.png`. No new raster generation, prompt or external download was introduced. The original exact prompt remains in [Paint blaster](Docs/Art/PAINT_BLASTER.md#recorded-generation-prompts). Opaque materials reuse the existing V12 held-gun shader with its retained Unity Companion License and provenance; transparent glass derives from the existing reservoir material. The original Kenney model and its attribution remain intact. New previews are actual Unity inspection renders, not recorded playtests. Source/output hashes are in the new asset folder's `Provenance.json`; integration and limits are in [Concept-style gun refinement](Docs/Art/CONCEPT_GUN_REFINEMENT.md).

## Existing third-party assets

| Asset | Recorded creator/source | Recorded license | Verification status |
|---|---|---|---|
| Balloon | Poly by Google, via [Poly Pizza](https://poly.pizza/m/d1gDDhM7pTf) | [CC-BY 3.0](https://creativecommons.org/licenses/by/3.0/) | October 17 page metadata identifies Balloon, Poly by Google, 1,048 triangles and CC-BY 3.0; title/creator/topology agree with the retained OBJ. Original download archive/hash is unavailable. |
| Blaster Kit | [Kenney](https://kenney.nl/assets/blaster-kit) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | Official pack page checked October 17; exact historical download record remains unavailable. |
| Impact Sounds | [Kenney](https://kenney.nl/assets/impact-sounds) | [CC0 1.0](https://creativecommons.org/publicdomain/zero/1.0/) | Official pack page checked October 17; exact historical download record remains unavailable. |

The Balloon is adapted with project materials, coloring and presentation; the original creator does not endorse this game. This source identification is supported by page metadata, not an independent byte-identical redownload. Full CC-BY 3.0 and CC0 texts are retained in `Tools/Release/Notices`, downloaded from the official Creative Commons legal-code URLs. Do not describe all project assets as CC0. Preserve model/palette packages and third-party notices.

## Project-created content

Studio textures, sprays, menu images, latex fragments and paint concepts use built-in image generation. Exact recorded prompts are retained in the [art guides](Docs/Art/ASSET_CATALOG.md). Older concept/splat prompts are unavailable. Concept images do not establish licenses for imported models.

The October 25 tintable paint-VFX candidate in `Assets/StudioArtPack/PaintSprayStyle20261025` was created with OpenAI built-in image generation as a new transparent raster. Its exact prompt, source output path, dimensions and SHA-256 are retained in the folder's `Provenance.json`; the prompt explicitly requires original shapes and excludes recognizable designs from named games. It remains unconnected candidate art until learner integration and Unity visual acceptance.

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

The October 14 interface clarity pass uses unmodified Lato Regular/Bold by Lukasz Dziedzic / tyPoland, distributed under SIL Open Font License 1.1. Font binaries, the complete OFL notice and pinned source URLs/SHA-256 hashes are in `Assets/StudioArtPack/InterfaceV2/Fonts`. Google Fonts revision: `5d3b76120a319730fda218cc7410174a462b32cb`. Source: [Google Fonts Lato](https://github.com/google/fonts/tree/5d3b76120a319730fda218cc7410174a462b32cb/ofl/lato). This license applies to the included font software; it does not alter song/model licenses.

The October 22 Japanese/English UI preparation adds unmodified M+ 1c Regular by Coji Morishita / M+ FONTS PROJECT under SIL Open Font License 1.1. The font, complete license and pinned URLs/SHA-256 hashes are in `Assets/StudioArtPack/InterfaceV2/Fonts/Japanese`. Source revision: `0d4459efc913a91f33c3f08b219a5a95d282c7b8`; source: [M+ Fonts for Google Fonts](https://github.com/rayshan/mplus-fonts/tree/0d4459efc913a91f33c3f08b219a5a95d282c7b8). The asset is prepared for Japanese and Latin UI; runtime hookup, Unity glyph rendering and layout acceptance remain separate work. It does not establish Simplified Chinese UI acceptance.

The October 25 Simplified Chinese UI preparation adds unmodified Noto Sans SC Variable from Google Fonts under SIL Open Font License 1.1. The font, complete license, upstream metadata and pinned URLs/SHA-256 hashes are in `Assets/StudioArtPack/InterfaceV2/Fonts/Chinese`. Google Fonts revision: `e7db6ca603fd4862010383ff17ffd6cc4ffac1c0`; source: [Google Fonts Noto Sans SC](https://github.com/google/fonts/tree/e7db6ca603fd4862010383ff17ffd6cc4ffac1c0/ofl/notosanssc). A fixed Medium instance used by legacy Unity UI is derived locally with `fontTools varLib.instancer wght=500`; its exact command, size and hash are retained in the same provenance record. Exact-glyph, runtime-format and 1080p/2K start-screen checks cover the current prepared strings, while exact standalone-player acceptance remains separate.

## October 29 open music and studio imports

Cozy Puzzle In-Game 1 & 2 and Magic Puzzle In-Game 1 & 2 by **MintoDog** are included under **CC0 1.0** from the author's OpenGameArt uploads. Recordings remain unchanged; project-authored practice grids select short windows. Exact source pages, downloads, sizes and hashes: `Tools/Rhythm/open-music-sources.json`. Notices and license: `Assets/StreamingAssets/Rhythm/OpenMusic20261028`.

**Chaoz Fantasy (2007), Chaoz Airflow (2009), Chaoz Impact (2008)** by **ParagonX9**, copyright ParagonX9, are included under **Creative Commons Attribution-ShareAlike 3.0 Unported**. Author: [ParagonX9](https://paragonx9.newgrounds.com/). Sources: [Fantasy](https://www.newgrounds.com/audio/listen/85046), [Airflow](https://www.newgrounds.com/audio/listen/245921), [Impact](https://www.newgrounds.com/audio/listen/152047). [License](https://creativecommons.org/licenses/by-sa/3.0/). Recordings are byte-identical and play completely. Project-authored classic practice charts and project contributions to synchronized sequences are under the same CC BY-SA 3.0 license. No original rhythm-game charts are copied. Original MP3s, full license, notices and download/hash manifest are in `Assets/StreamingAssets/Rhythm/Classics20261029`. No artist endorsement is implied. The recordings retain BY-SA rights independently of the StepMania package restriction.

**Kenney Furniture Pack 1.0**, by **Kenney (www.kenney.nl)**, supplies five OBJ/MTL models under **CC0 1.0**: pottedPlant, plantSmall1/2/3 and books. Source: [author's upload](https://opengameart.org/content/furniture-kit). Original model/license bytes and archive/selected-file hashes are preserved in `Assets/StudioArtPack/OpenExpansion20261028/KenneyFurniture`. URP material derivatives use original diffuse colors. Seven existing studio-expansion prefabs also enter production with their original prompts/attribution unchanged. See [update guide](Docs/Art/OPEN_MUSIC_STUDIO_UPDATE_20261029.md).

## Downloaded rhythm study references

[StepMania](https://github.com/stepmania/stepmania) supplies the public reference packs Goin' Under (NegaRen), Mecha-Tribe Assault (Kommisar), and Springtime (Kommisar). Upstream describes included songs as CC-NC, without a precise license version in its README. Chart-level credits remain in the original files.

At the user's request, the original collection is included in `Assets/StreamingAssets/Rhythm/StepMania` for noncommercial development packages. The upstream README, chart credits, source manifest and NOTICE.txt accompany the unmodified files. Do not sell packages containing these songs or claim commercial clearance; upstream does not specify a precise CC license version. The local reference directory remains a download cache. See [Music and charts](Docs/Design/MUSIC_AND_CHARTS.md) for provenance and packaging details.

The October 13 prepared gameplay track uses Goin' Under by NegaRen, dance-single Beginner chart by Fraxtil. `Assets/Audio/Music/StepMania/GoinUnder.ogg` is a byte-identical runtime copy of the pinned audio. `Assets/StreamingAssets/Rhythm/Prepared/GoinUnder_Beginner.json` is a derived event timeline: simultaneous tap lanes become one target event, with original lane metadata retained. It records the source revision, URLs, SHA-256 hashes and adaptation. No source audio or chart was edited; the same noncommercial restrictions and original notices apply. Asset preparation does not establish runtime integration.

## October 16 development captures

`Assets/Art/Previews/PrototypeScene_20261016.png` is an unedited 2560 × 1440 Unity camera render of the saved studio after fixed-room batching, without UI or balloons. `CreatorStats_20261016.png` is the creator-supplied Unity Game-view screenshot showing gameplay and Editor Stats. These are development evidence, not generated concept art or a standalone benchmark. All depicted assets retain their existing attribution and recorded prompts.

## October 18 standalone gameplay capture

`Assets/Art/Previews/Gameplay_v0.1.0-demo.2_20261018.jpg` is the unmodified 2560 × 1440 Windows capture taken from `Builds/Windows-x64-v0.1.0-demo.2/BalloonShooter.exe` after starting a round. It shows actual balloons, weapon and HUD, with no reconstructed or generated gameplay pixels. Capture SHA-256: `7624cebbc07c34d240705ed36bcee4dcac1d58c3737420ebae76452196a4beab`; captured build's `Assembly-CSharp.dll`: `b3deaa2df189bd650cbc69a161194a4b43597395d8432c464a3d867e6dc3595a`. Resized/compressed web copies and unchanged concept-image derivatives retain their existing asset attribution and prompts; preparation settings and hashes are in `Website/media/provenance.json`. No new image generation or external art import occurred. This capture establishes the displayed scene, not full-song physical acceptance or sustained performance.

## October 17 surface-paint lesson assets

`Assets/StudioArtPack/SurfacePaint` reuses the existing GraffitiSpray_Atlas_v3 Teal_Spray slice without changing image pixels, import settings or the exact recorded prompt. The new material is a native copy of SprayUnlit, using the installed unmodified URP sprite-unlit shader. The small SpriteRenderer prefab and close-up preview are native Unity outputs; no new external download or generated image is introduced. Source paths/hashes and preparation settings are in its `Provenance.json`; the original prompt remains in [Spray assets](Docs/Art/SPRAY_ASSETS.md#recorded-generation-prompts). The preview is an asset placement render, not a recorded gameplay hit.

The three-color balloon lesson prefab is a native copy of the current production balloon with the existing cosmetic appearance component, one retained mesh and the already prepared teal/coral/gold latex materials. It adds no image, model, external download or new generated prompt. Original Poly by Google attribution and all prior provenance remain unchanged.

The October 17 best-score results row is native editable Unity UI using existing Lato Bold, colors and panel artwork. Results_BestScore_1080p.png is a Unity layout render with illustrative score values, not a recorded result. No new generated image, font, download or prompt was introduced; all original UI art prompts and Lato notices remain applicable.

The October 18 author credit reads **何单东 Shandong He**. Its Balloon Credit Sans Regular font is a renamed, approximately 19 KB subset of the official Noto Sans CJK SC Regular, retaining printable ASCII and the three name characters only. Original copyright and SIL Open Font License 1.1 are retained in `Assets/StudioArtPack/InterfaceV2/Fonts/AuthorCredit/OFL.txt`; pinned upstream commit/URLs, source/derived SHA-256 and fontTools preparation details are in its `Provenance.json`. The original download stays in ignored `.local-reference/Fonts/NotoCredit`. This narrow font does not establish general Chinese or Japanese UI coverage. The cream backing plate and author label are native Unity UI, with no new generated image or changed art prompt.


## Audience celebration assets — October 31, 2026

The four unchanged Noto Emoji PNGs (U+1F44D, U+1F44F, U+1F389, U+1F929) are by Google LLC and Noto Emoji contributors, under Apache License 2.0. The two unchanged cheer MP3s, Cheer-Yay01-2 (Mid-Solo) and Cheer-Crowd01-3 (Low-Short), are by [OtoLogic](https://otologic.jp/free/se/applause-cheer01.html), under [CC BY 4.0](https://creativecommons.org/licenses/by/4.0/). Runtime scales/moves the PNGs and plays gain-adjusted, faded excerpts of the audio. [Provenance](Assets/Resources/Audience/Provenance.json) records pinned URLs, source hashes and restrictions; [notices and full licenses](Assets/StreamingAssets/ThirdParty/Audience/NOTICE.txt) accompany builds. These imports do not change the existing noncommercial music restriction.
