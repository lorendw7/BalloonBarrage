import test from "node:test";
import assert from "node:assert/strict";
import { readFile, stat } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { spawn } from "node:child_process";
import { createHash } from "node:crypto";
import { languages, strings } from "../src/strings.mjs";
import { getReleaseSnapshot, selectDownload, selectDownloads, repositoryUrl } from "../scripts/releases.mjs";
import { renderPage } from "../scripts/render.mjs";
import { renderWorld, worldPages } from "../scripts/render-world.mjs";
import { worldStrings } from "../src/world-strings.mjs";
import { loadTemplate, sitePages } from "../scripts/templates.mjs";

const site = fileURLToPath(new URL("../", import.meta.url));
const template = await loadTemplate();
const gameTemplate = await loadTemplate("game");
const pagePaths = [...sitePages, ...worldPages.map(page => `open-world/${page}`)];
const asset = { name: "BalloonBarrage-Windows-x64.zip", size: 1024, state: "uploaded", browser_download_url: `${repositoryUrl}/releases/download/v1/demo.zip` };
const macAsset = { name: "BalloonBarrage-macOS-Universal.zip", size: 2048, state: "uploaded", browser_download_url: `${repositoryUrl}/releases/download/v1/demo-macos.zip` };
const release = { tag_name: "v1", published_at: "2026-10-08T00:00:00Z", prerelease: false, draft: false, assets: [asset, macAsset] };

test("all three translations have the same complete, nonempty key set", () => {
  const expected = Object.keys(strings.en).sort();
  for (const locale of languages) {
    assert.deepEqual(Object.keys(strings[locale]).sort(), expected);
    for (const value of Object.values(strings[locale])) assert.ok(value.trim());
  }
});

test("festival guidance and delivery copy do not fall back to another language", () => {
  const keys = ["deliveryNowBody", "deliveryNextTitle", "deliveryNextBody", "checkpointBody", "downloadIntro",
    "watchTitle", "watchBody", "responseTitle", "responseBody", "nextBody",
    "sequenceTitle", "sequenceBody", "timingTitle", "timingBody", "burstTitle", "burstBody", "festivalSpecialTitle", "festivalSpecialBody"];
  for (const key of keys) {
    assert.doesNotMatch(strings.en[key], /[\u3400-\u9fff]/u, `English ${key} contains CJK text`);
    assert.match(strings.ja[key], /[\u3040-\u30ff]/u, `Japanese ${key} needs its own Japanese copy`);
    assert.notEqual(strings.ja[key], strings["zh-Hans"][key], `Japanese ${key} duplicates Chinese`);
  }
});

test("demo.15 copy keeps capacity, retention, input recovery and verification scopes explicit", () => {
  for (const locale of languages) {
    assert.ok(strings[locale].deliveryNowTitle.includes("demo.15"));
    for (const value of ["3", "24", "12", "NEXT"]) assert.ok(strings[locale].deliveryNowBody.includes(value));
    assert.ok(strings[locale].factPaint.includes("24"));
    assert.ok(strings[locale].factPaintBody.includes("12"));
    assert.ok(strings[locale].rhythmCaptureLabel.includes("demo.8"), "Preserve the actual screenshot source revision");
    assert.match(strings[locale].checkpointBody, /加速|時刻を進める|accelerated/ui);
    assert.match(strings[locale].deliveryNowBody, /松开|離して|release/u);
    assert.ok(strings[locale].checkpointBody.includes("20"));
    assert.match(strings[locale].checkpointBody, /无界面|画面なし|headless/u);
  }
});

