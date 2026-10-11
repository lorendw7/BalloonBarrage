# Project working agreements

- Write all project documentation in English, including future additions and revisions. Player-facing game and website translations remain separate.
- Keep README as the entry point, Docs/Design/GAME_DESIGN.md as the design source, and Docs/Design/ROADMAP.md as the delivery plan. Link to these rather than repeating their contents.
- The learner owns Assets/Scripts gameplay. Maintain tools, assets, tests, documentation, and the website; do not implement new gameplay without explicit authorization. Publishing saved learner code does not authorize completing unfinished features.
- Preserve Unity asset GUIDs and paired .meta files. Inspect working changes before editing and never overwrite concurrent learner work.
- Keep downloaded rhythm references in the ignored .local-reference directory. Record provenance, pinned URLs, hashes, and usage restrictions before importing external content into Assets or a release.
- Preserve attribution and exact recorded image-generation prompts. Remove superseded narrative from active documentation; Git history retains prior versions.
- Distinguish saved implementation, user-reported playtests, automated checks, and unverified plans. Do not claim a playable release or passed Unity test from a website/static check.
- Append normal commits; never force-push or rebuild history unless specifically requested.
