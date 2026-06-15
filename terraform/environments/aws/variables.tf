variable "aws_region" {
  type    = string
  default = "eu-west-1"
}

variable "project_name" {
  type    = string
  default = "learning-platform"
}

variable "vpc_cidr" {
  type    = string
  default = "10.0.0.0/16"
}

variable "single_nat_gateway" {
  description = "Use one NAT gateway to reduce cost (fine for dev/demo)"
  type        = bool
  default     = true
}

variable "kubernetes_version" {
  type    = string
  default = "1.31"
}

variable "node_instance_types" {
  type    = list(string)
  default = ["t3.medium"]
}

variable "node_min_size" {
  type    = number
  default = 1
}

variable "node_max_size" {
  type    = number
  default = 3
}

variable "node_desired_size" {
  type    = number
  default = 2
}

variable "namespace" {
  type    = string
  default = "learning-platform"
}

variable "release_name" {
  type    = string
  default = "learning-platform"
}

variable "ingress_host" {
  description = "Public hostname (point DNS A record to the NGINX LoadBalancer)"
  type        = string
}

variable "openai_api_key" {
  type      = string
  sensitive = true
}

variable "jwt_secret" {
  type      = string
  sensitive = true
}

variable "openai_model" {
  type    = string
  default = "gpt-4o"
}

variable "backend_image_repository" {
  type        = string
  description = "Public registry path, e.g. dockerhub-user/learning-platform-backend"
}

variable "backend_image_tag" {
  type    = string
  default = "latest"
}

variable "frontend_image_repository" {
  type = string
}

variable "frontend_image_tag" {
  type    = string
  default = "latest"
}

variable "frontend_api_url" {
  type    = string
  default = "/api"
}

variable "backend_replicas" {
  type    = number
  default = 2
}

variable "frontend_replicas" {
  type    = number
  default = 2
}

variable "postgresql_password" {
  type      = string
  sensitive = true
}

variable "install_monitoring" {
  description = "Install Prometheus + Grafana (kube-prometheus-stack)"
  type        = bool
  default     = true
}

variable "monitoring_namespace" {
  type    = string
  default = "monitoring"
}

variable "monitoring_release_name" {
  type    = string
  default = "monitoring"
}

variable "grafana_host" {
  description = "Grafana Ingress hostname (DNS record required)"
  type        = string
}

variable "grafana_admin_password" {
  type      = string
  sensitive = true
}

variable "prometheus_retention" {
  type    = string
  default = "15d"
}