test("audience previews preserve licensed source bytes and require an explicit audio play action", async () => {
  const root = path.resolve(site, "..");
  for (const [code, name] of [["1f44d", "thumb"], ["1f44f", "clap"], ["1f389", "party"], ["1f929", "star"]]) {
    assert.deepEqual(await readFile(path.join(site, "dist/assets", `audience-${name}.png`)),
      await readFile(path.join(root, "Assets/Resources/Audience", `emoji_u${code}.png`)));
  }
  const source = await readFile(path.join(root, "Assets/Resources/Audience/CheerYay.mp3"));
  const hash = createHash("sha256").update(source).digest("hex").slice(0, 12);
  assert.deepEqual(await readFile(path.join(site, "dist", `audience-cheer.${hash}.mp3`)), source);
  for (const name of ["NOTICE.txt", "Apache-2.0.txt", "CC-BY-4.0.txt"]) {
    assert.deepEqual(await readFile(path.join(site, "dist/assets", `audience-${name}`)),
      await readFile(path.join(root, "Assets/StreamingAssets/ThirdParty/Audience", name)));
  }
  for (const locale of languages) {
    const html = await readFile(path.join(site, "dist", locale === "zh-Hans" ? "" : locale, "game/index.html"), "utf8");
    assert.ok(html.includes(strings[locale].audienceTitle));
    assert.match(html, /<audio[^>]*controls[^>]*preload="none"/);
    assert.doesNotMatch(html, /<audio[^>]*\bautoplay/);
    assert.ok(html.includes(`audience-cheer.${hash}.mp3`));
    assert.match(html, /otologic\.jp\/free\/se\/applause-cheer01\.html/);
    assert.match(html, /github\.com\/googlefonts\/noto-emoji/);
  }
});

test("confirmed meeting room and packaged relay stay explicit in every language", () => {
  for (const locale of languages) {
    const html = renderPage(template, locale, selectDownloads([release]));
    const gameHtml = renderPage(gameTemplate, locale, selectDownloads([release]), {}, "game");
    assert.ok(strings[locale].meetingRoom.includes("2407"));
    assert.ok(strings[locale].festivalVenue.includes("2407"));
    assert.ok(strings[locale].relayStatus.includes("demo.15"));
    assert.ok(html.includes(strings[locale].meetingRoom));
    for (const key of ["relayTitle", "relayStatus", "relayRules"])
      assert.ok(gameHtml.includes(strings[locale][key]), `${locale}/${key}`);
    assert.ok(html.indexOf('class="venue-callout"') < html.indexOf('id="game"'));
  }
  assert.match(strings.ja.festivalNote, /集合場所は2407教室/);
  assert.match(strings.en.relayStatus, /Included in Windows \/ macOS/);
  assert.doesNotMatch(strings.en.relayStatus, /not in|development source/);
  assert.match(strings.en.relayRules, /Repeated colors.*preserve progress/);
});

