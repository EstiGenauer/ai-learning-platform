variable "namespace" {
  description = "Namespace for the monitoring stack"
  type        = string
  default     = "monitoring"
}

variable "release_name" {
  description = "Helm release name"
  type        = string
  default     = "monitoring"
}

variable "helm_chart_path" {
  description = "Path to helm/monitoring chart"
  type        = string
}

variable "grafana_host" {
  description = "Grafana Ingress hostname"
  type        = string
  default     = "grafana.learning-platform.local"
}

variable "grafana_admin_password" {
  description = "Grafana admin password"
  type        = string
  sensitive   = true
  default     = "admin"
}

variable "prometheus_retention" {
  description = "Prometheus data retention period"
  type        = string
  default     = "7d"
}
