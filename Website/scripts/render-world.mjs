import { languages } from "../src/strings.mjs";
import { worldStrings } from "../src/world-strings.mjs";
import { bossStrings } from "../src/world-boss-strings.mjs";
import { escapeHtml as e } from "./render.mjs";
import { sunnyStrings } from "../src/sunny-strings.mjs";
import { sunnyFigure, sunnyMetrics, sunnyDistricts, sunnyPalette, sunnyEvidence, sunnySource, sunnyMapPlaces, sunnyCapture, sunnyFlowers, sunnyGrass, sunnyHarbor } from "./render-sunny.mjs";

export const worldPages = ["", "map", "play", "boss", "art", "roadmap"];

export function renderWorld(locale, files = {}, page = "") {
  const t = worldStrings[locale];
  const boss = bossStrings[locale];
  const sunny = sunnyStrings[locale];
  if (!t) throw new Error(`Unsupported world language: ${locale}`);
  if (!worldPages.includes(page)) throw new Error(`Unsupported world page: ${page}`);
  const mapPlaces=page==="map"?sunnyMapPlaces(locale):[];
  const mapCamera=page==="map"?sunnyCapture("topdown"):null;
  const base = "../".repeat(1 + Number(locale !== "zh-Hans") + Number(Boolean(page)));
  const home = locale === "zh-Hans" ? base : `${base}${locale}/`;
  const worldRoot = `${home}open-world/`;
  const route = (lang) => `${base}${lang === "zh-Hans" ? "" : `${lang}/`}open-world/${page ? `${page}/` : ""}`;
  const canonical = `https://lorendw7.github.io/BalloonBarrage/${locale === "zh-Hans" ? "" : `${locale}/`}open-world/${page ? `${page}/` : ""}`;
  const labels = { "zh-Hans": "中文", ja: "日本語", en: "EN" };
  const ids = ["plaza", "pier", "garden", "rooftops"];
  const nav = [["", t.nav], ["map", t.mapNav], ["play", `${t.loopNav} / ${t.aiNav}`], ["boss", boss.nav], ["art", t.artNav], ["roadmap", t.planNav]];
  const pageTitle = page ? `${nav.find(([id]) => id === page)[1]} | ${t.title}` : t.title;
  const description = page === "boss" ? boss.intro : t.description;
  const sections = (items, css = "world-cards") => `<div class="${css}">${items.map(([title, body]) => `<article><h3>${e(title)}</h3><p>${e(body)}</p></article>`).join("")}</div>`;
  return `<!doctype html>
<html lang="${locale}">
<head>
  <meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
  <title>${e(pageTitle)}</title><meta name="description" content="${e(description)}">
  <meta name="theme-color" content="#fff9e8"><link rel="canonical" href="${canonical}">
  <meta property="og:type" content="website"><meta property="og:title" content="${e(pageTitle)}">
  <meta property="og:description" content="${e(description)}"><meta property="og:url" content="${canonical}">
  <meta property="og:image" content="https://lorendw7.github.io/BalloonBarrage/assets/sunny-share-20261108.jpg"><meta property="og:image:alt" content="${e(sunny.overviewAlt)}"><meta property="og:image:width" content="1200"><meta property="og:image:height" content="675"><meta name="twitter:card" content="summary_large_image">
  ${languages.map(lang => `<link rel="alternate" hreflang="${lang}" href="${route(lang)}">`).join("\n  ")}
  <link rel="icon" type="image/svg+xml" href="${base}favicon.svg">
  <link rel="stylesheet" href="${base}${e(files["styles.css"] ?? "styles.css")}">
  <link rel="stylesheet" href="${base}${e(files["world.css"] ?? "world.css")}">
  <script type="module" src="${base}${e(files["app.js"] ?? "app.js")}"></script>
</head>
<body class="world-page">
  <a class="skip-link" href="#main">${e(t.skip)}</a>
  <header class="site-header"><div class="header-inner">
    <a class="brand" href="${home}"><img src="${base}favicon.svg" width="32" height="32" alt=""><span>Balloon<span class="brand-color">Barrage</span></span></a>
    <a class="world-back" href="${home}">← ${e(t.home)}</a>
    <nav class="language-nav" aria-label="${e(t.language)}">${languages.map(lang => `<a href="${route(lang)}" lang="${lang}" hreflang="${lang}"${locale === lang ? ' aria-current="page"' : ""}>${labels[lang]}</a>`).join("")}</nav>
  </div></header>
  <main id="main">
    <nav class="world-nav" aria-label="${e(t.navigation)}">${nav.map(([id, text]) => `<a href="${worldRoot}${id ? `${id}/` : ""}"${id === page ? ' aria-current="location"' : ""}>${e(text)}</a>`).join("")}</nav>
    ${!page ? `<section class="world-hero section">
      <div class="world-hero-copy"><p class="eyebrow sunny-kicker">${e(sunny.tag)}</p><h1>${e(sunny.title)}</h1><p class="world-intro">${e(sunny.intro)}</p><div class="hero-actions"><a class="button button-teal" href="${worldRoot}map/">${e(sunny.mapLink)} →</a><a class="text-link" href="${worldRoot}art/">${e(sunny.galleryLink)}</a></div><p class="world-status">${e(t.status)}</p></div>
      <div class="sunny-world-hero-art">${sunnyFigure(locale,base,"overview",sunny.overviewAlt,sunny.native,{eager:true,wide:true})}</div>
    </section>` : `<header class="world-page-heading section"><h1>${e(nav.find(([id]) => id === page)[1])}</h1><p class="world-status">${e(t.status)}</p></header>`}
    ${!page ? `<section class="section sunny-home-metrics">${sunnyMetrics(locale)}</section><section class="section">${sunnyHarbor(locale,base,{compact:true})}</section><section class="section"><div class="sunny-section-heading"><div><p class="eyebrow">01 / SUNNY COAST</p><h2>${e(sunny.placesTitle)}</h2></div><p>${e(sunny.placesIntro)}</p></div>${sunnyDistricts(locale,base,{compact:true})}</section><section class="section">${sunnyEvidence(locale)}</section>` : ""}
    ${page === "map" ? `<section class="section" id="map" aria-labelledby="map-title">
      <div class="world-heading"><p class="eyebrow">01 / WORLD</p><h2 id="map-title">${e(sunny.mapTitle)}</h2><p>${e(sunny.mapIntro)}</p></div>
      <div class="sunny-map-layout"><figure class="sunny-map-figure"><div class="sunny-map-image"><a class="art-open" data-art href="${base}assets/sunny-topdown-20261108.png" aria-label="${e(sunny.openImage)}"><img src="${base}assets/sunny-topdown-20261108.webp" width="${mapCamera.width}" height="${mapCamera.height}" alt="${e(sunny.mapAlt)}" fetchpriority="high"></a>${mapPlaces.map((place,i)=>`<a class="sunny-map-pin sunny-map-pin-${i}" style="left:${place.left.toFixed(3)}%;top:${place.top.toFixed(3)}%" href="#${place.id}" aria-label="0${i+1} · ${e(place.title)}"><span>0${i+1}</span></a>`).join("")}</div><figcaption>${e(sunny.native)}</figcaption></figure><nav class="sunny-map-key" aria-label="${e(sunny.mapTitle)}">${mapPlaces.map((place,i)=>`<a href="#${place.id}"><span>0${i+1}</span><div><strong>${e(place.title)}</strong><small>${e(place.tag)}</small></div><b aria-hidden="true">↗</b></a>`).join("")}</nav></div>
      ${sunnyMetrics(locale)}
      ${sunnyHarbor(locale,base)}
      ${sunnyDistricts(locale,base)}
      <h2 class="sunny-north-heading">${e(sunny.northernTitle)}</h2>
      <div class="district-grid">${ids.map((id, i) => `<article id="${id}"><p class="eyebrow">${String(i).padStart(2, "0")} / ${e(t.districtTags[i])}</p><h3>${e(t.districtNames[i])}</h3><p>${e(t.districts[i])}</p></article>`).join("")}</div>
      <p class="world-note">${e(t.scale)}</p>
    </section>` : ""}
    ${page === "play" || page === "boss" ? `<div class="section sunny-plan-boundary"><p>${e(sunny.planNote)}</p></div>` : ""}
    ${page === "play" ? `<section class="world-loop-band"><div class="section" id="loop" aria-labelledby="loop-title"><div class="world-heading"><p class="eyebrow">02 / PLAY</p><h2 id="loop-title">${e(t.loopTitle)}</h2><p>${e(t.loopIntro)}</p></div>${sections(t.loop, "world-loop")}<p class="world-note">${e(t.rule)}</p></div></section>
    <section class="section" id="rival" aria-labelledby="rival-title"><div class="world-heading"><p class="eyebrow">03 / RIVAL</p><h2 id="rival-title">${e(t.aiTitle)}</h2><p>${e(t.aiIntro)}</p></div><ol class="ai-states">${t.aiStates.map(s => `<li>${e(s)}</li>`).join("")}</ol>${sections(t.aiRules)}<details class="world-role-details"><summary>${e(t.roleTitle)}</summary><p>${e(t.roleIntro)}</p>${sections(t.roles)}</details></section>
    ` : ""}
    ${page === "boss" ? `<section class="section" aria-labelledby="boss-title"><div class="world-heading"><h2 id="boss-title">${e(boss.title)}</h2><p>${e(boss.intro)}</p><p>${e(boss.distinction)}</p></div>${sections(boss.steps)}<p class="world-note">${e(boss.outcome)}</p><details class="world-role-details"><summary>${e(boss.rulesTitle)}</summary>${sections(boss.rules)}</details><details class="world-role-details"><summary>${e(boss.techTitle)}</summary><p>${e(boss.techIntro)}</p>${sections(boss.tech)}<p>${e(boss.inspiration)}</p><p><a href="https://www.nintendo.com/en-ca/whatsnew/splatoon-3-makes-a-big-splash-in-new-video-preview-filled-to-the-gills-with-fresh-gameplay-and-new-details/">Nintendo · Salmon Run</a> · <a href="https://unity-technologies.github.io/ml-agents/ML-Agents-Overview/">Unity · ML-Agents</a></p></details><details class="world-role-details"><summary>${e(boss.artTitle)}</summary><p>${e(boss.art)}</p></details><p class="world-note">${e(boss.order)}</p></section>` : ""}
    ${page === "art" ? `<section class="section" id="art-direction" aria-labelledby="art-title"><div class="world-heading"><p class="eyebrow">COLOR / MATERIAL / PLACE</p><h2 id="art-title">${e(sunny.paletteTitle)}</h2><p>${e(sunny.paletteIntro)}</p></div>${sunnyPalette(locale)}${sunnyHarbor(locale,base)}${sunnyGrass(locale,base)}${sunnyFlowers(locale,base)}${sunnyFigure(locale,base,"market",sunny.places[0][3],sunny.places[0][0],{wide:true})}<div class="sunny-section-heading"><h2>${e(sunny.artTitle)}</h2><p>${e(sunny.artIntro)}</p></div>${sunnyDistricts(locale,base,{compact:true})}${sections(t.artRules)}<p class="world-note">${e(t.artLater)}</p>${sunnySource(locale,base)}</section>` : ""}
    ${page === "roadmap" ? `<section class="section sunny-roadmap-intro"><div><p class="eyebrow">NOW / OW0</p><h2>${e(sunny.roadmapTitle)}</h2><p>${e(sunny.roadmapBody)}</p></div>${sunnyFigure(locale,base,"bridge",sunny.bridgeAlt,sunny.bridgeTitle)}${sunnyEvidence(locale)}</section>` : ""}
    ${page === "roadmap" ? `<section class="section world-plan" id="plan" aria-labelledby="plan-title"><div class="world-heading"><p class="eyebrow">04 / BUILD TOGETHER</p><h2 id="plan-title">${e(t.planTitle)}</h2><p>${e(t.planIntro)}</p></div><ol class="world-timeline">${t.stages.map(([code, name, work, gate]) => `<li><span>${code}</span><div><h3>${e(name)}</h3><p>${e(work)}</p><p class="stage-gate">${e(gate)}</p></div></li>`).join("")}</ol><p class="world-note">${e(t.deferred)}</p></section>` : ""}
    ${!page ? `<section class="section world-choices" id="choices" aria-labelledby="choices-title"><div class="world-heading"><p class="eyebrow">${e(t.choicesNav)}</p><h2 id="choices-title">${e(t.choicesTitle)}</h2><p>${e(t.choicesIntro)}</p></div>
      ${sections(t.choices)}
      <p class="world-note"><a href="${worldRoot}boss/">${e(boss.teaser)}</a></p>
    </section>` : ""}
    <nav class="world-nav" aria-label="${e(t.navigation)}">${nav.map(([id, text]) => `<a href="${worldRoot}${id ? `${id}/` : ""}"${id === page ? ' aria-current="location"' : ""}>${e(text)}</a>`).join("")}</nav>
    <footer class="section world-sources"><div class="world-source-links"><a href="${home}">${e(t.home)}</a><a href="${base}assets/sunny-provenance.json">${e(sunny.sourceLink)}</a><a href="https://github.com/lorendw7/BalloonBarrage/blob/main/Docs/Design/GAME_DESIGN.md#open-world-direction--planning-only">${e(t.sourceDesign)}</a><a href="https://github.com/lorendw7/BalloonBarrage/blob/main/Docs/Design/ROADMAP.md#open-world-development-track--planning-only">${e(t.sourceRoadmap)}</a></div><a href="${home}#download">${e(sunny.downloadBridge)} →</a>${page === "art" ? `<p>${e(t.originality)}</p>` : ""}</footer>
  </main><footer class="world-footer">BalloonBarrage · ${e(t.footer)}</footer>
  <dialog class="art-dialog" aria-label="${e(sunny.openImage)}"><button class="dialog-close" type="button">${e(sunny.closeImage)}</button><img alt=""><p></p></dialog>
</body></html>\n`;
}
