import { languages, strings } from "../src/strings.mjs";
import { repositoryUrl } from "./releases.mjs";
import { worldStrings } from "../src/world-strings.mjs";
import { harborStrings } from "../src/harbor-strings.mjs";
import { sunnyStrings } from "../src/sunny-strings.mjs";
import { sunnyFigure, sunnyMetrics, sunnyDistricts, sunnySource, sunnyEvidence, sunnyFlowers, sunnyGrass, sunnyHarbor } from "./render-sunny.mjs";

export function escapeHtml(value) {
  return String(value).replace(/[&<>"']/g, (char) => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[char]));
}

export function renderPage(template, locale, release, staticFiles = {}, page = "") {
  const text = strings[locale];
  const sunny = sunnyStrings[locale];
  if (!text) throw new Error(`Unsupported language: ${locale}`);
  const depth = Number(locale !== "zh-Hans") + Number(Boolean(page));
  const base = depth ? "../".repeat(depth) : "./";
  const home = `${base}${locale === "zh-Hans" ? "" : `${locale}/`}`;
  const route = (language) => `${base}${language === "zh-Hans" ? "" : `${language}/`}${page ? `${page}/` : ""}`;
  const labels = { "zh-Hans": "中文", ja: "日本語", en: "EN" };
  const languageLinks = languages.map((language) => `<a href="${route(language)}" lang="${language}" hreflang="${language}" data-language="${language}"${locale === language ? ' aria-current="page"' : ""}>${labels[language]}</a>`).join("");
  const alternateLinks = languages.map((language) => `<link rel="alternate" hreflang="${language}" href="${route(language)}">`).join("\n  ");
  const e = escapeHtml;
  const snapshots = release.windows ? release : {
    windows: release,
    macos: { available: false, status: release.status === "unknown" ? "unknown" : "empty" }
  };
  const platformPanel = (platform, snapshot) => {
    const isMac = platform === "macos";
    const platformName = isMac ? text.downloadMacPlatform : text.downloadPlatform;
    const ready = isMac ? text.downloadMacReady : text.downloadReady;
    const instructions = isMac ? text.downloadMacInstructions : text.downloadInstructions;
    const emptyBody = isMac ? text.downloadMacEmptyBody : text.downloadEmptyBody;
    const notice = isMac ? `<p class="platform-notice">${e(text.downloadMacNotice)}</p>` : "";
    return snapshot.available
      ? `<article class="download-card"><p class="platform">${e(platformName)}</p><div class="release-panel"><h3>${e(ready)}</h3>${snapshot.prerelease ? `<p class="badge badge-yellow">${e(text.downloadPreview)}</p>` : ""}<div class="release-meta"><span>${e(text.downloadBuild)}: ${e(snapshot.tag)}</span><span>${e(text.downloadSize)}: ${(snapshot.size / 1024 / 1024).toFixed(1)} MB</span></div><a class="button button-teal" href="${e(snapshot.url)}">${e(ready)}</a><p class="small">${e(instructions)}</p>${notice}<p><a href="${e(snapshot.notes)}">${e(text.downloadNotes)}</a></p></div></article>`
      : `<article class="download-card" data-release-status="${snapshot.status}"><p class="platform">${e(platformName)}</p><div class="release-panel"><h3>${e(snapshot.status === "empty" ? text.downloadEmpty : text.downloadUnknown)}</h3><p>${e(snapshot.status === "empty" ? emptyBody : text.downloadUnknownBody)}</p>${notice}<a class="button button-teal" href="${repositoryUrl}/releases">${e(text.downloadRelease)}</a></div></article>`;
  };
  const releasePanels = platformPanel("windows", snapshots.windows) + platformPanel("macos", snapshots.macos);
  const siteUrl = "https://lorendw7.github.io/BalloonBarrage/";
  const canonicalUrl = siteUrl + (locale === "zh-Hans" ? "" : `${locale}/`) + (page ? `${page}/` : "");
  const heroReleaseStatus = snapshots.windows.available
    ? `${text.downloadPreviewLabel} · ${snapshots.windows.tag}${snapshots.macos.available ? ` · macOS ${snapshots.macos.tag}` : ""}`
    : (snapshots.windows.status === "empty" ? text.downloadEmpty : text.downloadUnknown);
  const headings = { game: text.navGame, updates: text.navUpdates, art: text.navArt };
  const special = { base, home, locale, languageLinks, alternateLinks, releasePanels,
    sunnyTag: e(sunny.tag), sunnyTitle: e(sunny.title), sunnyNative: e(sunny.native),
    sunnyHomeVisual: sunnyFigure(locale,base,"overview",sunny.overviewAlt,sunny.native,{eager:true,wide:true}),
    sunnyMetrics: sunnyMetrics(locale), sunnyPlacesTitle:e(sunny.placesTitle), sunnyPlacesIntro:e(sunny.placesIntro),
    sunnyDistricts: sunnyDistricts(locale,base,{compact:true}), sunnyMapLink:e(sunny.mapLink),
    sunnySource: sunnySource(locale,base), sunnyEvidence: sunnyEvidence(locale),
    sunnyHarbor: sunnyHarbor(locale,base,{compact:true}),
    sunnyFlowers: sunnyGrass(locale,base)+sunnyFlowers(locale,base),
    sunnyLatestTitle:e(sunny.latestTitle), sunnyLatestBody:e(sunny.latestBody),
    sunnyGalleryIntro: `<section class="section sunny-gallery-intro"><p class="eyebrow">${e(sunny.tag)}</p><h2>${e(sunny.artTitle)}</h2><p>${e(sunny.artIntro)}</p></section>`,
    sunnyGallery: `<section class="section sunny-gallery">${sunnyHarbor(locale,base)}${sunnyGrass(locale,base)}${sunnyFlowers(locale,base)}${sunnyDistricts(locale,base)}${sunnySource(locale,base)}</section>`,
    sunnyUpdateFigure: sunnyFigure(locale,base,"ridgeline",harborStrings[locale].terrainViews[0][3],harborStrings[locale].terrainViews[0][0]),
    description: e(text.description),
    title: e(page ? `${headings[page]} | BalloonBarrage` : text.title), pageHeading: e(headings[page] ?? text.title), pageClass: page ? "site-detail" : "site-home",
    gameUrl: `${home}game/`, updatesUrl: `${home}updates/`, artUrl: `${home}art/`,
    worldUrl: `${home}open-world/`, worldNav: e(worldStrings[locale].nav), worldStatus: e(worldStrings[locale].status),
    styleFile: e(staticFiles["styles.css"] ?? "styles.css"), scriptFile: e(staticFiles["app.js"] ?? "app.js"),
    audienceCheerFile: e(staticFiles["audience-cheer.mp3"] ?? "audience-cheer.mp3"),
    canonicalUrl, shareImageUrl: `${siteUrl}assets/${page === "game" ? "festival-share-20261029.jpg" : "sunny-share-20261108.jpg"}`,
    shareImageAlt:e(page === "game" ? text.prototypeAlt : sunny.overviewAlt),
    preloadImage:`${base}assets/${page === "game" ? "festival-response-20261029.webp" : page === "updates" ? "sunny-ridgeline-20261108.webp" : page === "art" ? "sunny-harbor-overview-20261108.webp" : "sunny-overview-20261108.webp"}`,
    heroReleaseStatus: e(heroReleaseStatus) };
  return template.replace(/\{\{(\w+)\}\}/g, (_, key) => {
    if (key in special) return special[key];
    if (!(key in text)) throw new Error(`Missing translation: ${locale}.${key}`);
    return e(text[key]);
  });
}
