export const repository = "lorendw7/BalloonBarrage";
export const repositoryUrl = `https://github.com/${repository}`;

function selectPlatformDownload(releases, platform) {
  const patterns = {
    windows: /windows|win64|win-x64/i,
    macos: /macos|osx|mac-universal/i
  };
  const candidates = releases.filter((release) => !release.draft).sort((a, b) => {
    // Prefer stable builds; otherwise expose a clearly labelled pre-release demo.
    return Number(a.prerelease) - Number(b.prerelease) ||
      new Date(b.published_at) - new Date(a.published_at);
  });
  for (const release of candidates) {
    const asset = (release.assets ?? []).find((item) =>
      patterns[platform].test(item.name) && /\.zip$/i.test(item.name) &&
      item.size > 0 && item.state === "uploaded" &&
      item.browser_download_url?.startsWith(`${repositoryUrl}/releases/download/`)
    );
    if (asset) return {
      available: true, status: "ready", tag: release.tag_name,
      prerelease: Boolean(release.prerelease), name: asset.name,
      size: asset.size, url: asset.browser_download_url,
      notes: `${repositoryUrl}/releases/tag/${encodeURIComponent(release.tag_name)}`
    };
  }
  return { available: false, status: "empty" };
}

export function selectDownloads(releases) {
  return {
    windows: selectPlatformDownload(releases, "windows"),
    macos: selectPlatformDownload(releases, "macos")
  };
}

// Retained for callers that only need the established Windows package.
export function selectDownload(releases) {
  return selectDownloads(releases).windows;
}

export async function getReleaseSnapshot({ offline = false } = {}) {
  if (offline) return {
    windows: { available: false, status: "unknown" },
    macos: { available: false, status: "unknown" }
  };
  const headers = { Accept: "application/vnd.github+json", "User-Agent": "BalloonBarrage-Website" };
  if (process.env.GITHUB_TOKEN) headers.Authorization = `Bearer ${process.env.GITHUB_TOKEN}`;
  try {
    const response = await fetch(`https://api.github.com/repos/${repository}/releases?per_page=100`, {
      headers, signal: AbortSignal.timeout(15000)
    });
    if (!response.ok) throw new Error(`GitHub API returned ${response.status}`);
    return selectDownloads(await response.json());
  } catch (error) {
    // Never fail the website, invent a download, or call an API failure "no release".
    console.warn(`Release check unavailable: ${error.message}`);
    return {
      windows: { available: false, status: "unknown" },
      macos: { available: false, status: "unknown" }
    };
  }
}
