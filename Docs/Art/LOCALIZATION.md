# Localization

Project documentation is English only, including future updates. Player-facing text separately targets Simplified Chinese (`zh-Hans`), Japanese (`ja`) and English (`en`).

`Assets/Localization/UIStrings.json` holds 25 stable keys with three translations. It is not a Unity Localization String Table and has no runtime language switching yet. Teach a loader or migrate to formal tables as a later implementation step. Website translations live separately in `Website/src/strings.mjs`.

Use shared text-free menu and VFX images. Keep keys such as `menu.start` stable and preserve placeholders such as `{0}` across locales. Switching language must refresh existing UI and persist the preference.

Japanese readability is required for the exhibition build. Select appropriately licensed fonts with Japanese, Chinese and Latin coverage, configure TextMeshPro fallback assets, and test actual glyphs and long labels. Font coverage is not yet verified. Allow wrapping for device messages; do not squeeze whole sentences into small buttons.

Future feature visibility belongs in runtime code, not deletion of translations. Desktop co-op and mobile solo remain different planned configurations. See [menu assets](MENU_ASSETS.md) for artwork and the [roadmap](../Design/ROADMAP.md) for delivery priorities.
