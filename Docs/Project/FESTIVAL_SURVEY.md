# Kyudaisai future-direction survey

## Status and intent

The creator selected Google Forms for a visitor poll and confirmed on October 31, 2026: **nickname required; email only for visitors who choose future playtest invitations; email retention limited to 90 days**. The [published questionnaire](https://docs.google.com/forms/d/e/1FAIpQLSeVouVCSTX5NwfYFtdqnif3T98-n9IXLhL2dqh8Y2uucesujQ/viewform?usp=publish-editor) uses Japanese, Chinese and English labels. The compact website pages at `/survey/`, `/ja/survey/` and `/en/survey/` explain the choices and link to that responder form; they do not receive submissions themselves.

The poll explores priorities within the [confirmed design](../Design/GAME_DESIGN.md#open-world-direction--planning-only). Sunny street festival plus handmade town details remains selected; the night festival remains deferred. Votes inform investigation, not binding features or dates. The [roadmap](../Design/ROADMAP.md#open-world-development-track--planning-only) still owns delivery gates.

## Questionnaire

Visitor translations, the four single-choice option sets and their short examples live in [survey-strings.mjs](../../Website/src/survey-strings.mjs). Each question has a collapsed explanation of every substantive option, so visitors can compare without lengthening the homepage. Examples describe proposals, not implemented features. Keep the live form consistent with that source. Use Japanese for the exhibition audience, with Chinese and English equivalents available. Do not mark an option as recommended or preselect it.

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
- The creator selected a 90-day limit for signup emails. The opt-in section and all three website translations disclose manual deletion within 90 days of signup. The form footer's verified **Contact form owner** control is the withdrawal/deletion channel; no creator email address needs to be published on the website.
- The creator must manually remove signup responses containing expired emails, including any later private spreadsheet or export copies, within that limit and handle withdrawal requests. Google Forms has not been configured with automatic deletion, and no background cleanup service is claimed. Retain only aggregate vote counts when deleting those records.
- Keep response autosave off for shared exhibition devices, the repeat-submission link off, and question order fixed. These settings do not prevent duplicate votes.
- Never commit responses or personal details to Git, put them into website source, or prefill them in URLs. The static website collects no survey data and has no local submission endpoint, analytics or storage for answers.

References: Google on [sharing and response summaries](https://support.google.com/docs/answer/2839588), [section branching](https://support.google.com/docs/answer/141062) and [response/email management](https://support.google.com/docs/answer/139706).

## Connecting the website

`Website/src/survey-config.mjs` holds only the public Google Forms responder URL. An empty URL renders an explicit not-open state with no inputs or submit button. A supported responder URL enables the external form link; it must not contain personal prefills. Preserve all three locale routes and the short homepage festival link.

When changing that URL, verify the nickname requirement, both invitation branches, email validation, private response settings, contact/retention notice and public responder permissions. Use clearly labelled synthetic test data only when authorized to submit a test response. Website tests validate rendering and links; they do not establish that the live Google Form saves data or protects its responses correctly.

## QR entry and printable asset

All three survey pages display a locally generated black-on-white QR beside the responder button, plus a downloadable PNG. It encodes exactly `getSurveyFormUrl()`, including no visitor details or intermediate tracking service. The homepage keeps its compact survey link. The existing live questionnaire and its answer options are unchanged by the separate Boss-planning update.

Sources are `Website/media/survey/survey-qr.svg` and `.png`; `manifest.json` records the public URL, generation settings and SHA-256 hashes. The build checks the URL and hashes before publishing content-addressed copies. A URL change requires regeneration. Keep the four-module white quiet zone, square shape and unaltered dark modules when printing; do not put a logo over the code.

Regeneration uses isolated tooling, not a website/runtime dependency:

```powershell
python -m pip install --target .local-reference/qr-tools qrcode==8.2 zxing-cpp==3.0.0
$env:PYTHONPATH=(Resolve-Path .local-reference/qr-tools).Path
python Website/scripts/prepare-survey-qr.py
python Website/scripts/prepare-survey-qr.py --check
```

Pillow must also be available to Python (already present on the authoring workstation). The generator reads the canonical URL through Node and verifies the PNG with the independent ZXing decoder at its native 636-pixel size and the 260-pixel display size. These are automated decoding checks, not a claim of a physical-phone scan at the venue. Recheck with a phone after printing for the intended display distance.

## Live form verification — October 31, 2026

- The editor showed **Published**, automatic email collection **Do not collect**, account-based response limits off, and response summaries off. The sharing dialog showed one owner, restricted editor access and **Anyone with the link** responder access.
- Preview rejected an empty nickname. Vote-only routing reached the final submit page without an email question; opting in reached the required email section with the 90-day notice and contact control.
- Email validation rejected a deliberately invalid value with the configured three-language error and accepted a synthetic `example.com` address format. Preview mode disabled submission; no synthetic response was submitted.
- The published responder URL opened with the questionnaire and no closed-form message. This browser was signed in as the owner; a separate signed-out interactive session and final response persistence were not exercised. Public access is supported by the inspected sharing and sign-in settings, not a claimed anonymous end-to-end submission.
- These checks cover the survey, not Unity gameplay or a playable open-world release.
