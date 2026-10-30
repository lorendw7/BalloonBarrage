import { mkdir, readFile, writeFile, copyFile } from "node:fs/promises";
import { fileURLToPath } from "node:url";
import path from "node:path";
import { createHash } from "node:crypto";
import { languages } from "../src/strings.mjs";
import { getReleaseSnapshot } from "./releases.mjs";
import { renderPage } from "./render.mjs";
import { renderWorld } from "./render-world.mjs";

const site = fileURLToPath(new URL("../", import.meta.url));
const root = path.resolve(site, "..");
const output = path.join(site, "dist");
await mkdir(path.join(output, "assets"), { recursive: true });
const template = await readFile(path.join(site, "src/index.html"), "utf8");
// New pages must never reuse a cached stylesheet from an earlier layout.
const staticFiles = {};
for (const file of ["styles.css", "app.js", "world.css", "world.js"]) {
  const data = await readFile(path.join(site, "src", file));
  const extension = path.extname(file);
  const hash = createHash("sha256").update(data).digest("hex").slice(0, 12);
  const name = `${path.basename(file, extension)}.${hash}${extension}`;
  staticFiles[file] = name;
  await writeFile(path.join(output, name), data);
}
const release = await getReleaseSnapshot({ offline: process.argv.includes("--offline") });
const assets = JSON.parse(await readFile(path.join(site, "assets.json"), "utf8"));
for (const asset of assets) {
  const source = path.resolve(root, asset.source);
  if (!source.startsWith(root + path.sep) || path.basename(asset.output) !== asset.output) throw new Error("Invalid asset manifest path");
  const data = await readFile(source);
  if (data.subarray(0, 80).toString().includes("git-lfs.github.com/spec")) throw new Error(`Fetch Git LFS assets first: ${asset.source}`);
  const png = data.subarray(0, 8).equals(Buffer.from([137, 80, 78, 71, 13, 10, 26, 10]));
  const webp = data.toString("ascii", 0, 4) === "RIFF" && data.toString("ascii", 8, 12) === "WEBP";
  const jpeg = data[0] === 255 && data[1] === 216 && data[2] === 255;
  const valid = { ".png": png, ".webp": webp, ".jpg": jpeg };
  if (!valid[path.extname(asset.source)] || !valid[path.extname(asset.output)]) throw new Error(`Invalid artwork format: ${asset.source}`);
  await copyFile(source, path.join(output, "assets", asset.output));
}
for (const locale of languages) {
  const directory = locale === "zh-Hans" ? output : path.join(output, locale);
  await mkdir(directory, { recursive: true });
  await writeFile(path.join(directory, "index.html"), renderPage(template, locale, release, staticFiles));
  await mkdir(path.join(directory, "open-world"), { recursive: true });
  await writeFile(path.join(directory, "open-world/index.html"), renderWorld(locale, staticFiles));
}
for (const file of ["styles.css", "app.js", "favicon.svg"]) await copyFile(path.join(site, "src", file), path.join(output, file));
await writeFile(path.join(output, ".nojekyll"), "");
await writeFile(path.join(output, "release.json"), JSON.stringify(release, null, 2) + "\n");
console.log(
  `Built ${languages.length * 2} language pages using ${assets.length} project images. ` +
  `Release status: Windows=${release.windows.status}, macOS=${release.macos.status}.`
);
