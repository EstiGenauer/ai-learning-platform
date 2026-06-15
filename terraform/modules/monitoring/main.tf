resource "helm_release" "monitoring" {
  name             = var.release_name
  namespace        = var.namespace
  chart            = var.helm_chart_path
  create_namespace = true
  wait             = true
  timeout          = 900
  max_history      = 5

  set_sensitive {
    name  = "kube-prometheus-stack.grafana.adminPassword"
    value = var.grafana_admin_password
  }

  set {
    name  = "kube-prometheus-stack.grafana.ingress.enabled"
    value = "true"
  }

  set {
    name  = "kube-prometheus-stack.grafana.ingress.ingressClassName"
    value = "nginx"
  }

  set {
    name  = "kube-prometheus-stack.grafana.ingress.hosts[0]"
    value = var.grafana_host
  }

  set {
    name  = "kube-prometheus-stack.prometheus.prometheusSpec.retention"
    value = var.prometheus_retention
  }

  set {
    name  = "kube-prometheus-stack.prometheus.prometheusSpec.serviceMonitorSelectorNilUsesHelmValues"
    value = "false"
  }

  set {
    name  = "grafanaDashboards.enabled"
    value = "true"
  }
}
