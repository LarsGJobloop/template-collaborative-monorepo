import { showLoading, hideLoading, showError, renderCommits } from "./utils.js";
import { getCommits } from "./github-api.js";

const REPO = "LarsGJobloop/template-collaborative-monorepo";
const [OWNER, REPO_NAME] = REPO.split("/");
const REFRESH_INTERVAL = 10 * 60 * 1000;

const commitsDiv = document.getElementById("commits");
const loadingDiv = document.getElementById("loading");
const errorDiv = document.getElementById("error");
const commitTemplate = document.getElementById("commit-template");

async function loadCommits() {
  showLoading(loadingDiv, errorDiv, commitsDiv);

  try {
    const commits = await getCommits(OWNER, REPO_NAME, { perPage: 10 });
    hideLoading(loadingDiv);
    renderCommits(commitsDiv, commitTemplate, commits);
  } catch (error) {
    hideLoading(loadingDiv);
    const errorMessage = error.message || "Failed to load repository status";
    showError(errorDiv, commitsDiv, errorMessage);
  }
}

// Load commits immediately
loadCommits();

// Refresh every 10 minutes
setInterval(loadCommits, REFRESH_INTERVAL);
