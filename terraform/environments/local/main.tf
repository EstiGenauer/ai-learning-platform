terraform {
  required_version = ">= 1.5.0"

  required_providers {
    kubernetes = {
      source  = "hashicorp/kubernetes"
      version = "~> 2.35"
    }
    helm = {
      source  = "hashicorp/helm"
      version = "~> 2.17"
    }
    kind = {
      source  = "tehcyclone/kind"
      version = "~> 0.8"
    }
  }
}

provider "kind" {}

provider "kubernetes" {
  host                   = local.use_kind ? kind_cluster.this[0].endpoint : null
  cluster_ca_certificate = local.use_kind ? kind_cluster.this[0].cluster_ca_certificate : null
  client_certificate     = local.use_kind ? kind_cluster.this[0].client_certificate : null
  client_key             = local.use_kind ? kind_cluster.this[0].client_key : null
  config_path            = local.use_kind ? null : var.kubeconfig_path
  config_context         = var.kubeconfig_context != "" ? var.kubeconfig_context : null
}

provider "helm" {
  kubernetes {
    host                   = local.use_kind ? kind_cluster.this[0].endpoint : null
    cluster_ca_certificate = local.use_kind ? kind_cluster.this[0].cluster_ca_certificate : null
    client_certificate     = local.use_kind ? kind_cluster.this[0].client_certificate : null
    client_key             = local.use_kind ? kind_cluster.this[0].client_key : null
    config_path            = local.use_kind ? null : var.kubeconfig_path
    config_context         = var.kubeconfig_context != "" ? var.kubeconfig_context : null
  }
}

resource "kind_cluster" "this" {
  count = var.create_kind_cluster ? 1 : 0
  name  = var.kind_cluster_name
}

module "platform" {
  source = "../../modules/platform"

  helm_chart_path            = "${path.module}/../../../helm/learning-platform"
  install_ingress_controller = var.install_ingress_controller
  namespace                  = var.namespace
  release_name               = var.release_name
  ingress_host               = var.ingress_host
  openai_api_key             = var.openai_api_key
  jwt_secret                 = var.jwt_secret
  openai_model               = var.openai_model
  backend_image_repository   = var.backend_image_repository
  backend_image_tag          = var.backend_image_tag
  frontend_image_repository  = var.frontend_image_repository
  frontend_image_tag         = var.frontend_image_tag
  frontend_api_url           = var.frontend_api_url
  backend_replicas           = var.backend_replicas
  frontend_replicas          = var.frontend_replicas
  postgresql_password        = var.postgresql_password
}
