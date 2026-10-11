# Project structure and contribution rules

## Ownership

The learner owns `Assets/Scripts` gameplay. The assistant maintains side-effect-free foundation settings, editor tools, asset preparation, tests, documentation, and website. This decision preserves hands-on learning while reducing repetitive configuration work. New settings do not change gameplay until integrated and tested by the learner.

`BalloonBarrage.Foundation` isolates foundation code. The Learning Workbench checks enabled prototype shooter/spawner references and fields; it does not prove timing, hits, or game flow. Open it through `Tools > BalloonBarrage > Learning Workbench`.

## Layout

| Path | Responsibility |
|---|---|
| Assets/Scripts | Learner gameplay |
| Assets/Framework/Runtime | Foundation settings and data |
| Assets/Framework/Editor | Editor helpers |
| Assets/Framework/Tests/Editor | Configuration and import tests |
| Assets/Settings | Render, input and gameplay configuration |
| Assets/Scenes and Assets/Prefabs | Gameplay scenes and prefabs |
| Assets/Art and Assets/Audio | Canonical models, textures, previews, sound and music |
| Assets/StudioArtPack | Generated art, reusable props, menus and previews |
| Assets/Localization | Player-facing translations |
| Docs/Design | Design, roadmap, music/chart workflow |
| Docs/Art | Art catalog, integration guidance and recorded prompts |
| Docs/Project | Maintenance, validation and distribution |
| Tools/Audio | Original music generator |
| Tools/Rhythm | Pinned reference manifest and downloader |
| Tools/ProjectMaintenance | Repository audits and migration utility |
| Website | Site source, build scripts and tests |
| .local-reference | Ignored local third-party study downloads |
| .local-backups | Ignored snapshots and historical validation outputs |

Keep Assets, Packages and ProjectSettings at the Unity root. The active local checkout is `D:/CS/Code/BalloonBarrage`; retired checkouts are not authoritative.

## Maintenance rules

Write all current and future project documentation in English. Keep player-facing translations in their existing data files. Update the authoritative document and link to it rather than duplicating plans or status narratives. Keep source attribution, recorded prompts and unresolved validation issues when condensing documentation.

Preserve `.meta` files and GUIDs when moving Unity assets. Downloaded model packages retain their palette relationships. Preview scenes, prefab variants and older generated directories may still be referenced; do not delete them merely because their names look obsolete. Check references and content hashes first.

Do not commit Unity caches, website output, builds, backups, local reference downloads or nested Git repositories. Models, images and audio use Git LFS. Put playable ZIPs in GitHub Releases.

Inspect user edits before making changes and retain them. Append ordinary commits; the historical October 3 repository rebuild is not an instruction to rewrite history again. Previous migration narratives are recoverable from Git history and local backups.

Run the checks in [Validation](VALIDATION.md) and use the [release guide](WEBSITE_AND_RELEASES.md) for publication.
