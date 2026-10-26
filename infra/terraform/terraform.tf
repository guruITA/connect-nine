# The versions production is set up with, pinned with = so the same files give the same result
# next year (issue 94, task 94.1). Why Terraform: docs/decisions/010-productionOnK3s.md.
# An update is a merge request that changes a version here and the lock file next to it.
terraform {
  # https://releases.hashicorp.com/terraform/1.17.0/ (released 21 October 2026, read 24 October 2026)
  required_version = "= 1.17.0"

  required_providers {
    kubernetes = {
      source = "hashicorp/kubernetes"
      # https://registry.terraform.io/providers/hashicorp/kubernetes/3.4.0 (released 19 October 2026, read 24 October 2026)
      version = "= 3.4.0"
    }
  }

  # Used: the backend http, with the state named production kept by GitLab (Operate, Terraform
  # states), never in Git. The pipeline gives the address of that state and the login through the
  # TF_HTTP_ variables, so no address, project number or token is in this file.
  # The fallback of decision 010, the backend kubernetes, is not used.
  backend "http" {}
}