test("all languages explain unrestricted-duration held fire without promising per-frame spawning", () => {
  const required = {
    "zh-Hans": ["按住", "弹药", "换弹", "过热", "有限", "采样"],
    ja: ["長押し", "弾数", "リロード", "オーバーヒート", "一定間隔"],
    en: ["Hold", "ammo", "reload", "overheat", "finite tuned sampling cadence"]
  };
  for (const locale of languages) {
    const copy = `${strings[locale].description} ${strings[locale].gameIntro}`;
    for (const phrase of required[locale]) assert.ok(copy.includes(phrase), `${locale}: ${phrase}`);
  }
});
test("all languages describe the integrated eight-color coverage fixture", () => {
  const required = {
    "zh-Hans": ["八种颜色", "九个", "1000", "十次 Retry"],
    ja: ["8色", "9面", "1,000", "10回連続"],
    en: ["eight pigments", "nine", "1,000", "ten consecutive Retries"]
  };
  for (const locale of languages) {
    const copy = `${strings[locale].updatesIntro} ${strings[locale].sprayBody} ${strings[locale].sprayPreviewLabel} ${strings[locale].sprayRuntimeStatus}`;
    for (const phrase of required[locale]) assert.ok(copy.includes(phrase), `${locale}: ${phrase}`);
  }
});
test("release selection rejects drafts, source-only releases and untrusted downloads", () => {
  assert.equal(selectDownload([{ ...release, draft: true }]).available, false);
  assert.equal(selectDownload([{ ...release, assets: [] }]).available, false);
  assert.equal(selectDownload([{ ...release, assets: [{ ...asset, browser_download_url: "https://example.com/demo.zip" }] }]).available, false);
  assert.equal(selectDownload([{ ...release, assets: [{ ...asset, size: 0 }] }]).available, false);
});
test("stable Windows packages are preferred; prerelease demos remain available", () => {
  const preview = { ...release, tag_name: "v2-alpha", prerelease: true, published_at: "2026-10-09T00:00:00Z" };
  assert.equal(selectDownload([preview, release]).tag, "v1");
  assert.equal(selectDownload([preview]).prerelease, true);
});
test("desktop release selection keeps Windows and macOS assets separate", () => {
  const downloads = selectDownloads([release]);
  assert.equal(downloads.windows.url, asset.browser_download_url);
  assert.equal(downloads.macos.url, macAsset.browser_download_url);
  assert.equal(downloads.windows.tag, "v1");
  assert.equal(downloads.macos.tag, "v1");
});
test("offline release lookup returns explicit unknown states for both platforms", async () => {
  assert.deepEqual(await getReleaseSnapshot({ offline: true }), {
    windows: { available: false, status: "unknown" },
    macos: { available: false, status: "unknown" }
  });
});
test("ready, empty and API-failure pages render safely in all languages", () => {
  const snapshots = [
    selectDownloads([release]),
    {
      windows: { available: false, status: "empty" },
      macos: { available: false, status: "empty" }
    },
    {
      windows: { available: false, status: "unknown" },
      macos: { available: false, status: "unknown" }
    }
  ];
  for (const locale of languages) for (const snapshot of snapshots) {
    const html = renderPage(template, locale, snapshot);
    assert.ok(html.includes(`<html lang="${locale}">`));
    assert.ok(!html.includes("{{"));
    assert.equal((html.match(/aria-current="page"/g) ?? []).length, 1);
    assert.equal(html.includes(asset.browser_download_url), snapshot.windows.available);
    assert.equal(html.includes(macAsset.browser_download_url), snapshot.macos.available);
    if (!snapshot.windows.available) assert.ok(html.includes(`data-release-status="${snapshot.windows.status}"`));
  }
  const selected = selectDownloads([release]);
  const html = renderPage(template, "en", {
    ...selected,
    windows: { ...selected.windows, tag: '<script>alert("x")</script>' }
  });
  assert.ok(html.includes("&lt;script&gt;"));
  assert.ok(!html.includes('<script>alert("x")'));
});

test("sharing uses language-specific canonical URLs and the demo.8 response capture", async () => {
  const base = "https://lorendw7.github.io/BalloonBarrage/";
  for (const locale of languages) {
    const html = renderPage(template, locale, selectDownload([release]));
    const canonical = base + (locale === "zh-Hans" ? "" : `${locale}/`);
    assert.ok(html.includes(`<link rel="canonical" href="${canonical}">`));
    assert.ok(html.includes(`<meta property="og:url" content="${canonical}">`));
    assert.ok(html.includes(`<meta property="og:image" content="${base}assets/festival-share-20261029.jpg">`));
    assert.ok(!html.includes("v0.1.0-demo.1"), "Introductory copy must not advertise an obsolete package");
  }
  assert.ok((await stat(path.join(site, "media/festival-response-20261029.webp"))).size < 800_000,
    "Keep the 2K derivative of native 4K gameplay below 800 KB");
});

