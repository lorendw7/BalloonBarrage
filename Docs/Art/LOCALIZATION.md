# Localization

Project documentation is English only, including future updates. Player-facing text separately targets Simplified Chinese (`zh-Hans`), Japanese (`ja`) and English (`en`).

`Assets/Localization/UIStrings.json` holds 60 stable keys with three translations. The current-screen additions cover title/help, HUD labels, dynamic cue and on-beat feedback, pause, results, successful/on-beat/max-streak formats and all three language names. It is a small project-owned JSON table rather than a Unity Localization String Table. The saved runtime exposes Simplified Chinese, Japanese and English from this shared data. Website translations live separately in `Website/src/strings.mjs`.

Use shared text-free menu and VFX images. Keep keys such as `menu.start` stable and preserve placeholders such as `{0}` across locales. Switching language must refresh existing UI and persist the preference.

Japanese and Simplified Chinese readability are required for the exhibition build. `Assets/StudioArtPack/InterfaceV2/Fonts/Japanese/MPlus1C-Regular.ttf` is an unmodified M+ 1c Regular asset covering the prepared Japanese and Latin copy. The original `Assets/StudioArtPack/InterfaceV2/Fonts/Chinese/NotoSansSC-Variable.ttf` remains pinned; `NotoSansSC-Medium.ttf` is the locally derived fixed weight-500 instance used by legacy `UnityEngine.UI.Text`, where the variable source rendered too lightly. The derivation command and both hashes are recorded in the font provenance. Each family retains its complete SIL OFL 1.1 notice and paired Unity metadata. Unity exact-character validation found no missing glyphs in the current 60 entries. Connected labels select the bundled font for the active CJK language and restore their original font for English. Allow wrapping for instructional sentences; do not squeeze them into small buttons.

`LocalizationTableValidation` checks locale order, unique/non-empty entries, placeholder agreement and exact glyph availability in the imported Unity fonts. It is a structure/import check, not evidence that connected labels fit or render correctly in a standalone player.

For the festival, keep one set of screens. The saved language controller persists `zh-Hans`, `ja` or `en` under `ui.language.v1`, refreshes registered static labels, and supplies the format choice for dynamic cue, on-beat feedback and result strings. Title-screen language controls do not fire into gameplay. The default is Japanese only when no saved preference exists. Switching does not reload a round or reset a score.

## Saved implementation

1. `LocalizationManager` loads the JSON table once into a key lookup, exposes a missing-key fallback and formats placeholder-bearing entries through `Format`.
2. `LanguageSettings` persists the selected locale and emits one change event.
3. `LocalizedText` supports both `TMP_Text` and the production legacy `UnityEngine.UI.Text`, updates only on enable or a language event, switches CJK fonts without replacing each label's English font, and can apply a serialized Simplified-Chinese-only contrast color where a light background requires it.
4. Six production UI prefabs contain 31 static label bindings. The start-screen prefab owns the runtime manager and visible `中文`, `日本語` and `EN` buttons.
5. `RhythmChartReader` formats the NEXT/ENDING cue and on-beat award from localization keys. `RoundController` formats successful hits, on-beat hits and maximum streak on results, and explicitly refreshes the visible dynamic copy after a language change.
6. `LocalizedFont` switches the three existing runtime-written legacy Text labels between the bundled CJK fonts and their original English font without owning or replacing their content.
7. `LocalizationUiInstaller`, `LocalizationTableValidation`, `LocalizationRuntimeValidation` and `LocalizationPreviewCapture` keep installation repeatable and verify the saved mappings, locale data, placeholders, glyph coverage, runtime lookup, representative formatted values and the 1080p/2K Chinese start layout.

## Verification and remaining work

On 2026-10-25, Unity 6000.0.77f1 passed localization asset and runtime validation after installation of 31 static bindings and three dynamic-font bindings. The runtime check covered not-ready and missing-key fallbacks, all three locale lookups and representative on-beat, countdown and result formatting. A focused contrast pass then rendered the Simplified Chinese start screen at 1920 x 1080 and 2560 x 1440 using the fixed Medium font and serialized dark-on-light colors; both renders passed font, contrast, overflow and bounds checks, and the 2K output was visually inspected. Earlier Play Mode checks visibly switched the start screen among Simplified Chinese, Japanese and English and loaded the selected locale into the gameplay HUD.

The same editor session visually checked the Japanese start, HUD, pause, natural results and Retry flow at both 2560 x 1440 and 1920 x 1080. Each natural run played the complete 123.429-second clip; results appeared without a forced endpoint, and Retry returned to the title. The pause presentation was displayed through the existing saved pause-screen instance because synthetic Esc did not reach Unity's Input System; this is a layout check, not new physical-input acceptance. No clipping or unintended wrapping was observed in these views. The previously observed three-language start-screen switching plus exact glyph validation support the remaining locales, but full connected-screen acceptance on a standalone exhibition build remains open.

The cached beat countdown is invalidated on language change, and unchanged tenths are not reassigned each frame. Missing keys keep their clear development fallback and warning. Before the festival candidate is accepted, repeat the three-language connected-screen pass in the standalone player and include real pause input, nonzero localized result values and an actual on-beat notification.

Future feature visibility belongs in runtime code, not deletion of translations. Desktop co-op and mobile solo remain different planned configurations. See [menu assets](MENU_ASSETS.md) for artwork and the [roadmap](../Design/ROADMAP.md) for delivery priorities.
