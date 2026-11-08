import { sunnyStrings } from "../src/sunny-strings.mjs";
const e = value => String(value).replace(/[&<>"']/g, char => ({ "&":"&amp;", "<":"&lt;", ">":"&gt;", '"':"&quot;", "'":"&#39;" }[char]));
export const sunnyViews = ["market", "meadow", "coral-bay", "overlook"];
export const sunnyIds = ["sunshine-market", "meadow-grove", "coral-bay", "azure-overlook"];
export function sunnyFigure(locale, base, name, alt, title, { eager=false, wide=false, category="" }={}) {
  const t=sunnyStrings[locale];
  const overview=name==="overview"||name==="topdown";
  return `<figure class="sunny-figure${wide ? " sunny-figure-wide" : ""}"${category ? ` data-art-category="${category}"` : ""}><a class="art-open" data-art href="${base}assets/sunny-${name}-20261108.png" aria-label="${e(t.openImage)}: ${e(title)}"><img src="${base}assets/sunny-${name}-20261108.webp" width="${overview?2160:1920}" height="${overview?1440:1080}" ${eager?'fetchpriority="high"':'loading="lazy"'} decoding="async" alt="${e(alt)}"></a><figcaption><span>${e(title)}</span><span>${e(t.native)}</span></figcaption></figure>`;
}
export function sunnyMetrics(locale) {
  const t=sunnyStrings[locale];
  return `<div class="sunny-metrics-wrap"><dl class="sunny-metrics">${t.metricValues.map((value,i)=>`<div><dt>${e(t.metricLabels[i])}</dt><dd>${e(value)}</dd></div>`).join("")}</dl><p class="sunny-measure-note">${e(t.metricNote)}</p></div>`;
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
  return `<aside class="sunny-source"><div><p class="eyebrow">OPEN ASSETS / CC0</p><h2>${e(t.sourcesTitle)}</h2></div><div><p>${e(t.sourcesBody)}</p><p class="sunny-source-links"><a href="https://kenney.nl/assets/nature-kit">${e(t.licenseLink)} ↗</a><a href="${base}assets/sunny-provenance.json">${e(t.manifestLink)} ↗</a><a href="${base}assets/kenney-nature-license.txt">CC0 · License.txt</a></p><p class="small">${e(t.licenseNote)}</p></div></aside>`;
}
