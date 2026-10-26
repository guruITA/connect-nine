# Brings the tables of the production database up to date before the new server starts (issue 95,
# task 95.2). One Job per image version, so a new version runs its migrations once.
resource "kubernetes_job_v1" "connect_migrations" {
  metadata {
    name      = "connect-migrations-${var.image_tag}"
    namespace = "connect-production"
    labels = {
      app                         = "connect-migrations"
      "app.kubernetes.io/part-of" = "connect"
    }
  }

  spec {
    backoff_limit = 1

    template {
      metadata {
        labels = {
          app = "connect-migrations"
        }
      }

      spec {
        restart_policy = "Never"

        image_pull_secrets {
          name = "connect-registry-pull"
        }

        container {
          name  = "migrations"
          image = "${var.image_repository}/migrations:${var.image_tag}"

          # The bundle reads ConnectionStrings__DefaultConnection, never a password on its command line.
          env_from {
            secret_ref {
              name = "connect-server-secret"
            }
          }
        }
      }
    }
  }

  wait_for_completion = true

  timeouts {
    create = "5m"
    update = "5m"
  }

  depends_on = [kubernetes_stateful_set_v1.connect_database, kubernetes_service_v1.connect_database]
}
