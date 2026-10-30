// Set only after checking the responder flow and private response settings.
// Keep all responses, email addresses and editor links out of this repository.
export const surveyConfig = { formUrl: "" };

export function getSurveyFormUrl(value = surveyConfig.formUrl) {
  if (!value) return "";
  const url = new URL(value);
  const supported = (url.hostname === "forms.gle" && /^\/[A-Za-z0-9_-]+\/?$/.test(url.pathname)) ||
    (url.hostname === "docs.google.com" && /^\/forms\/d\/(?:e\/)?[A-Za-z0-9_-]+\/viewform$/.test(url.pathname));
  if (url.protocol !== "https:" || url.port || url.username || url.password || !supported ||
      url.hash || [...url.searchParams.keys()].some(key => key !== "usp")) {
    throw new Error("Use a public Google Forms responder link without personal prefills.");
  }
  return url.href;
}
