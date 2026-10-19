# Project working agreements

- Write all project documentation in English, including future additions and revisions. Player-facing game and website translations remain separate.
- Keep README as the entry point, Docs/Design/GAME_DESIGN.md as the design source, and Docs/Design/ROADMAP.md as the delivery plan. Link to these rather than repeating their contents.
- The learner owns Assets/Scripts gameplay. Maintain tools, assets, tests, documentation, and the website; do not implement new gameplay without explicit authorization. Publishing saved learner code does not authorize completing unfinished features.
- For future core gameplay requests, explain one small step and let the learner write the code, then review and verify it. Write core gameplay directly only when the creator explicitly asks the assistant to write that code.
- Preserve Unity asset GUIDs and paired .meta files. Inspect working changes before editing and never overwrite concurrent learner work.
- Keep download caches in ignored .local-reference. The user authorized the pinned StepMania collection in Assets/StreamingAssets/Rhythm/StepMania for noncommercial development packages. Preserve original files, attribution, manifest and NOTICE.txt; do not represent the songs as commercially cleared. Record provenance, pinned URLs, hashes and usage restrictions for future imports.
- Preserve attribution and exact recorded image-generation prompts. Remove superseded narrative from active documentation; Git history retains prior versions.
- Distinguish saved implementation, user-reported playtests, automated checks, and unverified plans. Do not claim a playable release or passed Unity test from a website/static check.
- Append normal commits; never force-push or rebuild history unless specifically requested.