test("web derivatives retain recorded source and output hashes", async () => {
  const records = JSON.parse(await readFile(path.join(site, "media/provenance.json"), "utf8"));
  const highClarityScreenshots = new Set([
    "gameplay-official-20261028.webp", "song-selection-20261028.webp", "title-official-20261028.webp",
    "festival-selection-20261029.webp", "festival-watch-20261029.webp", "festival-response-20261029.webp",
    "gameplay-development-20261019.webp",
    "results-statistics-20261023.webp",
    "start-localized-zh-hans-20261025.webp",
    "studio-refresh-20261020.webp",
    "surface-paint-coverage-20261027.webp",
    "studio-expansion-20261028.webp",
    "studio-props-20261028.webp"
  ]);
  for (const record of records.filter(({ output }) => output.endsWith(".webp"))) {
    assert.match(record.preparation, /source resolution up to 2560x1440/);
    const quality = highClarityScreenshots.has(path.basename(record.output)) ? 98 : 92;
    assert.match(record.preparation, new RegExp(`WebP quality ${quality}`));
  }
  for (const record of records) for (const [file, expected] of [[record.source, record.source_sha256], [record.output, record.output_sha256]]) {
    const data = await readFile(path.resolve(site, "..", file));
    assert.equal(createHash("sha256").update(data).digest("hex"), expected, file);
  }
});
test("built pages have valid local links, scripts, styles and artwork", async () => {
  const output = path.join(site, "dist");
  for (const locale of languages) {
    const localeRoot = locale === "zh-Hans" ? output : path.join(output, locale);
    for (const directory of pagePaths.map(page => path.join(localeRoot, page))) {
      const html = await readFile(path.join(directory, "index.html"), "utf8");
      for (const [, url] of html.matchAll(/(?:href|src)="([^"]+)"/g)) {
        if (url.startsWith("#")) {
          assert.ok(html.includes(`id="${url.slice(1)}"`), `${locale}/${url}`);
          continue;
        }
        if (/^https:/.test(url)) continue;
        const [filePath, fragment] = url.split("#");
        const target = path.resolve(directory, filePath);
        const info = await stat(target);
        assert.ok(info.isFile() || info.isDirectory(), url);
        if (fragment) {
          const targetHtml = await readFile(info.isDirectory() ? path.join(target, "index.html") : target, "utf8");
          assert.ok(targetHtml.includes(`id="${fragment}"`), `${directory}: ${url}`);
        }
      }
    }
  }
  const assets = JSON.parse(await readFile(path.join(site, "assets.json"), "utf8"));
  for (const item of assets) {
    const source = await readFile(path.resolve(site, "..", item.source));
    const outputAsset = await readFile(path.join(output, "assets", item.output));
    assert.deepEqual(source, outputAsset, "Website images must match their manifest source");
  }
});

test("new pages bypass legacy cached CSS and JS with content-addressed files", async () => {
  for (const locale of languages) {
    const directory = locale === "zh-Hans" ? path.join(site, "dist") : path.join(site, "dist", locale);
    const html = await readFile(path.join(directory, "index.html"), "utf8");
    for (const file of ["styles.css", "app.js"]) {
      const source = await readFile(path.join(site, "src", file));
      const extension = path.extname(file);
      const hash = createHash("sha256").update(source).digest("hex").slice(0, 12);
      const name = `${path.basename(file, extension)}.${hash}${extension}`;
      assert.ok(html.includes(name), `${locale} must load the matching ${file} version`);
      assert.ok(!html.includes(`/${file}"`), "Never refer new pages to a previously cached stable filename");
      assert.deepEqual(await readFile(path.join(site, "dist", name)), source);
    }
    assert.ok(html.indexOf('id="download"') < html.indexOf('id="art"'), "Keep the demo before the concept gallery");
  }
});

test("preview serves language directory roots and blocks paths outside dist", { timeout: 10000 }, async (t) => {
  const server = spawn(process.execPath, [path.join(site, "scripts/serve.mjs")], {
    env: { ...process.env, PORT: "0" }, stdio: ["ignore", "pipe", "pipe"], windowsHide: true
  });
  t.after(() => server.kill());
  const base = await new Promise((resolve, reject) => {
    let output = "";
    server.on("error", reject);
    server.on("exit", (code) => reject(new Error(`Preview exited before listening: ${code}`)));
    server.stdout.on("data", (chunk) => {
      output += chunk;
      const url = output.match(/http:\/\/127\.0\.0\.1:\d+/);
      if (url) resolve(url[0]);
    });
  });
  for (const locale of languages) for (const page of pagePaths) {
    const route = `/${locale === "zh-Hans" ? "" : `${locale}/`}${page ? `${page.replace(/\/$/, "")}/` : ""}`;
    const response = await fetch(base + route);
    assert.equal(response.status, 200, route);
    assert.ok((await response.text()).includes(`<html lang="${locale}">`));
  }
  assert.equal((await fetch(base + "/styles.css")).headers.get("content-type"), "text/css; charset=utf-8");
  assert.equal((await fetch(base + "/assets/gameplay-official-20261028.webp")).headers.get("content-type"), "image/webp");
  assert.equal((await fetch(base + "/assets/festival-share-20261029.jpg")).headers.get("content-type"), "image/jpeg");
  assert.equal((await fetch(base + "/assets/surface-paint-coverage-20261027.webp")).headers.get("content-type"), "image/webp");
  const game = await (await fetch(base + "/game/")).text();
  const cheerUrl = new URL(game.match(/<source src="([^"]+)" type="audio\/mpeg"/)[1], base + "/game/");
  assert.equal((await fetch(cheerUrl)).headers.get("content-type"), "audio/mpeg");
  assert.equal((await fetch(base + "/%2e%2e%2fassets.json")).status, 403);
  assert.equal((await fetch(base + "/missing.html")).status, 404);
});

