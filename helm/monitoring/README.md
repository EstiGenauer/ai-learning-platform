# Helm Chart — Monitoring (Prometheus + Grafana + Loki)

Deploys full observability stack for Kubernetes:

| Subchart | Source | Purpose |
|----------|--------|---------|
| **kube-prometheus-stack** | prometheus-community | Prometheus, Grafana, Alertmanager, Operator |
| **loki** | grafana | Log aggregation |
| **promtail** | grafana | Pod log shipping |

## Prerequisites

- Kubernetes cluster + Helm 3
- NGINX Ingress Controller
- Learning Platform with `monitoring.serviceMonitor.enabled=true`

## Download dependencies

```bash
cd helm/monitoring
helm repo add prometheus-community https://prometheus-community.github.io/helm-charts
helm repo add grafana https://grafana.github.io/helm-charts
helm dependency update
```

## Install

```bash
# 1. Monitoring stack (CRDs + Prometheus + Grafana + Loki)
helm install monitoring . -n monitoring --create-namespace

# 2. Application with metrics scraping
helm install learning-platform ../learning-platform \
  -n learning-platform --create-namespace \
  --set monitoring.serviceMonitor.enabled=true \
  --set secrets.openaiApiKey=sk-your-key \
  --set secrets.jwtSecret=your-jwt-secret
```

## Access Grafana

Hosts file:

```
127.0.0.1 grafana.learning-platform.local
```

Open: http://grafana.learning-platform.local  
Login: `admin` / `admin`

### Datasources (pre-configured)

- **Prometheus** — metrics from `/metrics`, nodes, pods
- **Loki** — container logs (Explore → Loki)

### Dashboards

- Kubernetes defaults (CPU, memory, pods)
- **Learning Platform Overview** (custom)

## Log queries (Grafana Explore)

```logql
{namespace="learning-platform"}
{namespace="learning-platform", container="backend"} |= "error"
```

## Custom values

```yaml
kube-prometheus-stack:
  grafana:
    adminPassword: "strong-password"
  prometheus:
    prometheusSpec:
      retention: 15d

loki:
  singleBinary:
    persistence:
      size: 20Gi
```

## Backend metrics & logs

- Metrics: `GET /metrics` (OpenTelemetry → Prometheus)
- Logs: Serilog → stdout → Promtail → Loki

## Uninstall

```bash
helm uninstall monitoring -n monitoring
```
