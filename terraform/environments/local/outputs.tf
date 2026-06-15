output "namespace" {
  value = module.platform.namespace
}

output "app_url" {
  value = module.platform.app_url
}

output "ingress_host" {
  value = module.platform.ingress_host
}

output "kind_cluster_name" {
  description = "Kind cluster name (when created)"
  value       = var.create_kind_cluster ? var.kind_cluster_name : null
}

output "next_steps" {
  description = "Commands to access the app after apply"
  value       = <<-EOT
    1. Add to hosts file: 127.0.0.1 ${var.ingress_host}
    2. Build/load images into your cluster if not using a registry:
       docker build -t learning-platform-backend:latest ./backend
       docker build -t learning-platform-frontend:latest --build-arg REACT_APP_API_URL=/api ./frontend
    3. For kind/minikube, load images: kind load docker-image learning-platform-backend:latest --name ${var.kind_cluster_name}
    4. Open: http://${var.ingress_host}
    5. Check pods: kubectl get pods -n ${var.namespace}
  EOT
}
