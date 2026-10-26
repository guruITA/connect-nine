# The database of production (issue 95, task 95.1). Its user, database and password come from the
# Secret connect-database-secret, which infra/k3s/applySecrets.sh makes, so no password is in this file.
resource "kubernetes_stateful_set_v1" "connect_database" {
  metadata {
    name      = "connect-database"
    namespace = "connect-production"
    labels = {
      app                         = "connect-database"
      "app.kubernetes.io/part-of" = "connect"
    }
  }

  spec {
    service_name = "connect-database"
    replicas     = 1

    selector {
      match_labels = {
        app = "connect-database"
      }
    }

    template {
      metadata {
        labels = {
          app = "connect-database"
        }
      }

      spec {
        container {
          name = "postgres"
          # https://hub.docker.com/_/postgres, tag 16.15 of 22 October 2026, read on 25 October 2026.
          image = "postgres:16.15"

          env_from {
            secret_ref {
              name = "connect-database-secret"
            }
          }

          # A folder inside the volume, so initdb never meets files of the volume itself.
          env {
            name  = "PGDATA"
            value = "/var/lib/postgresql/data/pgdata"
          }

          port {
            container_port = 5432
          }

          readiness_probe {
            exec {
              command = ["pg_isready", "-U", "connect", "-d", "connect"]
            }
            period_seconds = 10
          }

          # Start values, measured after a week with kubectl top pod and then adjusted.
          resources {
            requests = {
              memory = "256Mi"
              cpu    = "100m"
            }
            limits = {
              memory = "1Gi"
              cpu    = "500m"
            }
          }

          volume_mount {
            name       = "database-data"
            mount_path = "/var/lib/postgresql/data"
          }
        }
      }
    }

    # local-path keeps the files on the server under /var/lib/rancher/k3s/storage.
    volume_claim_template {
      metadata {
        name = "database-data"
      }
      spec {
        access_modes       = ["ReadWriteOnce"]
        storage_class_name = "local-path"
        resources {
          requests = {
            storage = var.database_storage
          }
        }
      }
    }
  }

  # The data of young people lives here. Terraform refuses a plan that would remove it.
  lifecycle {
    prevent_destroy = true
  }
}

resource "kubernetes_service_v1" "connect_database" {
  metadata {
    name      = "connect-database"
    namespace = "connect-production"
    labels = {
      "app.kubernetes.io/part-of" = "connect"
    }
  }

  spec {
    type = "ClusterIP"
    selector = {
      app = "connect-database"
    }
    port {
      port        = 5432
      target_port = 5432
    }
  }
}
