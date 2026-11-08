import test from "node:test";
import assert from "node:assert/strict";
import { readFile, stat } from "node:fs/promises";
import { createHash } from "node:crypto";
import { sunnyStrings } from "../src/sunny-strings.mjs";
import { renderWorld } from "../scripts/render-world.mjs";

const site = new URL("../",import.meta.url);
const hash = bytes => createHash("sha256").update(bytes).digest("hex");

test("sunny translations retain equivalent content and explicit static-world scope", () => {
  const shape = value => Array.isArray(value) ? value.map(shape) : typeof value;
  for(const [locale,copy] of Object.entries(sunnyStrings)) {
    assert.deepEqual(Object.keys(copy),Object.keys(sunnyStrings.en));
    for(const key of Object.keys(copy)) assert.deepEqual(shape(copy[key]),shape(sunnyStrings.en[key]),`${locale}/${key}`);
    const map=renderWorld(locale,{},"map");
    assert.ok(map.includes(copy.native));
    assert.ok(map.includes("4.55 ×"));
    assert.equal((map.match(/class="sunny-map-pin sunny-map-pin-/g)??[]).length,4);
    for(const page of ["play","boss"]) assert.ok(renderWorld(locale,{},page).includes(copy.planNote));
    for(const page of ["","map","art","roadmap"]) {
      const html=renderWorld(locale,{},page);
      assert.match(html,/<dialog class="art-dialog"/);
      assert.match(html,/script type="module"/);
      assert.doesNotMatch(html,/releases\/download/);
    }
  }
});

test("published native captures are complete originals with matching provenance", async () => {
  const provenance=JSON.parse(await readFile(new URL("media/sunny-provenance.json",site),"utf8"));
  assert.equal(provenance.scene.measurements.total_land_m2,34482.5);
  assert.equal(provenance.scene.measurements.added_land_m2+provenance.scene.measurements.original_land_m2,34482.5);
  assert.match(provenance.evidence_scope,/Not gameplay/);
  const views={Overview:"overview",TopDown:"topdown",Market:"market",Meadow:"meadow",CoralBay:"coral-bay",Bridge:"bridge",Overlook:"overlook"};
  for(const capture of provenance.captures) {
    const originalName=capture.path.split("/").pop().replace(/\.png$/,"");
    const name=`sunny-${views[originalName]}-20261108.png`;
    const bytes=await readFile(new URL(`media/${name}`,site));
    assert.equal(hash(bytes),capture.sha256,name);
    assert.deepEqual(await readFile(new URL(`dist/assets/${name}`,site)),bytes,name);
    assert.ok((await stat(new URL(`media/${name.replace(/\.png$/,".webp")}`,site))).size<500000,"Native scene thumbnails should remain below 500 KB");
  }
  assert.equal(provenance.captures.length,7);
  for(const name of ["sunny-provenance.json","kenney-nature-license.txt"])
    assert.deepEqual(await readFile(new URL(`dist/assets/${name}`,site)),await readFile(new URL(`media/${name}`,site)));
  assert.match(await readFile(new URL("media/kenney-nature-license.txt",site),"utf8"),/CC0/);
  const originalLicense=provenance.new_cc0_sources.files.find(file=>file.path==="License.txt");
  assert.equal(hash(await readFile(new URL("media/kenney-nature-license.txt",site))),originalLicense.sha256,"Original CC0 license bytes must survive checkout and publication");
});

test("sunny media needed for publication is contained in the website checkout",async()=>{
  const assets=JSON.parse(await readFile(new URL("assets.json",site),"utf8"));
  const media=JSON.parse(await readFile(new URL("media/provenance.json",site),"utf8"));
  const outputs=assets.filter(a=>a.output.startsWith("sunny-"));
  assert.equal(outputs.length,15);
  for(const asset of outputs) assert.match(asset.source,/^Website\/media\//);
  for(const record of media.filter(m=>m.output.includes("/sunny-"))) {
    assert.match(record.source,/^Website\/media\//);
    assert.match(record.attribution,/static scene/);
  }
});
