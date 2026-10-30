# Kyudaisai future-direction survey

## Status and intent

The creator selected Google Forms for a visitor poll and confirmed on October 31, 2026: **nickname required; email only for visitors who choose future playtest invitations**. The website offers a compact, translated question preview at `/survey/`, `/ja/survey/` and `/en/survey/`. Google authentication and a verified public responder link are still pending. No live form, votes or registrations are delivered by the preview page.

The poll explores priorities within the [confirmed design](../Design/GAME_DESIGN.md#open-world-direction--planning-only). Sunny street festival plus handmade town details remains selected; the night festival remains deferred. Votes inform investigation, not binding features or dates. The [roadmap](../Design/ROADMAP.md#open-world-development-track--planning-only) still owns delivery gates.

## Questionnaire

Visitor translations and the four single-choice option sets live in [survey-strings.mjs](../../Website/src/survey-strings.mjs). Keep the live form consistent with that source. Use Japanese for the exhibition audience, with Chinese and English equivalents available. Do not mark an option as recommended or preselect it.

| Order | Field | Requirement and choices |
|---|---|---|
| 1 | Nickname | Required short text. Do not require a real name, student ID, telephone number or account identity. |
| 2 | Experience priority | Exploration/hidden stages; musical rival duels; collectible ensemble parts; not sure. |
| 3 | District after the first pier | Melody Garden; Syncopation Rooftops; more plaza activities; not sure. The first pier is not reopened for a vote. |
| 4 | Candidate opponent | Beat Teaser; Balloon Porter; Stage Leader; prefer solo play first; not sure. These are candidates, not a promise to deliver all three. |
| 5 | Handmade detail priority | Paper posters/signs; fabric/wood/balloon decorations; instrument props/local bands; not sure. |
| 6 | Additional suggestion | Optional paragraph. Ask visitors not to include other people's personal information. |
| 7 | Future playtest invitations | Required choice: opt in and proceed to the email section, or vote only and submit immediately. No option preselected. |
| 8 | Email | Required only in the opt-in section; validate email format. Make its invitation-only purpose explicit. Format validation does not prove ownership. |

The four poll questions are required single-choice fields, each with a neutral “not sure” option. Allow approximately 1–2 minutes. No answer should suggest that the future open world is already playable.

## Google Form setup and information handling

- Keep automatic collection of Google account email addresses off. Use the conditional email field above, so voting alone never requires an address.
- For easy event access, do not require sign-in or enable the one-response-per-account restriction. Ask each visitor to answer once; this does not guarantee one person/one vote. A nickname is not verified identity.
- Use a multiple-choice invitation question and Google Forms section routing to bypass the email section for vote-only respondents.
- Keep “View results summary” off: it may disclose full text responses. Keep form editing and response/spreadsheet access with the creator unless explicitly expanded. Share the responder link, never an editor or response-sheet link.
- Explain before submission: nickname for organizing feedback; optional email only for future playtest invitations; no public names, emails or raw comments; any public results contain aggregate vote counts only. Registration is interest, not a confirmed place or schedule.
- Before opening responses, add the creator's actual contact method and a chosen retention/deletion period to the live form. These details are not yet supplied; do not invent them or claim an automatic deletion mechanism exists.
- Never commit responses or personal details to Git, put them into website source, or prefill them in URLs. The static website collects no survey data and has no local submission endpoint, analytics or storage for answers.

References: Google on [sharing and response summaries](https://support.google.com/docs/answer/2839588), [section branching](https://support.google.com/docs/answer/141062) and [response/email management](https://support.google.com/docs/answer/139706).

## Connecting the website

`Website/src/survey-config.mjs` holds only the public Google Forms responder URL. An empty URL renders an explicit not-open state with no inputs or submit button. A supported responder URL enables the external form link; it must not contain personal prefills. Preserve all three locale routes and the short homepage festival link.

Before setting that URL, verify the nickname requirement, both invitation branches, email validation, private response settings, contact/retention notice and visitor access without an owner account. Use clearly labelled synthetic test data only when authorized to submit a test response. Website tests validate rendering and links; they do not establish that the live Google Form branches, saves data or protects its responses correctly.
