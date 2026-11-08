import test from "node:test";
import assert from "node:assert/strict";
import { readFile, stat } from "node:fs/promises";
import { createHash } from "node:crypto";
import { sunnyStrings } from "../src/sunny-strings.mjs";
import { harborStrings } from "../src/harbor-strings.mjs";
import { renderWorld } from "../scripts/render-world.mjs";
import { sunnyMetricValues, sunnyMapPlaces, sunnyCapture } from "../scripts/render-sunny.mjs";

const site = new URL("../",import.meta.url);
const hash = bytes => createHash("sha256").update(bytes).digest("hex");

test("sunny translations retain equivalent content and explicit static-world scope", () => {
  const shape = value => Array.isArray(value) ? value.map(shape) : typeof value;
  for(const [locale,copy] of Object.entries(sunnyStrings)) {
    assert.deepEqual(Object.keys(copy),Object.keys(sunnyStrings.en));
    for(const key of Object.keys(copy)) assert.deepEqual(shape(copy[key]),shape(sunnyStrings.en[key]),`${locale}/${key}`);
    const map=renderWorld(locale,{},"map");
    assert.ok(map.includes(copy.native));
    for(const value of sunnyMetricValues(locale)) assert.ok(map.includes(value));
    assert.equal((map.match(/class="sunny-map-pin sunny-map-pin-/g)??[]).length,5);
    assert.deepEqual(Object.keys(harborStrings[locale]),Object.keys(harborStrings.en));
    for(const key of Object.keys(harborStrings.en)) assert.deepEqual(shape(harborStrings[locale][key]),shape(harborStrings.en[key]),`${locale}/harbor/${key}`);
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
  const m=provenance.scene.measurements;
  assert.equal(m.added_land_m2+m.original_land_m2,m.total_land_m2);
  assert.equal(m.southern_expansion_land_m2+m.western_harbor_land_m2,m.added_land_m2);
  assert.equal(m.total_land_m2,provenance.harbor_expansion.source_manifest.totalLandSquareMetres);
  assert.ok(Math.abs(m.total_land_ratio-m.total_land_m2/m.original_land_m2)<.00001);
  assert.match(provenance.evidence_scope,/Not gameplay/);
  const views={Overview:"overview",TopDown:"topdown",Market:"market",Meadow:"meadow",CoralBay:"coral-bay",Bridge:"bridge",Overlook:"overlook",Flowers:"flowers",Grass:"grass",HarborOverview:"harbor-overview",HarborStreet:"harbor-street",Terrace:"terrace",GardenArena:"garden-arena",HarborBridge:"harbor-bridge",Ridgeline:"ridgeline",CourtyardAlley:"courtyard-alley",FlankingCover:"flanking-cover"};
  for(const capture of provenance.captures) {
    const originalName=capture.path.split("/").pop().replace(/\.png$/,"");
    const name=`sunny-${views[originalName]}-20261108.png`;
    const bytes=await readFile(new URL(`media/${name}`,site));
    assert.equal(hash(bytes),capture.sha256,name);
    assert.deepEqual(await readFile(new URL(`dist/assets/${name}`,site)),bytes,name);
    assert.ok((await stat(new URL(`media/${name.replace(/\.png$/,".webp")}`,site))).size<500000,"Native scene thumbnails should remain below 500 KB");
  }
  assert.equal(provenance.captures.length,17);
  assert.equal(new Set(provenance.captures.map(c=>c.path.split("/").pop())).size,17);
  for(const name of ["sunny-provenance.json","kenney-nature-license.txt","ambientcg-grass-CC0.txt","kenney-suburban-license.txt","kenney-commercial-license.txt","polyhaven-surfaces-CC0.txt"])
    assert.deepEqual(await readFile(new URL(`dist/assets/${name}`,site)),await readFile(new URL(`media/${name}`,site)));
  assert.match(await readFile(new URL("media/kenney-nature-license.txt",site),"utf8"),/CC0/);
  for(const source of [provenance.new_cc0_sources,provenance.coastal_detail_sources]) {
    const originalLicense=source.files.find(file=>file.path==="License.txt");
    assert.equal(hash(await readFile(new URL("media/kenney-nature-license.txt",site))),originalLicense.sha256,"Original CC0 license bytes must survive checkout and publication");
  }
});

test("sunny media needed for publication is contained in the website checkout",async()=>{
  const assets=JSON.parse(await readFile(new URL("assets.json",site),"utf8"));
  const media=JSON.parse(await readFile(new URL("media/provenance.json",site),"utf8"));
  const outputs=assets.filter(a=>a.output.startsWith("sunny-"));
  assert.equal(outputs.length,35);
  for(const asset of outputs) assert.match(asset.source,/^Website\/media\//);
  for(const record of media.filter(m=>m.output.includes("/sunny-"))) {
    assert.match(record.source,/^Website\/media\//);
    assert.match(record.attribution,/static scene/);
  }
});

test("flower update separates imported CC0 models from original native geometry",async()=>{
  const provenance=JSON.parse(await readFile(new URL("media/sunny-provenance.json",site),"utf8"));
  const baseModels=provenance.new_cc0_sources.files.filter(file=>file.path.endsWith(".obj"));
  const detailModels=provenance.coastal_detail_sources.files.filter(file=>file.path.endsWith(".obj"));
  const buildings=provenance.harbor_building_sources.files.filter(file=>file.path.endsWith(".obj"));
  const allSources=[...baseModels,...detailModels,...buildings].map(file=>file.path);
  assert.equal(new Set(allSources).size,allSources.length,"Expansion subsets must not double-count the same model");
  for(const locale of Object.keys(sunnyStrings)) assert.equal(Number(sunnyMetricValues(locale)[2]),allSources.length);
  assert.equal(provenance.original_flower_models.count,4);
  assert.match(provenance.original_flower_models.authoring,/procedural 3D/);
  assert.match(provenance.original_flower_models.scope,/not raster/);
  assert.equal(provenance.refinement.scene_sha256,provenance.scene.sha256,"Refinement and captures must identify the same saved scene");
  for(const locale of Object.keys(sunnyStrings)) {
    const prefix=locale==="zh-Hans"?"":`${locale}/`;
    for(const route of ["","art/","open-world/art/"]) {
      const html=await readFile(new URL(`dist/${prefix}${route}index.html`,site),"utf8");
      assert.ok(html.includes("sunny-flowers-20261108.webp"),`${locale}/${route}`);
      assert.ok(html.includes(sunnyStrings[locale].native));
    }
  }
});

test("grass evidence preserves its CC0 source technique and original geometry recipe",async()=>{
  const provenance=JSON.parse(await readFile(new URL("media/sunny-provenance.json",site),"utf8"));
  const source=provenance.grass_surface_sources;
  assert.equal(source.assetId,"Grass005");
  assert.equal(source.license,"CC0-1.0");
  assert.match(source.sourceTechnique,/Procedural with Bitmap Elements/);
  const license=source.files.find(file=>file.path==="CC0-1.0.txt");
  assert.equal(hash(await readFile(new URL("media/ambientcg-grass-CC0.txt",site))),license.sha256);
  assert.equal(provenance.grass_refinement.scene_sha256,provenance.scene.sha256);
  assert.ok(provenance.grass_refinement.creative_specification.length>100);
  assert.match(provenance.grass_refinement.original_geometry,/C#/);
  assert.match(provenance.grass_refinement.evidence_scope,/not runtime performance acceptance/);
  for(const locale of Object.keys(sunnyStrings)) {
    const prefix=locale==="zh-Hans"?"":`${locale}/`;
    for(const route of ["","art/","open-world/art/"]) {
      const html=await readFile(new URL(`dist/${prefix}${route}index.html`,site),"utf8");
      assert.ok(html.includes("sunny-grass-20261108.webp"),`${locale}/${route}`);
      assert.ok(html.includes(sunnyStrings[locale].native));
    }
  }
});

test("harbor sources, surface coverage and combat layout retain separate evidence",async()=>{
  const p=JSON.parse(await readFile(new URL("media/sunny-provenance.json",site),"utf8"));
  const buildings=p.harbor_building_sources;
  assert.equal(buildings.license,"CC0-1.0");
  assert.equal(buildings.models.length,p.asset_counts.harbor_cc0_building_models);
  assert.equal(p.prepared_harbor_buildings.source_manifest.assets.length,buildings.models.length);
  for(const [name,file] of [["kenney-suburban-license.txt","Suburban/License.txt"],["kenney-commercial-license.txt","Commercial/License.txt"]]) {
    const original=buildings.files.find(record=>record.path===file);
    assert.equal(hash(await readFile(new URL(`media/${name}`,site))),original.sha256);
  }
  const original=p.town_surface_sources.files.find(record=>record.path==="CC0-1.0.txt");
  assert.equal(hash(await readFile(new URL("media/polyhaven-surfaces-CC0.txt",site))),original.sha256);
  for(const evidence of [p.harbor_expansion,p.terrain_refinement,p.textured_town]) assert.equal(evidence.scene_sha256,p.scene.sha256);
  assert.match(p.harbor_expansion.evidence_scope,/no combat, enemy AI, movement controller/);
  const texture=p.textured_town.source_manifest;
  assert.equal(texture.coverage.missingTextureSlots,0);
  assert.equal(texture.coverage.texturedSlots,texture.coverage.materialSlots);
  assert.ok(texture.coverage.materialSlots>0);
  assert.equal(texture.coverage.slots.length,texture.coverage.materialSlots);
  assert.ok(texture.textures.some(item=>/Original deterministic/.test(item.origin)));
  assert.ok(texture.textures.some(item=>/CC0 Poly Haven/.test(item.origin)));
  for(const item of texture.textures.filter(item=>/Original deterministic/.test(item.origin))) {
    assert.deepEqual(item.sourcePaths,[texture.originalRecipeSource.path]);
    assert.deepEqual(item.sourceSha256,[texture.originalRecipeSource.sha256],"Original surfaces must retain the exact generation-time recipe snapshot");
  }
  assert.equal(p.textured_town.creative_specification,texture.creativeSpecification);
  for(const locale of Object.keys(harborStrings)) {
    const prefix=locale==="zh-Hans"?"":`${locale}/`;
    for(const route of ["","art/","open-world/","open-world/map/","open-world/art/"]) {
      const html=await readFile(new URL(`dist/${prefix}${route}index.html`,site),"utf8");
      assert.ok(html.includes("sunny-harbor-overview-20261108.webp"),`${locale}/${route}`);
      assert.ok(html.includes(harborStrings[locale].scope),`${locale}/${route}/scope`);
      if(route!==""&&route!=="open-world/")for(const view of ["harbor-street","terrace","garden-arena","harbor-bridge"])assert.ok(html.includes(`sunny-${view}-20261108.webp`));
    }
    assert.ok((await readFile(new URL(`dist/${prefix}updates/index.html`,site),"utf8")).includes("sunny-ridgeline-20261108.webp"));
  }
});

test("terrain refinement keeps native evidence separate from implemented movement",async()=>{
  const p=JSON.parse(await readFile(new URL("media/sunny-provenance.json",site),"utf8"));
  const terrain=p.terrain_refinement;
  assert.equal(terrain.scene_sha256,p.scene.sha256);
  assert.match(terrain.source_manifest_sha256,/^[a-f0-9]{64}$/);
  assert.equal(terrain.source_manifest.sceneSha256,p.scene.sha256);
  assert.match(terrain.evidence_scope,/does not implement or accept character movement, combat, enemy AI/);
  const native=terrain.source_manifest;
  assert.equal(terrain.creative_specification,native.creativeSpecification);
  assert.equal(native.landSquareMetres,p.scene.measurements.total_land_m2);
  assert.ok(Math.abs(native.highestGround-native.lowestGround-native.verticalRange)<.0001);
  assert.equal(native.alleys.length,native.alleyRoutes);
  assert.equal(native.coverBypasses.length,native.coverBypassRoutes);
  assert.equal(native.mainSupportSamples+native.alleySupportSamples+native.coverSupportSamples,native.clearanceSamples);
  assert.equal(p.scene.harbor_support_probes,native.clearanceSamples);
  const mainMetres=native.mainRoutes.reduce((sum,route)=>sum+route.lengthMetres,0);
  const alleyMetres=native.alleys.reduce((sum,route)=>sum+route.lengthMetres,0);
  const bypassMetres=native.coverBypasses.reduce((sum,route)=>sum+route.lengthMetres,0);
  assert.equal(p.scene.measurements.harbor_main_route_metres,mainMetres);
  assert.equal(p.scene.measurements.harbor_alley_metres,alleyMetres);
  assert.equal(p.scene.measurements.harbor_cover_bypass_metres,bypassMetres);
  for(const locale of Object.keys(harborStrings)) {
    const prefix=locale==="zh-Hans"?"":`${locale}/`;
    for(const route of ["art/","open-world/map/","open-world/art/"]) {
      const html=await readFile(new URL(`dist/${prefix}${route}index.html`,site),"utf8");
      for(const view of ["ridgeline","courtyard-alley","flanking-cover"])assert.ok(html.includes(`sunny-${view}-20261108.webp`),`${locale}/${route}/${view}`);
      assert.ok(html.includes(harborStrings[locale].terrainIntro));
    }
  }

  assert.match(harborStrings.en.scope,/character movement, combat, enemy AI and destruction are not implemented/);
  assert.match(harborStrings["zh-Hans"].scope,/尚未实现角色移动、战斗、敌人 AI/);
  assert.match(harborStrings.ja.scope,/キャラクター移動、戦闘、敵AI、破壊は未実装/);
});

test("map links use the final north-up camera and all five destinations",()=>{
  const camera=sunnyCapture("topdown");
  assert.equal(camera.orthographic,true);
  for(const locale of Object.keys(sunnyStrings)) {
    const html=renderWorld(locale,{},"map");
    const points=sunnyMapPlaces(locale);
    assert.equal(points.length,5);
    for(const point of points) {
      assert.ok(point.left>0&&point.left<100&&point.top>0&&point.top<100);
      assert.ok(html.includes(`id="${point.id}"`),point.id);
      assert.ok(html.includes(`left:${point.left.toFixed(3)}%;top:${point.top.toFixed(3)}%`),point.id);
    }
    const market=points[0];
    const x=camera.position.x+(market.left/100-.5)*camera.size*2*camera.width/camera.height;
    const z=camera.position.z-(market.top/100-.5)*camera.size*2;
    assert.ok(Math.abs(x)<.0001&&Math.abs(z+79)<.0001,"Market pin must project to its scene landmark, independent of camera reframing");
  }
});
