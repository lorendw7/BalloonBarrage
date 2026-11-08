import { sunnyStrings } from "../src/sunny-strings.mjs";
import { harborStrings } from "../src/harbor-strings.mjs";
import { readFileSync } from "node:fs";
export const sunnyProvenance = JSON.parse(readFileSync(new URL("../media/sunny-provenance.json",import.meta.url),"utf8"));
const e = value => String(value).replace(/[&<>"']/g, char => ({ "&":"&amp;", "<":"&lt;", ">":"&gt;", '"':"&quot;", "'":"&#39;" }[char]));
export const sunnyViews = ["market", "meadow", "coral-bay", "overlook"];
export const sunnyIds = ["sunshine-market", "meadow-grove", "coral-bay", "azure-overlook"];
export const harborViews = ["harbor-street", "terrace", "garden-arena", "harbor-bridge"];
const captureNames = {overview:"Overview",topdown:"TopDown",market:"Market",meadow:"Meadow","coral-bay":"CoralBay",bridge:"Bridge",overlook:"Overlook",flowers:"Flowers",grass:"Grass","harbor-overview":"HarborOverview","harbor-street":"HarborStreet",terrace:"Terrace","garden-arena":"GardenArena","harbor-bridge":"HarborBridge"};
export function sunnyCapture(name) {
  const capture=sunnyProvenance.captures.find(item=>item.path.endsWith(`/${captureNames[name]}.png`));
  if(!capture) throw new Error(`Missing native capture: ${name}`);
  return capture;
}
export function sunnyMetricValues(locale) {
  const m=sunnyProvenance.scene.measurements;
  const number=new Intl.NumberFormat(locale,{maximumFractionDigits:0});
  return [`${m.total_land_ratio.toFixed(2)} ×`,`${number.format(m.total_land_m2)} m²`,number.format(sunnyProvenance.asset_counts.combined_expansion_cc0_models)];
}
export function sunnyMapPlaces(locale) {
  const t=sunnyStrings[locale],h=harborStrings[locale];
  const camera=sunnyCapture("topdown");
  if(!camera.orthographic || Math.abs(camera.euler.x-90)>.01 || Math.abs(camera.euler.y)>.01 || Math.abs(camera.euler.z)>.01) throw new Error("Map pins require the recorded north-up orthographic camera");
  const positions=[[0,-79],[-91,-83],[98,-94],[27,-137],[-245,-65]];
  return [...t.places.map((p,i)=>({id:sunnyIds[i],title:p[0],tag:p[1]})),{id:"harbor-borough",title:h.title,tag:h.tag}].map((place,i)=>{
    const [x,z]=positions[i];
    const left=50+(x-camera.position.x)/(2*camera.size*camera.width/camera.height)*100;
    const top=50-(z-camera.position.z)/(2*camera.size)*100;
    if(left<0||left>100||top<0||top>100)throw new Error(`Map landmark lies outside capture: ${place.id}`);
    return {...place,left,top};
  });
}
export function sunnyFigure(locale, base, name, alt, title, { eager=false, wide=false, category="" }={}) {
  const t=sunnyStrings[locale];
  const capture=sunnyCapture(name);
  return `<figure class="sunny-figure${wide ? " sunny-figure-wide" : ""}"${category ? ` data-art-category="${category}"` : ""}><a class="art-open" data-art href="${base}assets/sunny-${name}-20261108.png" aria-label="${e(t.openImage)}: ${e(title)}"><img src="${base}assets/sunny-${name}-20261108.webp" width="${capture.width}" height="${capture.height}" ${eager?'fetchpriority="high"':'loading="lazy"'} decoding="async" alt="${e(alt)}"></a><figcaption><span>${e(title)}</span><span>${e(t.native)}</span></figcaption></figure>`;
}
export function sunnyMetrics(locale) {
  const t=sunnyStrings[locale];
  return `<div class="sunny-metrics-wrap"><dl class="sunny-metrics">${sunnyMetricValues(locale).map((value,i)=>`<div><dt>${e(t.metricLabels[i])}</dt><dd>${e(value)}</dd></div>`).join("")}</dl><p class="sunny-measure-note">${e(t.metricNote)}</p></div>`;
}
export function sunnyHarbor(locale,base,{compact=false}={}) {
  const h=harborStrings[locale];
  return `<section class="sunny-harbor" id="harbor-borough" aria-labelledby="sunny-harbor-title"><div class="sunny-section-heading"><div><p class="eyebrow">05 / ${e(h.tag)}</p><h2 id="sunny-harbor-title">${e(h.title)}</h2></div><p>${e(h.intro)}</p></div>${sunnyFigure(locale,base,"harbor-overview",h.overviewAlt,h.title,{wide:true})}<div class="sunny-harbor-note"><div><h3>${e(h.surfaceTitle)}</h3><p>${e(h.surfaceBody)}</p><a href="${base}assets/sunny-provenance.json">${e(h.sourceLabel)} ↗</a></div><p class="small">${e(h.scope)}</p></div>${compact?"":`<div class="sunny-districts">${h.views.map(([title,tag,body,alt],i)=>`<article class="sunny-district" id="${harborViews[i]}">${sunnyFigure(locale,base,harborViews[i],alt,title)}<div class="sunny-district-copy"><div><p class="eyebrow">${e(tag)}</p><h3>${e(title)}</h3><p>${e(body)}</p></div></div></article>`).join("")}</div>`}</section>`;
}
export function sunnyDistricts(locale,base,{compact=false}={}) {
  const t=sunnyStrings[locale];
  return `<div class="sunny-districts${compact?" sunny-districts-compact":""}">${t.places.map(([title,tag,body,alt],i)=>`<article class="sunny-district" id="${sunnyIds[i]}">${sunnyFigure(locale,base,sunnyViews[i],alt,title)}<div class="sunny-district-copy"><span class="sunny-number" aria-hidden="true">0${i+1}</span><div><p class="eyebrow">${e(tag)}</p><h3>${e(title)}</h3><p>${e(body)}</p></div></div></article>`).join("")}</div>`;
}
export function sunnyPalette(locale) {
  const t=sunnyStrings[locale];
  return `<div class="sunny-palette">${["#65AF50","#27A3B8","#FF7759","#FFD34E","#FFF2CE","#4E9FE8"].map((color,i)=>`<div><span style="--swatch:${color}" aria-hidden="true"></span><strong>${e(t.colors[i])}</strong><code>${color}</code></div>`).join("")}</div>`;
}
export function sunnyFlowers(locale,base) {
  const t=sunnyStrings[locale];
  return `<section class="sunny-flowers" aria-labelledby="sunny-flowers-title">${sunnyFigure(locale,base,"flowers",t.flowerAlt,t.flowerTitle)}<div class="sunny-flowers-copy"><p class="eyebrow">BOTANICAL DETAIL / 3D</p><h2 id="sunny-flowers-title">${e(t.flowerTitle)}</h2><p>${e(t.flowerIntro)}</p><ul class="sunny-flower-types">${t.flowerTypes.map((name,i)=>`<li><span class="sunny-flower-dot sunny-flower-dot-${i}" aria-hidden="true"></span>${e(name)}</li>`).join("")}</ul><p class="small">${e(t.flowerScope)}</p></div></section>`;
}
export function sunnyGrass(locale,base) {
  const t=sunnyStrings[locale];
  return `<section class="sunny-flowers sunny-grass" aria-labelledby="sunny-grass-title">${sunnyFigure(locale,base,"grass",t.grassAlt,t.grassTitle)}<div class="sunny-flowers-copy"><p class="eyebrow">GROUND / TEXTURE / GRASS</p><h2 id="sunny-grass-title">${e(t.grassTitle)}</h2><p>${e(t.grassIntro)}</p><p class="sunny-source-links"><a href="https://ambientcg.com/view?id=Grass005">${e(t.grassSourceLink)} ↗</a><a href="${base}assets/ambientcg-grass-CC0.txt">CC0 · License</a></p><p class="small">${e(t.grassScope)}</p></div></section>`;
}
export function sunnyEvidence(locale) {
  const t=sunnyStrings[locale];
  return `<section class="sunny-evidence" aria-label="${e(t.evidenceTitle)}"><h2>${e(t.evidenceTitle)}</h2><div>${t.evidence.map(([label,body])=>`<article><h3>${e(label)}</h3><p>${e(body)}</p></article>`).join("")}</div></section>`;
}
export function sunnySource(locale,base) {
  const t=sunnyStrings[locale];
  return `<aside class="sunny-source"><div><p class="eyebrow">OPEN ASSETS / CC0</p><h2>${e(t.sourcesTitle)}</h2></div><div><p>${e(t.sourcesBody)}</p><p class="sunny-source-links"><a href="https://kenney.nl/assets/nature-kit">${e(t.licenseLink)} ↗</a><a href="${base}assets/sunny-provenance.json">${e(t.manifestLink)} ↗</a><a href="${base}assets/kenney-nature-license.txt">Nature Kit · License</a><a href="https://kenney.nl/assets/city-kit-suburban">City Kit Suburban ↗</a><a href="${base}assets/kenney-suburban-license.txt">Suburban · License</a><a href="https://kenney.nl/assets/city-kit-commercial">City Kit Commercial ↗</a><a href="${base}assets/kenney-commercial-license.txt">Commercial · License</a><a href="https://polyhaven.com/a/white_plaster_02">Poly Haven · White Plaster 02 ↗</a><a href="https://polyhaven.com/a/roof_tiles">Poly Haven · Roof Tiles ↗</a><a href="${base}assets/polyhaven-surfaces-CC0.txt">Poly Haven · CC0</a></p><p class="small">${e(t.licenseNote)}</p></div></aside>`;
}
