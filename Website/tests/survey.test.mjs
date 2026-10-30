import test from "node:test";
import assert from "node:assert/strict";
import { surveyStrings } from "../src/survey-strings.mjs";
import { getSurveyFormUrl } from "../src/survey-config.mjs";
import { surveyContent } from "../scripts/render-survey.mjs";
import { escapeHtml } from "../scripts/render.mjs";

test("unconnected survey previews never pretend to collect personal information or votes", () => {
  assert.equal(getSurveyFormUrl(""), "");
  for (const [locale, text] of Object.entries(surveyStrings)) {
    assert.deepEqual(Object.keys(text).sort(), Object.keys(surveyStrings.en).sort());
    assert.equal(text.questions.length, 4);
    const html = surveyContent(locale, escapeHtml, "");
    assert.ok(html.includes(text.pending));
    assert.ok(html.includes(escapeHtml(text.participation)));
    assert.ok(html.includes(escapeHtml(text.privacy)));
    assert.doesNotMatch(html, /<form\b|<input\b|<textarea\b|<button\b|href="https:\/\/(?:forms\.gle|docs\.google\.com)/);
    for (const [question, options] of text.questions) {
      assert.ok(html.includes(escapeHtml(question)));
      assert.ok(options.every(option => html.includes(escapeHtml(option))));
    }
  }
});

test("survey link configuration accepts responder URLs and rejects edit or personal-prefill URLs", () => {
  assert.equal(getSurveyFormUrl("https://forms.gle/Test123"), "https://forms.gle/Test123");
  assert.equal(getSurveyFormUrl("https://docs.google.com/forms/d/e/Test123/viewform?usp=sharing"),
    "https://docs.google.com/forms/d/e/Test123/viewform?usp=sharing");
  const html = surveyContent("en", escapeHtml, "https://forms.gle/Test123");
  assert.ok(html.includes('href="https://forms.gle/Test123"'));
  assert.ok(html.includes(surveyStrings.en.openBody));
  assert.doesNotMatch(html, /<input\b|<form\b/);
  for (const url of ["https://example.com/", "javascript:alert(1)", "https://docs.google.com/forms/d/Test123/edit",
    "https://docs.google.com/forms/d/e/Test123/viewform?entry.1=email@example.com",
    "https://forms.gle/Test123#private", "https://user:password@forms.gle/Test123"]) {
    assert.throws(() => getSurveyFormUrl(url));
  }
});
