import { languages } from "../src/strings.mjs";
import { worldStrings } from "../src/world-strings.mjs";
import { escapeHtml as e } from "./render.mjs";

export function renderWorld(locale, files = {}) {
  const t = worldStrings[locale];
  if (!t) throw new Error(`Unsupported world language: ${locale}`);
  const base = locale === "zh-Hans" ? "../" : "../../";
  const home = locale === "zh-Hans" ? base : `${base}${locale}/`;
  const route = (lang) => `${base}${lang === "zh-Hans" ? "" : `${lang}/`}open-world/`;
  const canonical = `https://lorendw7.github.io/BalloonBarrage/${locale === "zh-Hans" ? "" : `${locale}/`}open-world/`;
  const labels = { "zh-Hans": "中文", ja: "日本語", en: "EN" };
  const ids = ["plaza", "pier", "garden", "rooftops"];
  const nav = [["map", t.mapNav], ["loop", t.loopNav], ["rival", t.aiNav], ["plan", t.planNav], ["choices", t.choicesNav]];
  const sections = (items, css = "world-cards") => `<div class="${css}">${items.map(([title, body]) => `<article><h3>${e(title)}</h3><p>${e(body)}</p></article>`).join("")}</div>`;
  return `<!doctype html>
<html lang="${locale}">
<head>
  <meta charset="utf-8"><meta name="viewport" content="width=device-width, initial-scale=1">
  <title>${e(t.title)}</title><meta name="description" content="${e(t.description)}">
  <meta name="theme-color" content="#fff4dc"><link rel="canonical" href="${canonical}">
  <meta property="og:type" content="website"><meta property="og:title" content="${e(t.title)}">
  <meta property="og:description" content="${e(t.description)}"><meta property="og:url" content="${canonical}">
  ${languages.map(lang => `<link rel="alternate" hreflang="${lang}" href="${route(lang)}">`).join("\n  ")}
  <link rel="icon" type="image/svg+xml" href="${base}favicon.svg">
  <link rel="stylesheet" href="${base}${e(files["styles.css"] ?? "styles.css")}">
  <link rel="stylesheet" href="${base}${e(files["world.css"] ?? "world.css")}">
  <script type="module" src="${base}${e(files["world.js"] ?? "world.js")}"></script>
</head>
<body class="world-page">
  <a class="skip-link" href="#main">${e(t.skip)}</a>
  <header class="site-header"><div class="header-inner">
    <a class="brand" href="${home}"><img src="${base}favicon.svg" width="32" height="32" alt=""><span>Balloon<span class="brand-color">Barrage</span></span></a>
    <a class="world-back" href="${home}">← ${e(t.home)}</a>
    <nav class="language-nav" aria-label="${e(t.language)}">${languages.map(lang => `<a href="${route(lang)}" lang="${lang}" hreflang="${lang}"${locale === lang ? ' aria-current="page"' : ""}>${labels[lang]}</a>`).join("")}</nav>
  </div></header>
  <main id="main">
    <section class="world-hero section">
      <div class="world-hero-copy"><p class="eyebrow">${e(t.eyebrow)}</p><h1>${e(t.headline).replace("\n", "<br>")}</h1><p class="world-intro">${e(t.intro)}</p><p class="world-status">${e(t.status)}</p><p class="small">${e(t.updated)}</p></div>
      <aside class="world-decisions"><span class="world-stamp" aria-hidden="true">01</span><h2>${e(t.selected)}</h2><ol>${t.decisions.map(item => `<li>${e(item)}</li>`).join("")}</ol><p>${e(t.boundary)}</p></aside>
    </section>
    <nav class="world-nav" aria-label="${e(t.navigation)}">${nav.map(([id, text]) => `<a href="#${id}">${e(text)}</a>`).join("")}</nav>
    <section class="section" id="map" aria-labelledby="map-title">
      <div class="world-heading"><p class="eyebrow">01 / WORLD</p><h2 id="map-title">${e(t.mapTitle)}</h2><p>${e(t.mapIntro)}</p></div>
      <figure class="world-map"><div class="world-map-canvas">
        <svg class="world-map-lines" viewBox="0 0 1000 440" preserveAspectRatio="none" aria-hidden="true"><path d="M500 130 V210 H180 V300 M500 210 V300 M500 210 H820 V300"/><path class="world-return-path" d="M150 330 Q70 150 410 100 M850 330 Q930 150 590 100"/></svg>
        ${ids.map((id, i) => `<a class="map-place map-place-${i}" href="#${id}"><span class="map-symbol" aria-hidden="true">${["◎", "▤", "♧", "⌁"][i]}</span><strong>${e(t.districtNames[i])}</strong><span>${e(t.districtTags[i])}</span></a>`).join("")}
      </div><figcaption>${e(t.mapLabel)}</figcaption></figure>
      <div class="district-grid">${ids.map((id, i) => `<article id="${id}"><p class="eyebrow">${String(i).padStart(2, "0")} / ${e(t.districtTags[i])}</p><h3>${e(t.districtNames[i])}</h3><p>${e(t.districts[i])}</p></article>`).join("")}</div>
      <p class="world-note">${e(t.scale)}</p>
    </section>
    <section class="world-loop-band"><div class="section" id="loop" aria-labelledby="loop-title"><div class="world-heading"><p class="eyebrow">02 / PLAY</p><h2 id="loop-title">${e(t.loopTitle)}</h2><p>${e(t.loopIntro)}</p></div>${sections(t.loop, "world-loop")}<p class="world-note">${e(t.rule)}</p></div></section>
    <section class="section" id="rival" aria-labelledby="rival-title"><div class="world-heading"><p class="eyebrow">03 / RIVAL</p><h2 id="rival-title">${e(t.aiTitle)}</h2><p>${e(t.aiIntro)}</p></div><ol class="ai-states">${t.aiStates.map(s => `<li>${e(s)}</li>`).join("")}</ol>${sections(t.aiRules)}</section>
    <section class="section world-plan" id="plan" aria-labelledby="plan-title"><div class="world-heading"><p class="eyebrow">04 / BUILD TOGETHER</p><h2 id="plan-title">${e(t.planTitle)}</h2><p>${e(t.planIntro)}</p></div><ol class="world-timeline">${t.stages.map(([code, name, work, gate]) => `<li><span>${code}</span><div><h3>${e(name)}</h3><p>${e(work)}</p><p class="stage-gate">${e(gate)}</p></div></li>`).join("")}</ol><p class="world-note">${e(t.deferred)}</p></section>
    <section class="section world-choices" id="choices" aria-labelledby="choices-title"><div class="world-heading"><p class="eyebrow">05 / YOUR NEXT CHOICE</p><h2 id="choices-title">${e(t.choicesTitle)}</h2><p>${e(t.choicesIntro)}</p></div>
      <form data-world-choices data-unset="${e(t.unset)}"><div class="world-choice-grid">${t.groups.map(([name, options], i) => `<fieldset data-label="${e(name)}"><legend>${e(name)}</legend>${options.map((option, j) => `<label><input type="radio" name="choice-${i}" value="${"ABC"[j]}"><span>${e(option)}</span></label>`).join("")}</fieldset>`).join("")}</div><div class="world-choice-summary" hidden><h3>${e(t.selectionLabel)}</h3><output aria-live="polite" aria-atomic="true"></output><p class="small">${e(t.selectionHint)}</p><button type="reset">${e(t.reset)}</button></div><noscript><p>${e(t.noScript)}</p></noscript></form>
    </section>
    <section class="section world-sources"><h2>${e(t.sourcesTitle)}</h2><div class="world-source-links"><a class="button button-teal" href="${home}">${e(t.home)}</a><a href="https://github.com/lorendw7/BalloonBarrage/blob/main/Docs/Design/GAME_DESIGN.md#open-world-direction--planning-only">${e(t.sourceDesign)}</a><a href="https://github.com/lorendw7/BalloonBarrage/blob/main/Docs/Design/ROADMAP.md#open-world-development-track--planning-only">${e(t.sourceRoadmap)}</a></div><p>${e(t.originality)}</p></section>
  </main><footer class="world-footer">BalloonBarrage · ${e(t.footer)}</footer>
</body></html>\n`;
}
