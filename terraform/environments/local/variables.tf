variable "create_kind_cluster" {
  description = "Create a local kind cluster (set false for Docker Desktop / minikube / existing cluster)"
  type        = bool
  default     = false
}

variable "kind_cluster_name" {
  description = "Name of the kind cluster when create_kind_cluster is true"
  type        = string
  default     = "learning-platform"
}

variable "kubeconfig_path" {
  description = "Path to kubeconfig when using an existing cluster"
  type        = string
  default     = "~/.kube/config"
}

variable "kubeconfig_context" {
  description = "Optional kubeconfig context name"
  type        = string
  default     = ""
}

variable "install_ingress_controller" {
  description = "Install NGINX Ingress Controller"
  type        = bool
  default     = true
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
  type    = string
  default = "learning-platform.local"
}

variable "openai_api_key" {
  type      = string
  sensitive = true
}

variable "jwt_secret" {
  type      = string
  sensitive = true
  default   = "MySuperSecretKeyForLearningPlatform123!"
}

variable "openai_model" {
  type    = string
  default = "gpt-4o"
}

variable "backend_image_repository" {
  type    = string
  default = "learning-platform-backend"
}

variable "backend_image_tag" {
  type    = string
  default = "latest"
}

variable "frontend_image_repository" {
  type    = string
  default = "learning-platform-frontend"
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
  default = 1
}

variable "frontend_replicas" {
  type    = number
  default = 1
}

variable "postgresql_password" {
  type      = string
  sensitive = true
  default   = "password"
}
