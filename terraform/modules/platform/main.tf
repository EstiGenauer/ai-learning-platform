resource "kubernetes_namespace" "this" {
  metadata {
    name = var.namespace
    labels = {
      "app.kubernetes.io/name" = "learning-platform"
    }
  }
}

resource "helm_release" "ingress_nginx" {
  count = var.install_ingress_controller ? 1 : 0

  name             = "ingress-nginx"
  repository       = "https://kubernetes.github.io/ingress-nginx"
  chart            = "ingress-nginx"
  namespace        = "ingress-nginx"
  create_namespace = true
  version          = "4.11.3"

  set {
    name  = "controller.ingressClassResource.name"
    value = "nginx"
  }

  set {
    name  = "controller.ingressClassResource.default"
    value = "true"
  }
}

resource "helm_release" "learning_platform" {
  name       = var.release_name
  namespace  = kubernetes_namespace.this.metadata[0].name
  chart      = var.helm_chart_path
  wait       = true
  timeout    = 600
  max_history = 5

  depends_on = [helm_release.ingress_nginx]

  set_sensitive {
    name  = "secrets.openaiApiKey"
    value = var.openai_api_key
  }

  set_sensitive {
    name  = "secrets.jwtSecret"
    value = var.jwt_secret
  }

  set_sensitive {
    name  = "postgresql.auth.password"
    value = var.postgresql_password
  }

  set {
    name  = "ingress.enabled"
    value = "true"
  }

  set {
    name  = "ingress.className"
    value = "nginx"
  }

  set {
    name  = "ingress.host"
    value = var.ingress_host
  }

  set {
    name  = "backend.replicaCount"
    value = tostring(var.backend_replicas)
  }

  set {
    name  = "backend.openaiModel"
    value = var.openai_model
  }

  set {
    name  = "backend.image.repository"
    value = var.backend_image_repository
  }

  set {
    name  = "backend.image.tag"
    value = var.backend_image_tag
  }

  set {
    name  = "frontend.replicaCount"
    value = tostring(var.frontend_replicas)
  }

  set {
    name  = "frontend.image.repository"
    value = var.frontend_image_repository
  }

  set {
    name  = "frontend.image.tag"
    value = var.frontend_image_tag
  }

  set {
    name  = "frontend.apiUrl"
    value = var.frontend_api_url
  }

  set {
    name  = "monitoring.serviceMonitor.enabled"
    value = tostring(var.enable_service_monitor)
  }

  set {
    name  = "monitoring.prometheusReleaseName"
    value = var.prometheus_release_name
  }
}
