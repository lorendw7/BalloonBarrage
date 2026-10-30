import { surveyStrings } from "../src/survey-strings.mjs";
import { getSurveyFormUrl } from "../src/survey-config.mjs";

export function surveyContent(locale, e, configuredUrl, qrFiles = {}) {
  const t = surveyStrings[locale];
  const formUrl = getSurveyFormUrl(configuredUrl);
  const qr = formUrl && qrFiles.svg && qrFiles.png ? `<figure class="survey-qr"><a href="${e(formUrl)}" rel="noreferrer"><img src="${e(qrFiles.svg)}" width="260" height="260" alt="${e(t.qrAlt)}"></a><figcaption><strong>${e(t.qrTitle)}</strong><p>${e(t.qrHint)}</p><a href="${e(qrFiles.png)}" download="BalloonBarrage-survey-QR.png">${e(t.qrDownload)}</a></figcaption></figure>` : "";
  return `<section class="section survey-entry" aria-labelledby="survey-status"><div><h2 id="survey-status">${e(formUrl ? t.open : t.pending)}</h2><p>${e(formUrl ? t.openBody : t.pendingBody)}</p>${formUrl ? `<p><a class="button button-teal" href="${e(formUrl)}" rel="noreferrer">${e(t.open)}</a></p>` : ""}</div>${qr}</section>
    <section class="section" aria-labelledby="survey-questions"><h2 id="survey-questions">${e(t.questionsTitle)}</h2><p>${e(t.questionHelp)}</p><ol>${t.questions.map(([title, choices], questionIndex) => `<li><h3>${e(title)}</h3><p>${[...choices, t.unsure].map(e).join(" · ")}</p><details><summary>${e(t.detailsTitle)}</summary><ul>${choices.map((choice, optionIndex) => `<li><strong>${e(choice)}</strong> — ${e(t.choiceExplanations[questionIndex][optionIndex])}</li>`).join("")}</ul></details></li>`).join("")}</ol></section>
    <section class="section" aria-labelledby="survey-participation"><h2 id="survey-participation">${e(t.participationTitle)}</h2><p>${e(t.participation)}</p><details><summary>${e(t.privacyTitle)}</summary><p>${e(t.privacy)}</p></details>${formUrl ? "" : `<p class="small">${e(t.note)}</p>`}</section>`;
}
