import test from "node:test";
import assert from "node:assert/strict";
import { readFile, stat } from "node:fs/promises";
import path from "node:path";
import { fileURLToPath } from "node:url";
import { spawn } from "node:child_process";
import { languages, strings } from "../src/strings.mjs";
import { selectDownload, repositoryUrl } from "../scripts/releases.mjs";
import { renderPage } from "../scripts/render.mjs";

const site = fileURLToPath(new URL("../", import.meta.url));
const template = await readFile(path.join(site, "src/index.html"), "utf8");
const asset = { name: "BalloonBarrage-Windows-x64.zip", size: 1024, state: "uploaded", browser_download_url: `${repositoryUrl}/releases/download/v1/demo.zip` };
const release = { tag_name: "v1", published_at: "2026-10-08T00:00:00Z", prerelease: false, draft: false, assets: [asset] };

test("all three translations have the same complete, nonempty key set", () => {
  const expected = Object.keys(strings.en).sort();
  for (const locale of languages) {
    assert.deepEqual(Object.keys(strings[locale]).sort(), expected);
    for (const value of Object.values(strings[locale])) assert.ok(value.trim());
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
test("ready, empty and API-failure pages render safely in all languages", () => {
  for (const locale of languages) for (const snapshot of [selectDownload([release]), { available: false, status: "empty" }, { available: false, status: "unknown" }]) {
    const html = renderPage(template, locale, snapshot);
    assert.ok(html.includes(`<html lang="${locale}">`));
    assert.ok(!html.includes("{{"));
    assert.equal((html.match(/aria-current="page"/g) ?? []).length, 1);
    assert.equal(html.includes(asset.browser_download_url), snapshot.available);
    if (!snapshot.available) assert.ok(html.includes(`data-release-status="${snapshot.status}"`));
  }
  const html = renderPage(template, "en", { ...selectDownload([release]), tag: '<script>alert("x")</script>' });
  assert.ok(html.includes("&lt;script&gt;"));
  assert.ok(!html.includes('<script>alert("x")'));
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
    assert.deepEqual(source, outputAsset, "Website artwork must come from the canonical game asset");
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
  assert.equal((await fetch(base + "/%2e%2e%2fassets.json")).status, 403);
  assert.equal((await fetch(base + "/missing.html")).status, 404);
});
