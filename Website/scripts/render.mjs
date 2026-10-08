import { languages, strings } from "../src/strings.mjs";
import { repositoryUrl } from "./releases.mjs";

export function escapeHtml(value) {
  return String(value).replace(/[&<>"']/g, (char) => ({ "&": "&amp;", "<": "&lt;", ">": "&gt;", '"': "&quot;", "'": "&#39;" }[char]));
}

export function renderPage(template, locale, release) {
  const text = strings[locale];
  if (!text) throw new Error(`Unsupported language: ${locale}`);
  const base = locale === "zh-Hans" ? "./" : "../";
  const route = (language) => language === "zh-Hans" ? base : `${base}${language}/`;
  const labels = { "zh-Hans": "中文", ja: "日本語", en: "EN" };
  const languageLinks = languages.map((language) => `<a href="${route(language)}" lang="${language}" hreflang="${language}" data-language="${language}"${locale === language ? ' aria-current="page"' : ""}>${labels[language]}</a>`).join("");
  const alternateLinks = languages.map((language) => `<link rel="alternate" hreflang="${language}" href="${route(language)}">`).join("\n  ");
  const e = escapeHtml;
  const releasePanel = release.available
    ? `<div class="release-panel"><h3>${e(text.downloadReady)}</h3>${release.prerelease ? `<p class="badge badge-yellow">${e(text.downloadPreview)}</p>` : ""}<div class="release-meta"><span>${e(text.downloadBuild)}: ${e(release.tag)}</span><span>${e(text.downloadSize)}: ${(release.size / 1024 / 1024).toFixed(1)} MB</span></div><a class="button button-teal" href="${e(release.url)}">${e(text.downloadReady)}</a><p class="small">${e(text.downloadInstructions)}</p><p><a href="${e(release.notes)}">${e(text.downloadNotes)}</a></p></div>`
    : `<div class="release-panel" data-release-status="${release.status}"><h3>${e(release.status === "empty" ? text.downloadEmpty : text.downloadUnknown)}</h3><p>${e(release.status === "empty" ? text.downloadEmptyBody : text.downloadUnknownBody)}</p><a class="button button-teal" href="${repositoryUrl}/releases">${e(text.downloadRelease)}</a></div>`;
  const special = { base, home: route(locale), locale, languageLinks, alternateLinks, releasePanel };
  return template.replace(/\{\{(\w+)\}\}/g, (_, key) => {
    if (key in special) return special[key];
    if (!(key in text)) throw new Error(`Missing translation: ${locale}.${key}`);
    return e(text[key]);
  });
}
