# Branch Protection Rules
# Enforces code review requirements and other protections on the main branch

resource "github_branch_protection" "main" {
  repository_id = github_repository.remote_repo.name
  pattern       = "main"

  # Require pull request reviews before merging
  required_pull_request_reviews {
    required_approving_review_count = 0

    # Only allow project owner to dismiss reviews.
    # This is to prevent abuse of the dismissal feature.
    dismissal_restrictions = [
      "/${local.owner}"
    ]

    # Dismiss stale reviews automatically when new commits are pushed.
    dismiss_stale_reviews = true

    # Require reviews from at least one code owner.
    # Defined by ownership in CODEOWNERS file.
    require_code_owner_reviews = true
  }

  # Require status checks to pass before merging
  # (Can be expanded later when CI/CD is set up)
  # required_status_checks {
  #   strict = true
  #   contexts = []
  # }

  # Dissallow merge commits
  # This ensures that the branch history is linear
  # and avoids merge complications when rebasing.
  required_linear_history = true

  # Prevent force pushes
  # Prevents changing the history of the branch.
  allows_force_pushes = false

  # Prevent deletion of the branch
  allows_deletions = false

  # Require branches to be up to date before merging
  require_conversation_resolution = true

  # Apply the same rules to administrators as to other users.
  # This ensures that administrators cannot bypass the protection rules.
  enforce_admins = true

  # We don't require signed handoffs of commits.
  require_signed_commits = false
}
