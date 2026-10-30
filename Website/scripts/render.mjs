import { languages, strings } from "../src/strings.mjs";
import { repositoryUrl } from "./releases.mjs";
import { worldStrings } from "../src/world-strings.mjs";

export function escapeHtml(value) {
  return String(value).replace(/[&<>"']/g, (char) => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[char]));
}

export function renderPage(template, locale, release, staticFiles = {}) {
  const text = strings[locale];
  if (!text) throw new Error(`Unsupported language: ${locale}`);
  const base = locale === "zh-Hans" ? "./" : "../";
  const route = (language) => language === "zh-Hans" ? base : `${base}${language}/`;
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
  const canonicalUrl = siteUrl + (locale === "zh-Hans" ? "" : `${locale}/`);
  const heroReleaseStatus = snapshots.windows.available
    ? `${text.downloadPreviewLabel} · ${snapshots.windows.tag}${snapshots.macos.available ? ` · macOS ${snapshots.macos.tag}` : ""}`
    : (snapshots.windows.status === "empty" ? text.downloadEmpty : text.downloadUnknown);
  const special = { base, home: route(locale), locale, languageLinks, alternateLinks, releasePanels,
    worldUrl: `${route(locale)}open-world/`, worldNav: e(worldStrings[locale].nav), worldStatus: e(worldStrings[locale].status),
    styleFile: e(staticFiles["styles.css"] ?? "styles.css"), scriptFile: e(staticFiles["app.js"] ?? "app.js"),
    canonicalUrl, shareImageUrl: `${siteUrl}assets/festival-share-20261029.jpg`, heroReleaseStatus: e(heroReleaseStatus) };
  return template.replace(/\{\{(\w+)\}\}/g, (_, key) => {
    if (key in special) return special[key];
    if (!(key in text)) throw new Error(`Missing translation: ${locale}.${key}`);
    return e(text[key]);
  });
}
