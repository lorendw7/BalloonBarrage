# Official website and desktop releases

## Current website direction — November 8, 2026

The trilingual site leads with native views of the saved Sunny Coast environment: a larger southern island, four new places, two bridges and a brighter turquoise, coral, sunflower and ivory palette. The home page, map, art gallery and development pages share those images while retaining the published studio demo downloads. Open-world movement, song entry, rivals and Boss combat remain future gameplay. [Game design](../Design/GAME_DESIGN.md#open-world-direction--planning-only) owns the experience and [Roadmap](../Design/ROADMAP.md#immediate-lesson-queue) owns the next learner-written increment.

There are ten topic pages per language (30 routes). The actual top-down capture replaces the earlier topology sketch and provides four numbered destination links. Native image dialogs work with keyboard focus restoration; ordinary image links remain usable without JavaScript. Shared content, language navigation, release discovery, source attribution and retirement checks remain intact.

Nine complete PNG captures, including close-ups of flowers and grass, are copied byte-for-byte into `Website/media`, alongside bounded WebP derivatives and a separate social image. The website is independently deployable without publishing the local Unity art working tree. `sunny-provenance.json` records exact capture hashes, static area and route evidence, both expansion CC0 model manifests, the ambientCG grass-source manifest and the original flower/grass geometry provenance. The original Kenney `License.txt` and grass CC0 legal text are also published. Public source buttons lead to these website assets and the official publishers.

The planting update adds shared grass and flower features to the home page and both art pages, and makes the grass close-up the lead image on Updates. Its translated copy distinguishes the two CC0 expansion subsets (16 + 20 models) from four original procedural 3D flower models: daisies, coral tulips, bluebells and sunflowers. The latter are native mesh geometry, not generated raster artwork or a flower-growth mechanic. Counts describe distinct source models, not scene placements. Earlier source and artwork records remain separate.

Ground material uses ambientCG Grass005, which the provider describes as **Procedural with Bitmap Elements**. Website copy describes textured or photographic-style PBR turf, not photogrammetry. Original narrow 3D grass blades supplement that surface. The exact deterministic geometry specification and native scene record are included in public provenance; spatial LOD and geometry inventories do not establish frame-rate or wind-simulation acceptance. Grass textures and original blade meshes are separate from the 36 imported model count.

Event promotion, form links, QR assets and survey routes remain retired. The build recreates only its known generated `Website/dist` directory, so stale files cannot survive a rebuild. Removed survey URLs return not found.

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

The 25 website checks cover translation structure, trusted platform-specific release selection, escaping, canonical/share metadata, media provenance, complete native capture copies, self-contained Sunny Coast publication assets, local links, content-addressed CSS/JS and preview path safety. The planting update additionally checks that CC0 model/grass inventories remain separate from original native geometry, preserve source technique/license text and identify the same final scene revision. They do not test Unity players or sustained browser performance. The November 8 grass-and-flower build produced 30 pages and 87 image assets; all 25 checks passed.

The initial Sunny Coast browser inspection on November 8 covered the 1280 px desktop and 390 px mobile home page, with no horizontal overflow or broken loaded images. Mobile map links reached their matching destinations. The image dialog opened, closed with Escape and restored keyboard focus. Switching Chinese → English → Japanese preserved the map subpage. No browser console errors were observed. These checks verify that local website revision, separately from the later planting update, publication and Unity acceptance.

The final grass-and-flower revision was also inspected at 1280 × 900 and 390 × 844. Its new grass feature fitted both layouts, all loaded image URLs resolved, and the native-image dialog opened and closed with Escape while restoring focus to the originating link. No console errors were observed. A subsequent online build resolved both existing Windows and macOS release entries as ready; this is website metadata verification, not platform playtesting.

## Website content and media

Player-facing copy lives in `Website/src/strings.mjs`; project documentation remains English-only. Each language uses the same page structure and distinguishes published builds from development screenshots, concepts and model previews.

Chinese uses the root; Japanese and English use `/ja/` and `/en/`. Each page preserves its topic when switching languages. The home page keeps a concise introduction, open-world next step, actual downloads and topic links. Detailed tutorials, galleries and verification notes stay on separate pages.

| Route within a language | Content |
|---|---|
| `/` | Native coast hero, measured static-map summary, four district previews, studio downloads and topic links |
| `game/` | Controls, musical demonstration/response, song catalog and relay |
| `updates/` | Current static expansion evidence, studio delivery priorities and historical verification details |
| `art/` | Native coast gallery, open-asset provenance, existing filterable studio archive and pigment study |
| `open-world/` | Saved scene overview, new places, evidence boundary and future play ideas |
| `open-world/map/` | Native top-down capture, numbered destination links, new and original districts |
| `open-world/play/` | Exploration/challenge loop, readable troupe rules and expandable candidate roles |
| `open-world/boss/` | Future solo direct combat against a Boss and AI squad; expandable rules, AI research and asset plans |
| `open-world/art/` | Bright palette, native district views, source attribution and material direction |
| `open-world/roadmap/` | Saved environment baseline, learner-led development gates and deferred scope |

`Website/src/index.html` is the shared shell; `home.html`, `game.html`, `updates.html` and `art.html` supply its bodies through `Website/scripts/templates.mjs`. `world-strings.mjs` and `world-boss-strings.mjs` distinguish saved scenery from future gameplay; `render-world.mjs` renders six world pages. `sunny-strings.mjs` and `render-sunny.mjs` share localized district, evidence, palette and licensing sections. Rule/technology detail is collapsed by default. Content-addressed CSS/JS keeps layout changes coherent. Website publication does not alter Unity gameplay or desktop packages.

`Website/assets.json` selects canonical project artwork and prepared web derivatives. `python Website/scripts/prepare-images.py` regenerates committed WebP derivatives up to the source resolution, capped at 2560 × 1440. Gameplay, results, interface and coverage screenshots use quality 98 for clearer text and paint edges; other artwork uses quality 92. `Website/media/provenance.json` records source/output hashes, settings and attribution. Canonical PNG/JPEG files and exact image-generation prompts remain preserved.

The current gameplay, mode/song selection and demonstration/response figures use the three native 3840 × 2160 PNGs in `Assets/Art/Previews/Festival20261029`, corresponding to demo.8 source. Their provenance identifies the natural-DSP automated Editor fixture and source commit; they are not recordings of the standalone player or human tests. The two-step guide explains noninteractive outlines followed by real balloons. Earlier captures remain version-labelled in the media archive.

The gameplay page shows 2560 × 1440 quality-98 WebP derivatives, links to untouched 4K originals and retains `festival-share-20261029.jpg` as its 1200 × 675 share image. Other pages use `sunny-share-20261108.jpg`, fitting the complete native overview into 1200 × 675 with ivory side padding. Sunny Coast WebPs use quality 92, with a 500 KB per-image budget checked by the website suite. Regenerate the studio media with:

```powershell
python Website/scripts/prepare-images.py --only festival-selection-20261029 festival-watch-20261029 festival-response-20261029
```

The corresponding provenance records are merged without rewriting unrelated derivatives. Large figures stop at 1600 CSS pixels; the dialog may use most of the viewport. The two-step guide stacks below 800 CSS pixels. Below-the-fold images are lazy-loaded, image dialogs restore keyboard focus, and content-addressed CSS/JS prevents cached layout files from mixing with new pages. Translation checks additionally guard the new guide and delivery fields against Chinese text leaking into Japanese/English pages.

Regenerate the coast media only after the native captures are finalized:

```powershell
python Website/scripts/refresh-sunny-provenance.py
python Website/scripts/prepare-images.py --only sunny-overview-20261108 sunny-topdown-20261108 sunny-market-20261108 sunny-meadow-20261108 sunny-coral-bay-20261108 sunny-bridge-20261108 sunny-overlook-20261108 sunny-flowers-20261108 sunny-grass-20261108
```

The provenance refresher requires matching scene/detail/grass hashes, checks native captures, original flower mesh/prefabs and grass-authoring source, and validates the raw CC0 model/grass inventories before replacing the website snapshot. Measurements and probe counts come from native evidence files. The media tool then copies complete PNG originals into the website and updates their WebP/share derivatives and recorded hashes. The build uses those committed website files, not the local Unity scene. Do not combine measurements or captures from different scene revisions.

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
