# Localization

Project documentation is English only, including future updates. Player-facing text separately targets Simplified Chinese (`zh-Hans`), Japanese (`ja`) and English (`en`).

`Assets/Localization/UIStrings.json` holds 52 stable keys with three translations. The current-screen additions cover title/help, HUD labels, dynamic cue and on-beat feedback, pause, results, successful/on-beat/max-streak formats and language names. They are prepared copy, not connected runtime labels. It is not a Unity Localization String Table and has no runtime language switching yet. The festival runtime commitment exposes Japanese and English from this shared data; Simplified Chinese remains prepared for later integration. Website translations live separately in `Website/src/strings.mjs`.

Use shared text-free menu and VFX images. Keep keys such as `menu.start` stable and preserve placeholders such as `{0}` across locales. Switching language must refresh existing UI and persist the preference.

Japanese readability is required for the exhibition build. `Assets/StudioArtPack/InterfaceV2/Fonts/Japanese/MPlus1C-Regular.ttf` is an unmodified M+ 1c Regular asset covering Japanese and Latin, accompanied by the complete SIL OFL 1.1 notice and pinned source/hash provenance. The asset and its Unity `.meta` identity are saved, but no switchable label references it and its exact glyph rendering/layout has not been accepted. The current UI uses legacy `UnityEngine.UI.Text`, so the first implementation should assign this Font to switchable labels rather than introducing a TextMeshPro migration before the festival. Allow wrapping for instructional sentences; do not squeeze them into small buttons.

For the festival, keep one set of screens. A small language controller persists `ja` or `en`, refreshes registered static labels, and supplies the format choice for dynamic cue, on-beat feedback and result strings. Title-screen Japanese/English controls must not fire into gameplay. Default to Japanese only when no saved preference exists. Switching must not reload a round or reset a score.

Future feature visibility belongs in runtime code, not deletion of translations. Desktop co-op and mobile solo remain different planned configurations. See [menu assets](MENU_ASSETS.md) for artwork and the [roadmap](../Design/ROADMAP.md) for delivery priorities.
