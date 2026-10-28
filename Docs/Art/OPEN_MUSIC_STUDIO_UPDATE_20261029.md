# Open music and production studio update — October 29, 2026

This update connects seven additional recordings through the existing song selector and promotes current studio art into `PrototypeScene`. It changes data, assets, scene decoration, Editor tools and the website. Learner-owned `Assets/Scripts` files remain unchanged. [Game design](../Design/GAME_DESIGN.md) owns behavior, [Roadmap](../Design/ROADMAP.md) owns delivery, and [Validation](../Project/VALIDATION.md) owns completed evidence.

## Songs and licenses

| Track | Artist | Original publication | License | Round | Practice events |
|---|---|---|---|---|---|
| Cozy Puzzle In-Game 1 | MintoDog | February 5, 2026 | CC0 1.0 | 81.356 seconds | 77 |
| Cozy Puzzle In-Game 2 | MintoDog | March 6, 2026 | CC0 1.0 | 85.333 seconds | 61 |
| Magic Puzzle In-Game 1 | MintoDog | July 22, 2026 | CC0 1.0 | 87.273 seconds | 77 |
| Magic Puzzle In-Game 2 | MintoDog | September 2, 2026 | CC0 1.0 | 80 seconds | 77 |
| Chaoz Fantasy | ParagonX9 | June 18, 2007 | CC BY-SA 3.0 | Full recording, 133.904 seconds | 173 |
| Chaoz Airflow | ParagonX9 | June 11, 2009 | CC BY-SA 3.0 | Full recording, 253.780 seconds | 362 |
| Chaoz Impact | ParagonX9 | June 18, 2008 | CC BY-SA 3.0 | Full recording, 115.540 seconds | 160 |

These join Goin' Under, Springtime and Mecha-Tribe Assault for ten choices. Each addition has a distinct versioned record identity. All recordings are byte-identical to pinned downloads. MintoDog tracks use 32- or 40-bar windows; classics play completely without looping or cropping. New AudioClip imports stream audio rather than decompressing all seven recordings on scene load.

Author-uploaded pages for [Cozy 1](https://opengameart.org/content/cozy-puzzle-in-game-1), [Cozy 2](https://opengameart.org/content/cozy-puzzle-in-game-2), [Magic 1](https://opengameart.org/content/magic-puzzle-in-game-1) and [Magic 2](https://opengameart.org/content/magic-puzzle-in-game-2) identify each as CC0. ParagonX9's pages for [Fantasy](https://www.newgrounds.com/audio/listen/85046), [Airflow](https://www.newgrounds.com/audio/listen/245921) and [Impact](https://www.newgrounds.com/audio/listen/152047) each link to CC BY-SA 3.0 Unported. This licenses the recording; DAW project sources were not supplied.

Preserve ParagonX9 attribution, titles, license links and modification statements. Project-authored classic charts and project contributions to their synchronized sequences are under CC BY-SA 3.0. Original MP3s, complete license, notices and manifests are included in `StreamingAssets/Rhythm/Classics20261029`, with freely accessible byte-identical audio under `Originals`. Independent software/assets retain their own notices. StepMania restrictions still make the complete mixed package noncommercial; they do not relicense CC0 or BY-SA additions.

New charts are project-authored two-beat practice grids, separate from osu!, Geometry Dash or community StepMania charts. MintoDog tempos come from author filenames; Fantasy's 162.347 BPM and Airflow's 175 BPM come from author descriptions. Impact uses provisional 175 BPM, with a community reference explicitly recorded. Phase zero, simple grids and full-file MP3 durations are preparation decisions; human listening calibration, encoder-delay alignment and musical phrase review remain unverified. Automated settlement does not establish chart quality.

## Reproduction

[Open music sources](../../Tools/Rhythm/open-music-sources.json) and [classic music sources](../../Tools/Rhythm/classic-music-sources.json) pin exact source/download URLs, dates, byte lengths, SHA-256, licenses and timing evidence. Downloads remain in ignored `.local-reference/OpenExpansion20261028`.

```powershell
python Tools/Rhythm/prepare_open_music.py --download
python Tools/Rhythm/prepare_classic_music.py --download
python -m unittest discover -s Tools/Rhythm -p 'test_*.py'
```

Omit `--download` for a verified cache. Preparation rejects changed sources/destinations and preserves audio bytes. Unity metadata is independent of source hashes. Explicit Git attributes preserve downloaded license/MTL bytes and manifests across checkouts.

`OpenExpansionInstaller.Apply` initially installs four tracks and art; `AddClassics` appends three classics. These batch-only, guarded one-time operations preserve subsequent manual edits. Use saved assets normally. `OpenMusicRuntimeValidation.Run` checks ten profile references and exercises two real-time natural rounds, pause/resume, results and Retry. `KyudaisaiRuntimeSmokeCheck.RunBatch` separately checks selection, original Springtime flow and capacity. Both provide Editor evidence, distinct from an extracted package.

## Production art

Seven prefabs from the [current expansion](STUDIO_EXPANSION_20261028.md) enter production: supply trolley, roller tray, easel, swatch rail, drying rack and two framed prints. Their meshes, materials, GUIDs, original provenance and exact generation prompts are preserved. Separate gallery/study scenes remain available.

Five Kenney decorations use original OBJ/MTL files from the author's [Furniture Kit upload](https://opengameart.org/content/furniture-kit): potted plant, three small plants and books. The [official catalog](https://kenney.nl/assets/furniture-kit) confirms CC0; its download returned HTTP 403, so the author's OpenGameArt distribution was used. This is a new import, not a claim that the 2018 pack was newly released. Original license bytes, archive hash and selected-file hashes accompany `Assets/StudioArtPack/OpenExpansion20261028/KenneyFurniture`.

URP/Lit derivatives retain MTL diffuse colors and share instanced materials. Added decoration has no colliders or runtime scripts, stays around the perimeter and uses existing probes/realtime lighting. Existing gameplay component data, target area and lightmaps remain. Seven studio prefabs already consolidate geometry; no new bake, draw-call measurement or player-FPS improvement is claimed.

Native captures are under `Assets/StudioArtPack/OpenExpansion20261028/Previews`. Website derivatives record source/output hashes. Scene renders, automated Play Mode captures and standalone checks are labelled separately. No new image generation occurred.

## Delivery

Candidate version: `v0.1.0-demo.7`, Windows x64 and unsigned macOS Universal, built from one named isolated checkout. [Validation](../Project/VALIDATION.md) records actual build/package evidence and exact hashes; the [release runbook](../Project/PACKAGING_LESSON.md) owns commands. Demo.6 remains a fallback. Full ten-song human play/listening, exhibition hardware and real-Mac observations remain separate.
