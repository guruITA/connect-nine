# The Connect server of production (issue 95, task 95.2). It starts only after the migrations of the
# same image version have finished.
resource "kubernetes_deployment_v1" "connect_server" {
  metadata {
    name      = "connect-server"
    namespace = "connect-production"
    labels = {
      app                         = "connect-server"
      "app.kubernetes.io/part-of" = "connect"
    }
  }

  spec {
    replicas = 1

    selector {
      match_labels = {
        app = "connect-server"
      }
    }

    # The old pod keeps answering until the new one is ready.
    strategy {
      type = "RollingUpdate"
      rolling_update {
        max_unavailable = 0
        max_surge       = 1
      }
    }

    template {
      metadata {
        labels = {
          app = "connect-server"
        }
      }

      spec {
        image_pull_secrets {
          name = "connect-registry-pull"
        }

        # 1654 is the user app of the .NET images the Dockerfile builds on (USER $APP_UID).
        security_context {
          run_as_non_root = true
          run_as_user     = 1654
        }

        container {
          name  = "server"
          image = "${var.image_repository}/server:${var.image_tag}"

          port {
            container_port = 8080
          }

          env_from {
            config_map_ref {
              name = kubernetes_config_map_v1.connect_server_config.metadata[0].name
            }
          }

          env_from {
            secret_ref {
              name = "connect-server-secret"
            }
          }

          # Ready only when /api/health says the server and its database are healthy, so visitors
          # never reach a server that cannot answer.
          readiness_probe {
            http_get {
              path = "/api/health"
              port = 8080
            }
            period_seconds    = 10
            failure_threshold = 3
          }

          # Restarted only when the process stops answering at all. Never /api/health, or a database
          # that is away for a moment would restart a server that is fine.
          liveness_probe {
            tcp_socket {
              port = 8080
            }
            initial_delay_seconds = 30
            period_seconds        = 10
          }

          # Start values, measured after a week with kubectl top pod and then adjusted.
          resources {
            requests = {
              memory = "256Mi"
              cpu    = "100m"
            }
            limits = {
              memory = "512Mi"
              cpu    = "500m"
            }
          }

          # A stopping pod still gets visits until Traefik has seen it go. Measured on 25 October 2026:
          # one visit of 378 got no answer when the old pod stopped (#1051). The pod waits five seconds
          # before .NET is asked to stop, so Traefik sends every visit to the new pod first.
          lifecycle {
            pre_stop {
              exec {
                command = ["sleep", "5"]
              }
            }
          }
        }
      }
    }
  }

  depends_on = [kubernetes_job_v1.connect_migrations]
}

resource "kubernetes_service_v1" "connect_server" {
  metadata {
    name      = "connect-server"
    namespace = "connect-production"
    labels = {
      "app.kubernetes.io/part-of" = "connect"
    }
  }

  spec {
    type = "ClusterIP"
    selector = {
      app = "connect-server"
    }
    port {
      port        = 80
      target_port = 8080
    }
  }
}
