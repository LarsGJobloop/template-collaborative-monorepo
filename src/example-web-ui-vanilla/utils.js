export function showLoading(loadingDiv, errorDiv, commitsDiv) {
  loadingDiv.classList.remove("hidden");
  errorDiv.classList.add("hidden");
  commitsDiv.innerHTML = "";
}

export function hideLoading(loadingDiv) {
  loadingDiv.classList.add("hidden");
}

export function showError(errorDiv, commitsDiv, message) {
  errorDiv.textContent = message;
  errorDiv.classList.remove("hidden");
  commitsDiv.innerHTML = "";
}

export function formatDate(dateString) {
  const date = new Date(dateString);
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, "0");
  const day = String(date.getDate()).padStart(2, "0");
  const hours = String(date.getHours()).padStart(2, "0");
  const minutes = String(date.getMinutes()).padStart(2, "0");
  return `${year}-${month}-${day} ${hours}:${minutes}`;
}

function updateMetaTags(latestCommit) {
  if (!latestCommit) return;

  const message = latestCommit.commit.message.split("\n")[0];
  const author = latestCommit.commit.author.name;
  const date = formatDate(latestCommit.commit.author.date);
  const description = `Latest: ${message} - ${author} (${date})`;

  document.title = `Repository Status - ${message.split("\n")[0].substring(0, 50)}`;
  document.querySelector('meta[name="description"]').content = description;
  document.querySelector('meta[property="og:title"]').content = document.title;
  document.querySelector('meta[property="og:description"]').content =
    description;
  document.querySelector('meta[name="twitter:title"]').content = document.title;
  document.querySelector('meta[name="twitter:description"]').content =
    description;
}

export function renderCommits(commitsDiv, commitTemplate, commits) {
  commitsDiv.innerHTML = "";

  if (commits.length === 0) {
    const noCommits = document.createElement("p");
    noCommits.className = "text-center text-gray-600 p-2";
    noCommits.textContent = "No commits found in repository.";
    commitsDiv.appendChild(noCommits);
    return;
  }

  // Update meta tags with latest commit info
  updateMetaTags(commits[0]);

  commits.forEach((commit) => {
    const clone = commitTemplate.content.cloneNode(true);
    const commitEl = clone.querySelector("li");

    const spans = commitEl.querySelectorAll("span");
    spans[0].textContent = commit.commit.author.name;
    spans[1].textContent = formatDate(commit.commit.author.date);

    commitEl.querySelector("p").textContent =
      commit.commit.message.split("\n")[0];
    commitEl.querySelector("code").textContent = commit.sha.substring(0, 7);

    commitsDiv.appendChild(clone);
  });
}
