variable "kubeconfig_path" {
  description = "Path to the kubeconfig the pipeline builds from KUBE_CA_CERT and a token (infra/k3s/kubeconfigFromToken.sh). There is no default, so Terraform never reaches a cluster by accident."
  type        = string
}

variable "image_repository" {
  description = "Where the images server and migrations live, the container registry of the project. The pipeline sets it to $CI_REGISTRY_IMAGE."
  type        = string
}

variable "image_tag" {
  description = "The version of both images, the short commit id that image-push gave them. The pipeline sets it to $CI_COMMIT_SHORT_SHA."
  type        = string
}

variable "production_host" {
  description = "The address of the live version, which Caddy passes to Traefik. It comes from the GitLab variable PRODUCTION_HOST."
  type        = string
}

variable "database_storage" {
  description = "The size of the volume of the production database. The server had 228 GB of storage free on 25 October 2026."
  type        = string
  default     = "10Gi"
}
