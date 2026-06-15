output "cluster_name" {
  value = module.eks.cluster_name
}

output "cluster_endpoint" {
  value = module.eks.cluster_endpoint
}

output "configure_kubectl" {
  value = "aws eks update-kubeconfig --region ${var.aws_region} --name ${module.eks.cluster_name}"
}

output "namespace" {
  value = module.platform.namespace
}

output "app_url" {
  value = module.platform.app_url
}

output "ingress_host" {
  value = module.platform.ingress_host
}

output "grafana_url" {
  value = var.install_monitoring ? module.monitoring[0].grafana_url : null
}

output "get_ingress_ip" {
  value = "kubectl get svc -n ingress-nginx ingress-nginx-controller -o jsonpath='{.status.loadBalancer.ingress[0].hostname}'"
}
