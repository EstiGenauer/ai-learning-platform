output "namespace" {
  description = "Kubernetes namespace"
  value       = kubernetes_namespace.this.metadata[0].name
}

output "release_name" {
  description = "Helm release name"
  value       = helm_release.learning_platform.name
}

output "ingress_host" {
  description = "Configured Ingress hostname"
  value       = var.ingress_host
}

output "app_url" {
  description = "Application URL (add host to /etc/hosts for local clusters)"
  value       = "http://${var.ingress_host}"
}
