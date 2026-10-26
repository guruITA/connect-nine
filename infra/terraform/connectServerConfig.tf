# The settings of the Connect server in production that are not secret. The namespace
# connect-production comes from infra/k3s/clusterAccess.yaml, Terraform does not create it.
resource "kubernetes_config_map_v1" "connect_server_config" {
  metadata {
    name      = "connect-server-config"
    namespace = "connect-production"
    labels = {
      "app.kubernetes.io/part-of" = "connect"
    }
  }

  data = {
    ASPNETCORE_ENVIRONMENT = "Production"
  }
}
