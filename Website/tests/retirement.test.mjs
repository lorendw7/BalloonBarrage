import test from "node:test";
import assert from "node:assert/strict";
import { readFile, readdir, stat } from "node:fs/promises";
import { languages } from "../src/strings.mjs";
import { sitePages } from "../scripts/templates.mjs";
import { worldPages } from "../scripts/render-world.mjs";

test("retired event pages, form links and QR assets are absent from the published output", async () => {
  const output = new URL("../dist/", import.meta.url);
  assert.ok(!(await readdir(output)).some(name => name.startsWith("survey-qr")));
  for (const locale of languages) {
    const prefix = locale === "zh-Hans" ? "" : `${locale}/`;
    await assert.rejects(stat(new URL(`${prefix}survey/index.html`, output)), { code: "ENOENT" });
    for (const page of [...sitePages, ...worldPages.map(page => `open-world/${page}`)]) {
      const html = await readFile(new URL(`${prefix}${page ? `${page.replace(/\/$/, "")}/` : ""}index.html`, output), "utf8");
      assert.doesNotMatch(html, /九大祭|Kyudai-?sai|2407|docs\.google\.com\/forms|survey\/|id="festival"/i);
      assert.doesNotMatch(html, /\{\{/);
    }
    const home = await readFile(new URL(`${prefix}index.html`, output), "utf8");
    assert.match(home, /href="[^"]*open-world\/"/);
    assert.match(home, /id="download"/);
  }
});