test("pagination keeps landing pages compact and preserves localized detail routes", async () => {
  for (const locale of languages) {
    const localeRoot = path.join(site, "dist", locale === "zh-Hans" ? "" : locale);
    const home = await readFile(path.join(localeRoot, "index.html"), "utf8");
    assert.doesNotMatch(home, /id="art-grid"|id="rhythm-title"|id="performance-title"/);
    const world = await readFile(path.join(localeRoot, "open-world/index.html"), "utf8");
    assert.doesNotMatch(world, /id="map-title"|id="loop-title"|id="art-title"|id="plan-title"/);
    for (const page of pagePaths) {
      const html = await readFile(path.join(localeRoot, page, "index.html"), "utf8");
      assert.equal((html.match(/<h1[ >]/g) ?? []).length, 1, `${locale}/${page} needs one main heading`);
      assert.ok(!html.includes("{{"));
      for (const language of languages) {
        const target = `https://lorendw7.github.io/BalloonBarrage/${language === "zh-Hans" ? "" : `${language}/`}${page ? `${page.replace(/\/$/, "")}/` : ""}`;
        const match = html.match(new RegExp(`<link rel="alternate" hreflang="${language}" href="([^"]+)"`));
        assert.ok(match);
        const currentUrl = `https://lorendw7.github.io/BalloonBarrage/${locale === "zh-Hans" ? "" : `${locale}/`}${page ? `${page.replace(/\/$/, "")}/` : ""}`;
        assert.equal(new URL(match[1], currentUrl).href, target);
      }
    }
  }
});

test("world pages preserve locale across nested navigation and keep proposals separate from downloads", async () => {
  const shape = value => Array.isArray(value) ? value.map(shape) : typeof value;
  for (const locale of languages) {
    assert.deepEqual(Object.keys(worldStrings[locale]).sort(), Object.keys(worldStrings.en).sort());
    for (const key of Object.keys(worldStrings.en)) assert.deepEqual(shape(worldStrings[locale][key]), shape(worldStrings.en[key]), `${locale}/${key}`);
    const html = renderWorld(locale);
    const root = locale === "zh-Hans" ? "../" : "../../";
    const prefix = locale === "zh-Hans" ? "" : `${locale}/`;
    assert.ok(html.includes(`href="${root}${prefix}"`), "Return to the same locale home");
    assert.ok(html.includes(`href="https://lorendw7.github.io/BalloonBarrage/${prefix}open-world/"`));
    for (const language of languages) {
      const target = `${root}${language === "zh-Hans" ? "" : `${language}/`}open-world/`;
      assert.ok(html.includes(`href="${target}" lang="${language}"`), "Language switch keeps world subpage");
    }
    assert.equal((html.match(/aria-current="page"/g) ?? []).length, 1);
    assert.doesNotMatch(html, /type="radio"|data-world-choices|\{\{|releases\/download/);
    assert.deepEqual(worldStrings[locale].choices.map(([title]) => title[0]), ["C", "A", "B"]);
    for (const [title, body] of worldStrings[locale].choices) {
      assert.ok(html.includes(title));
      assert.ok(html.includes(body));
    }
    assert.ok(html.includes(worldStrings[locale].status));
    const directory = path.join(site, "dist", prefix, "open-world");
    const built = await readFile(path.join(directory, "index.html"), "utf8");
    for (const file of ["world.css"]) {
      const source = await readFile(path.join(site, "src", file));
      const extension = path.extname(file);
      const hash = createHash("sha256").update(source).digest("hex").slice(0, 12);
      assert.ok(built.includes(`${path.basename(file, extension)}.${hash}${extension}`));
    }
  }
});
