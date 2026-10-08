import { mkdir, readFile, writeFile, copyFile } from "node:fs/promises";
import { fileURLToPath } from "node:url";
import path from "node:path";
import { languages } from "../src/strings.mjs";
import { getReleaseSnapshot } from "./releases.mjs";
import { renderPage } from "./render.mjs";

const site = fileURLToPath(new URL("../", import.meta.url));
const root = path.resolve(site, "..");
const output = path.join(site, "dist");
await mkdir(path.join(output, "assets"), { recursive: true });
const template = await readFile(path.join(site, "src/index.html"), "utf8");
const release = await getReleaseSnapshot({ offline: process.argv.includes("--offline") });
const assets = JSON.parse(await readFile(path.join(site, "assets.json"), "utf8"));
for (const asset of assets) {
  const source = path.resolve(root, asset.source);
  if (!source.startsWith(root + path.sep) || path.basename(asset.output) !== asset.output) throw new Error("Invalid asset manifest path");
  const data = await readFile(source);
  if (data.subarray(0, 80).toString().includes("git-lfs.github.com/spec")) throw new Error(`Fetch Git LFS assets first: ${asset.source}`);
  if (!data.subarray(0, 8).equals(Buffer.from([137, 80, 78, 71, 13, 10, 26, 10]))) throw new Error(`Expected PNG artwork: ${asset.source}`);
  await copyFile(source, path.join(output, "assets", asset.output));
}
for (const locale of languages) {
  const directory = locale === "zh-Hans" ? output : path.join(output, locale);
  await mkdir(directory, { recursive: true });
  await writeFile(path.join(directory, "index.html"), renderPage(template, locale, release));
}
for (const file of ["styles.css", "app.js", "favicon.svg"]) await copyFile(path.join(site, "src", file), path.join(output, file));
await writeFile(path.join(output, ".nojekyll"), "");
await writeFile(path.join(output, "release.json"), JSON.stringify(release, null, 2) + "\n");
console.log(`Built ${languages.length} language pages using ${assets.length} canonical project artworks. Release status: ${release.status}.`);
