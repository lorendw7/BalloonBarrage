import { readFile } from "node:fs/promises";

export const sitePages = ["", "game", "updates", "art"];

export async function loadTemplate(page = "") {
  if (!sitePages.includes(page)) throw new Error(`Unsupported site page: ${page}`);
  const [shell, content] = await Promise.all([
    readFile(new URL("../src/index.html", import.meta.url), "utf8"),
    readFile(new URL(`../src/${page || "home"}.html`, import.meta.url), "utf8")
  ]);
  return shell.replace("{{pageContent}}", content);
}
