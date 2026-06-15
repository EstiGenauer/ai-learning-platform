variable "namespace" {
  description = "Kubernetes namespace for the application"
  type        = string
  default     = "learning-platform"
}

variable "release_name" {
  description = "Helm release name"
  type        = string
  default     = "learning-platform"
}

variable "helm_chart_path" {
  description = "Path to the learning-platform Helm chart directory"
  type        = string
}

variable "install_ingress_controller" {
  description = "Install NGINX Ingress Controller via Helm"
  type        = bool
  default     = true
}

variable "ingress_host" {
  description = "Hostname for the Ingress resource"
  type        = string
  default     = "learning-platform.local"
}

variable "openai_api_key" {
  description = "OpenAI API key"
  type        = string
  sensitive   = true
}

variable "jwt_secret" {
  description = "JWT signing secret"
  type        = string
  sensitive   = true
}

variable "openai_model" {
  description = "OpenAI model name"
  type        = string
  default     = "gpt-4o"
}

variable "backend_image_repository" {
  description = "Backend container image repository"
  type        = string
  default     = "learning-platform-backend"
}

variable "backend_image_tag" {
  description = "Backend container image tag"
  type        = string
  default     = "latest"
}

variable "frontend_image_repository" {
  description = "Frontend container image repository"
  type        = string
  default     = "learning-platform-frontend"
}

variable "frontend_image_tag" {
  description = "Frontend container image tag"
  type        = string
  default     = "latest"
}

variable "frontend_api_url" {
  description = "API URL baked into the frontend (use /api when behind Ingress)"
  type        = string
  default     = "/api"
}

variable "backend_replicas" {
  description = "Number of backend pods"
  type        = number
  default     = 1
}

variable "frontend_replicas" {
  description = "Number of frontend pods"
  type        = number
  default     = 1
}

variable "postgresql_password" {
  description = "PostgreSQL password (Bitnami subchart)"
  type        = string
  sensitive   = true
  default     = "password"
}

variable "enable_service_monitor" {
  description = "Create Prometheus ServiceMonitor for backend /metrics"
  type        = bool
  default     = false
}

variable "prometheus_release_name" {
  description = "Prometheus Helm release name (ServiceMonitor label)"
  type        = string
  default     = "monitoring"
}
