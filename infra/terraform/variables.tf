variable "kubeconfig_path" {
  description = "Path to the kubeconfig the pipeline builds from KUBE_CA_CERT and a token (infra/k3s/kubeconfigFromToken.sh). There is no default, so Terraform never reaches a cluster by accident."
  type        = string
}
