# Helm Chart — AI Learning Platform

Deploys the full stack with **Bitnami PostgreSQL** as a subchart dependency.

## Prerequisites

- Kubernetes cluster + Helm 3
- NGINX Ingress Controller
- Docker images built (see below)

## Download PostgreSQL chart dependency

From the repository root:

```bash
cd helm/learning-platform
helm repo add bitnami https://charts.bitnami.com/bitnami
helm dependency update
```

This downloads the Bitnami PostgreSQL chart into `charts/postgresql-*.tgz`.

## Build Docker images

```bash
docker build -t learning-platform-backend:latest ./backend
docker build -t learning-platform-frontend:latest --build-arg REACT_APP_API_URL=/api ./frontend
```

## Install

```bash
helm install learning-platform ./helm/learning-platform \
  --namespace learning-platform \
  --create-namespace \
  --set secrets.openaiApiKey=sk-your-key-here \
  --set secrets.jwtSecret=your-jwt-secret-min-32-chars
```

## Custom values

Create `my-values.yaml`:

```yaml
secrets:
  openaiApiKey: "sk-..."
  jwtSecret: "your-production-jwt-secret"

backend:
  openaiModel: gpt-4o
  replicaCount: 2

postgresql:
  auth:
    password: "strong-db-password"
  primary:
    persistence:
      size: 20Gi

ingress:
  host: learning.example.com
```

```bash
helm upgrade --install learning-platform ./helm/learning-platform \
  -f my-values.yaml \
  --namespace learning-platform
```

## Upgrade / Uninstall

```bash
helm upgrade learning-platform ./helm/learning-platform -f my-values.yaml
helm uninstall learning-platform -n learning-platform
```

## Chart structure

| Template | Description |
|----------|-------------|
| `templates/backend.yaml` | Backend Deployment + Service |
| `templates/frontend.yaml` | Frontend Deployment + Service |
| `templates/ingress.yaml` | NGINX Ingress rules |
| `templates/configmap.yaml` | App configuration |
| `templates/secret.yaml` | OpenAI key + JWT secret |
| `charts/postgresql` | Bitnami PostgreSQL (dependency) |

## Values reference

See `values.yaml` for all configurable options including PostgreSQL persistence, replica counts, and resource limits.
