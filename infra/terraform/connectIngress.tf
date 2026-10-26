# Tells Traefik that the address of the live version goes to the Service connect-server (issue 95,
# task 95.3). Caddy holds the padlock in front of it, so there is no tls block here.
resource "kubernetes_ingress_v1" "connect_server" {
  metadata {
    name      = "connect-server"
    namespace = "connect-production"
    labels = {
      "app.kubernetes.io/part-of" = "connect"
    }
  }

  spec {
    ingress_class_name = "traefik"

    rule {
      host = var.production_host
      http {
        path {
          path      = "/"
          path_type = "Prefix"
          backend {
            service {
              name = kubernetes_service_v1.connect_server.metadata[0].name
              port {
                number = 80
              }
            }
          }
        }
      }
    }
  }
}
