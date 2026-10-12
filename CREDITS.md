# Credits and provenance

## Existing third-party assets

| Asset | Recorded creator/source | Recorded license | Verification status |
|---|---|---|---|
| Balloon | Poly by Google, via [Poly Pizza](https://poly.pizza) | CC-BY | Preserve attribution; exact original page/version and full text still need verification |
| Blaster Kit | [Kenney](https://kenney.nl) | CC0 | Existing project declaration; exact download record still needs verification |
| Impact Sounds | [Kenney](https://kenney.nl) | CC0 | Existing project declaration; exact download record still needs verification |

These retained declarations are not a new independent license audit. Do not describe all project assets as CC0. Preserve model/palette packages and third-party notices.

## Project-created content

Studio textures, sprays, menu images, latex fragments and paint concepts use built-in image generation. Exact recorded prompts are retained in the [art guides](Docs/Art/ASSET_CATALOG.md). Older concept/splat prompts are unavailable. Concept images do not establish licenses for imported models.

The October 11 Interface V2 background and transparent sprite sheet use built-in image generation; their exact prompts are recorded in [Interface V2](Docs/Art/INTERFACE_V2.md). UI layouts are editable Unity Canvas prefabs, with Unity's built-in LegacyRuntime font. Their preview PNGs are Unity camera renders, and displayed scores are illustrative samples.

RoomFinishV7 adds a generated oak-plank albedo texture, with its exact prompt in [Studio art pack](Docs/Art/STUDIO_ART_PACK.md). Cream wall and ivory ceiling materials are native Unity material settings. Floor UVs reuse the existing mesh shape; the refreshed reflection is a Unity bake.

ExhibitionV8 reuses those room materials and the BalloonPopV2 latex atlas. Its added shell/skirting uses Unity cubes, and paint droplets use a Unity sphere mesh with particle materials. No new AI-generated raster or external download was introduced in this pass; the wall/effect previews and refreshed reflection are Unity renders.

CoverFinishV9 adds mathematical mineral-plaster and leaf-vein textures, procedural rounded joinery, leaf geometry and a sideboard. Existing wood/cloth assets retain their provenance. Its offline lightmaps, reflection and previews are Unity renders; it adds no AI-generated raster or external download. The exact earlier prompts remain in the linked art guides. See [Cover finish V9](Docs/Art/COVER_FINISH_V9.md).

The courtyard is fictional and is not official Kyushu University or Qpic imagery. Frames, props, curved leaves, alcoves, refined gun/balloon surfaces, ropes and meshes use Unity primitives or procedural geometry. Native woven, normal and surface maps are mathematical patterns; fine oak retains its generated-image provenance. Room reflections and prototype previews are Unity camera captures.

The five current WAVs in `Assets/Audio/Music` are original synthesized compositions produced with `Tools/Audio/compose_studio_music.py`. They use no third-party recordings, samples or songs. This statement applies to those files only and does not change any existing asset license.

## Downloaded rhythm study references

[StepMania](https://github.com/stepmania/stepmania) supplies the public reference packs Goin' Under (NegaRen), Mecha-Tribe Assault (Kommisar), and Springtime (Kommisar). Upstream describes included songs as CC-NC, without a precise license version in its README. Chart-level credits remain in the original files.

At the user's request, the original collection is included in `Assets/StreamingAssets/Rhythm/StepMania` for noncommercial development packages. The upstream README, chart credits, source manifest and NOTICE.txt accompany the unmodified files. Do not sell packages containing these songs or claim commercial clearance; upstream does not specify a precise CC license version. The local reference directory remains a download cache. See [Music and charts](Docs/Design/MUSIC_AND_CHARTS.md) for provenance and packaging details.
