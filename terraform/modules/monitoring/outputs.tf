output "namespace" {
  value = var.namespace
}

output "release_name" {
  value = helm_release.monitoring.name
}

output "grafana_url" {
  value = "http://${var.grafana_host}"
}

output "grafana_host" {
  value = var.grafana_host
}
