# Official website and desktop releases

## Current website direction — November 6, 2026

The trilingual site now leads with the open-world preparation plan while keeping the published studio demo downloads. Event dates, venue details, special-event promotion, voting/registration links, QR assets and all three survey routes are removed. The build recreates only its known generated `Website/dist` directory so stale pages and QR files cannot survive a rebuild. Removed survey URLs return not found; they do not collect information or redirect to the external form.

There are ten topic pages per language (30 routes). The selected daytime handmade music town and later Boss plan remain planning-only. The first next step is a route/camera sketch and small original asset brief, governed by [Game design](../Design/GAME_DESIGN.md#open-world-direction--planning-only) and [Roadmap](../Design/ROADMAP.md#immediate-lesson-queue).

The website retirement does not delete external form replies or change Google Forms settings. Any previously collected invitation emails retain their original 90-day limit; [retention notes](FESTIVAL_SURVEY.md) preserve those obligations. Published desktop packages, historical screenshots, exact image prompts and license attribution remain intact.

## Release discovery

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
node --test Website/tests/*.test.mjs
node Website/scripts/serve.mjs
```

Preview at http://127.0.0.1:4173/. Add `--offline` to the build command when release lookup is unavailable. Offline/API-failure state is unknown, not evidence that no release exists. Generated `Website/dist` remains ignored.

The tests cover translation structure, trusted platform-specific release selection, escaping, canonical/share metadata, media provenance, local assets, content-addressed CSS/JS and preview path safety. They do not test Unity players or sustained browser performance.

## Website content and media

Player-facing copy lives in `Website/src/strings.mjs`; project documentation remains English-only. Each language uses the same page structure and distinguishes published builds from development screenshots, concepts and model previews.

Chinese uses the root; Japanese and English use `/ja/` and `/en/`. Each page preserves its topic when switching languages. The home page keeps a concise introduction, open-world next step, actual downloads and topic links. Detailed tutorials, galleries and verification notes stay on separate pages.

| Route within a language | Content |
|---|---|
| `/` | Game introduction, open-world next step, downloads and topic links |
| `game/` | Controls, musical demonstration/response, song catalog and relay |
| `updates/` | Current delivery priorities and expandable historical verification details |
| `art/` | Filterable scene/prop/artwork gallery and pigment study |
| `open-world/` | An imagined first visit and plain-language explanations of rivals, camera views and musical rewards |
| `open-world/map/` | Labelled original topology diagram and district concepts |
| `open-world/play/` | Exploration/challenge loop, readable troupe rules and expandable candidate roles |
| `open-world/boss/` | Future solo direct combat against a Boss and AI squad; expandable rules, AI research and asset plans |
| `open-world/art/` | Sunny street festival with handmade details; night festival deferred |
| `open-world/roadmap/` | Learner-led development gates and deferred scope |

`Website/src/index.html` is the shared shell; `home.html`, `game.html`, `updates.html` and `art.html` supply its bodies through `Website/scripts/templates.mjs`. `world-strings.mjs` and `world-boss-strings.mjs` hold planning translations; `render-world.mjs` renders six world pages. Rule/technology detail is collapsed by default. Content-addressed CSS/JS keeps layout changes coherent. No Unity gameplay or desktop package change is included in the website retirement.

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
