# Website and desktop releases

This document owns the website architecture and its contract with GitHub Releases. Use the [desktop release runbook](PACKAGING_LESSON.md) for local Unity builds, Draft Release staging, physical acceptance and publication. [Validation](VALIDATION.md) records completed checks and package hashes; the [Roadmap](../Design/ROADMAP.md) owns scope and release gates.

## Distribution contract

- GitHub Pages hosts the static Chinese, Japanese and English website.
- GitHub Releases hosts playable desktop ZIPs and their SHA-256 sidecars.
- Windows and macOS assets are selected independently. A source archive is never treated as a playable package.
- Draft releases are staging only and are excluded from the website.
- A missing platform package is shown as unavailable rather than inferred from source code or another platform's asset.
- macOS packages remain labelled unsigned, unnotarized and pending real-hardware verification until that evidence exists.
- Bundled StepMania music remains limited to noncommercial development packages with its original notices and attribution.

The website is a release catalogue, not acceptance evidence. Static checks cannot prove that a Unity player launches, plays audio, handles input or completes a song.

The October 31 demo.14 update publishes the previously verified Windows/macOS candidates and updates all three website languages. A compact home highlight introduces the new version; the game page adds labelled order/ring/chain diagrams and a Kyudaisai dawn/lantern/79 explanation with an optional spoiler disclosure. Capacity/retention copy is 24 targets and twelve seconds after the note. Current verification limits remain explicit, while older screenshots retain their source version labels. [Validation](VALIDATION.md#demo14-publication-and-official-website--october-31) owns exact source/deployment identities and public download evidence.

## Release asset selection

`Website/scripts/releases.mjs` queries this repository's public releases during the site build. It examines up to the latest 100 releases, rejects drafts and empty assets, accepts only trusted HTTPS `releases/download` URLs from this repository, and prefers a stable release over a prerelease. Windows and macOS labelled ZIPs are resolved separately.

The generated pages contain a public, token-free snapshot. Visitors do not call the GitHub API. Read [GitHub Releases](https://github.com/lorendw7/BalloonBarrage/releases) for live package availability; documentation is not a live release listing.

## Publication flow

The default desktop flow is:

1. Build and package Windows x64 and macOS Universal locally from one named commit.
2. Upload ZIPs and checksums to a GitHub Draft Release.
3. Test the exact extracted packages on their target operating systems.
4. Record evidence and known limits.
5. Publish the draft as a prerelease or release.
6. Verify the refreshed website routes and download links.

`.github/workflows/website.yml` listens for published, edited and deleted releases. A release event asks the main-branch workflow to rebuild so Pages retains its main-only environment rules. The website therefore updates after publication without committing ZIPs into the repository or editing download URLs by hand.

The manually dispatched `.github/workflows/desktop-build.yml` is a Windows-only fallback that uploads a private workflow artifact. It does not publish releases. The primary local process and the reason cloud macOS building is disabled are documented in the [desktop release runbook](PACKAGING_LESSON.md); the failed historical cloud attempt remains in [Validation](VALIDATION.md#desktop-delivery-pipeline-checkpoint-october-21).

## Local website build

Use Node.js 22 or newer. No package installation is required:

```powershell
node Website/scripts/build.mjs
node --test Website/tests/site.test.mjs
node Website/scripts/serve.mjs
```

Preview at http://127.0.0.1:4173/. Add `--offline` to the build command when release lookup is unavailable. Offline/API-failure state is unknown, not evidence that no release exists. Generated `Website/dist` remains ignored.

The tests cover translation structure, trusted platform-specific release selection, escaping, canonical/share metadata, media provenance, local assets, content-addressed CSS/JS and preview path safety. They do not test Unity players or sustained browser performance.

## Website content and media

Player-facing copy lives in `Website/src/strings.mjs`; project documentation remains English-only. Each language uses the same page structure and distinguishes published builds from development screenshots, concepts and model previews.

The site provides ten topic pages per language (30 routes), including the October 31 survey preview. Chinese uses the root; Japanese and English use `/ja/` and `/en/`. Each page preserves its topic when switching languages and provides a home link. The home page retains concise game information, actual downloads, topic links and exhibition details; detailed tutorials, galleries and verification notes live on their own pages.

| Route within a language | Content |
|---|---|
| `/` | Game introduction, downloads, topic links and exhibition information |
| `game/` | Controls, musical demonstration/response, song catalog and relay |
| `updates/` | Current delivery priorities and expandable historical verification details |
| `art/` | Filterable scene/prop/artwork gallery and pigment study |
| `survey/` | Future-direction question preview and optional playtest signup policy; Google Form connection pending |
| `open-world/` | Brief art A + C introduction and confirmed gameplay C / A / B cards |
| `open-world/map/` | Labelled original topology diagram and district concepts |
| `open-world/play/` | Exploration/challenge loop, readable troupe rules and expandable candidate roles |
| `open-world/art/` | Sunny street festival with handmade details; night festival deferred |
| `open-world/roadmap/` | Learner-led development gates and deferred scope |

`Website/src/index.html` is the shared site shell; `home.html`, `game.html`, `updates.html`, `art.html` and `survey.html` provide its page bodies through `Website/scripts/templates.mjs`. [Survey operations](FESTIVAL_SURVEY.md) records questions, information handling and the pending Google Form connection. `Website/src/world-strings.mjs` holds public planning translations; `Website/scripts/render-world.mjs` renders its five pages. [Game design](../Design/GAME_DESIGN.md#open-world-direction--planning-only) owns decisions; [Roadmap](../Design/ROADMAP.md#open-world-development-track--planning-only) owns delivery gates. Gameplay C / A / B appears as static cards, while art A + C has a separate compact page. Candidate rival roles are collapsed by default. Exact roles, camera settings, reward conditions and save scope remain open. World-specific styles use content-addressed filenames. No Unity gameplay or desktop package changes are part of this update.

The October 30 update shows the creator-confirmed meeting room (Center Building 2, Room 2407) near the hero and in the exhibition section, keeping demo operating hours unconfirmed. An illustrated three-step explanation introduces the tricolor relay as included in the demo.11 Windows/macOS candidates. Download cards independently discover the actually published package for each platform. It uses CSS swatches and existing fonts without new image downloads or generated media. Existing release selection and screenshot provenance remain unchanged.

`Website/assets.json` selects canonical project artwork and prepared web derivatives. `python Website/scripts/prepare-images.py` regenerates committed WebP derivatives up to the source resolution, capped at 2560 × 1440. Gameplay, results, interface and coverage screenshots use quality 98 for clearer text and paint edges; other artwork uses quality 92. `Website/media/provenance.json` records source/output hashes, settings and attribution. Canonical PNG/JPEG files and exact image-generation prompts remain preserved.

The current gameplay, mode/song selection and demonstration/response figures use the three native 3840 × 2160 PNGs in `Assets/Art/Previews/Festival20261029`, corresponding to demo.8 source. Their provenance identifies the natural-DSP automated Editor fixture and source commit; they are not recordings of the standalone player or human tests. The two-step guide explains noninteractive outlines followed by real balloons. Earlier captures remain version-labelled in the media archive.

The site shows 2560 × 1440 quality-98 WebP derivatives, links to untouched 4K originals and uses `festival-share-20261029.jpg` as its 1200 × 675 share image. Regenerate only these media with:

```powershell
python Website/scripts/prepare-images.py --only festival-selection-20261029 festival-watch-20261029 festival-response-20261029
```

The corresponding provenance records are merged without rewriting unrelated derivatives. Large figures stop at 1600 CSS pixels; the dialog may use most of the viewport. The two-step guide stacks below 800 CSS pixels. Below-the-fold images are lazy-loaded, image dialogs restore keyboard focus, and content-addressed CSS/JS prevents cached layout files from mixing with new pages. Translation checks additionally guard the new guide and delivery fields against Chinese text leaking into Japanese/English pages.

## Verification after publication

After a release or website change:

1. Confirm the Pages workflow succeeds from `main`.
2. Open the live Chinese, Japanese and English routes.
3. Check the displayed version, file size and download URL for each available platform.
4. Confirm unavailable and API-failure states remain truthful.
5. Inspect desktop and narrow layouts, language navigation and the image dialog.
6. Download the public asset and compare its SHA-256 with the release sidecar.

These checks verify publication only. Keep physical Windows/macOS acceptance in [Validation](VALIDATION.md).

## Recovery

- If a package is wrong, return the release to draft or remove the bad asset, then publish a newly versioned accepted package.
- If only the website is wrong, append a fix or revert commit and let the Pages workflow redeploy.
- Keep earlier accepted releases immutable and available as fallbacks.
- Never force-push or rewrite repository history for release recovery.
