const API_BASE = "https://api.github.com";
const CACHE_PREFIX = "github_commits_";
const CACHE_DURATION = 10 * 60 * 1000; // 10 minutes

function getCacheKey(owner, repo, perPage) {
  return `${CACHE_PREFIX}${owner}/${repo}/${perPage}`;
}

function getCached(owner, repo, perPage) {
  try {
    const cached = localStorage.getItem(getCacheKey(owner, repo, perPage));
    if (!cached) return null;

    const { data, timestamp } = JSON.parse(cached);
    if (Date.now() - timestamp > CACHE_DURATION) {
      localStorage.removeItem(getCacheKey(owner, repo, perPage));
      return null;
    }
    return data;
  } catch {
    return null;
  }
}

function setCached(owner, repo, perPage, commits) {
  try {
    localStorage.setItem(
      getCacheKey(owner, repo, perPage),
      JSON.stringify({
        data: commits,
        timestamp: Date.now(),
      }),
    );
  } catch {
    // Ignore storage errors
  }
}

export async function getCommits(owner, repo, options = {}) {
  const perPage = options.perPage || 10;

  // Check cache
  const cached = getCached(owner, repo, perPage);
  if (cached) return cached;

  // Fetch from API
  const url = new URL(`${API_BASE}/repos/${owner}/${repo}/commits`);
  url.searchParams.set("per_page", perPage.toString());
  if (options.branch) {
    url.searchParams.set("sha", options.branch);
  }

  const response = await fetch(url, {
    headers: { Accept: "application/vnd.github.v3+json" },
  });

  if (!response.ok) {
    throw new Error(`Error: ${response.status} ${response.statusText}`);
  }

  const commits = await response.json();
  if (!Array.isArray(commits)) {
    throw new Error("Invalid response format");
  }

  setCached(owner, repo, perPage, commits);
  return commits;
}
