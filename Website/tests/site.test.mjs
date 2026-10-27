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

const site = fileURLToPath(new URL("../", import.meta.url));
const template = await readFile(path.join(site, "src/index.html"), "utf8");
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

test("sharing uses language-specific canonical URLs and the development capture", async () => {
  const base = "https://lorendw7.github.io/BalloonBarrage/";
  for (const locale of languages) {
    const html = renderPage(template, locale, selectDownload([release]));
    const canonical = base + (locale === "zh-Hans" ? "" : `${locale}/`);
    assert.ok(html.includes(`<link rel="canonical" href="${canonical}">`));
    assert.ok(html.includes(`<meta property="og:url" content="${canonical}">`));
    assert.ok(html.includes(`<meta property="og:image" content="${base}assets/gameplay-development-share-20261019.jpg">`));
    assert.ok(!html.includes("v0.1.0-demo.1"), "Introductory copy must not advertise an obsolete package");
  }
  assert.ok((await stat(path.join(site, "media/gameplay-development-20261019.webp"))).size < 300_000,
    "Keep the high-quality first-screen gameplay image below 300 KB");
});

test("web derivatives retain recorded source and output hashes", async () => {
  const records = JSON.parse(await readFile(path.join(site, "media/provenance.json"), "utf8"));
  for (const record of records.filter(({ output }) => output.endsWith(".webp"))) {
    assert.match(record.preparation, /source resolution up to 2560x1440/);
    assert.match(record.preparation, /WebP quality 92/);
  }
  for (const record of records) for (const [file, expected] of [[record.source, record.source_sha256], [record.output, record.output_sha256]]) {
    const data = await readFile(path.resolve(site, "..", file));
    assert.equal(createHash("sha256").update(data).digest("hex"), expected, file);
  }
});
test("built pages have valid local links, scripts, styles and artwork", async () => {
  const output = path.join(site, "dist");
  for (const locale of languages) {
    const directory = locale === "zh-Hans" ? output : path.join(output, locale);
    const html = await readFile(path.join(directory, "index.html"), "utf8");
    for (const [, url] of html.matchAll(/(?:href|src)="([^"]+)"/g)) {
      if (/^(https:|#)/.test(url)) continue;
      assert.ok((await stat(path.resolve(directory, url))).isFile() || (await stat(path.resolve(directory, url))).isDirectory(), url);
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
  for (const [route, locale] of [["/", "zh-Hans"], ["/ja/", "ja"], ["/en/", "en"]]) {
    const response = await fetch(base + route);
    assert.equal(response.status, 200, route);
    assert.ok((await response.text()).includes(`<html lang="${locale}">`));
  }
  assert.equal((await fetch(base + "/styles.css")).headers.get("content-type"), "text/css; charset=utf-8");
  assert.equal((await fetch(base + "/assets/gameplay-development-20261019.webp")).headers.get("content-type"), "image/webp");
  assert.equal((await fetch(base + "/assets/gameplay-development-share-20261019.jpg")).headers.get("content-type"), "image/jpeg");
  assert.equal((await fetch(base + "/assets/surface-paint-coverage-20261027.webp")).headers.get("content-type"), "image/webp");
  assert.equal((await fetch(base + "/%2e%2e%2fassets.json")).status, 403);
  assert.equal((await fetch(base + "/missing.html")).status, 404);
});
