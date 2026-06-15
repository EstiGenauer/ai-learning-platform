# Kubernetes & Helm Architecture

Two deployment options for Kubernetes clusters.

## Option A — Plain manifests (`k8s/`)

```mermaid
flowchart TB
    subgraph NS["Namespace: learning-platform"]
        CM[ConfigMap]
        SEC[Secret]
        PG[PostgreSQL Deployment + PVC]
        BE[Backend Deployment + Service]
        FE[Frontend Deployment + Service]
        ING[Ingress]
    end

    subgraph Cluster
        NGX[NGINX Ingress Controller]
    end

    USER[User] --> NGX
    NGX --> ING
    ING -->|/api| BE
    ING -->|/| FE
    BE --> PG
    BE --> SEC
    FE --> CM
```

## Option B — Helm chart (`helm/learning-platform/`)

Recommended for production. Includes **Bitnami PostgreSQL** subchart.

```mermaid
flowchart TB
    subgraph HelmRelease["Helm release: learning-platform"]
        subgraph App
            BED[Backend Deployment x2]
            FED[Frontend Deployment x2]
            BES[Backend Service]
            FES[Frontend Service]
            CFG[ConfigMap]
            SEC2[Secret]
            ING2[Ingress]
        end

        subgraph Subchart["Bitnami PostgreSQL"]
            PG2[StatefulSet]
            SVC[ClusterIP Service]
            PVC[PVC 8Gi]
        end
    end

    ING2 --> BED
    ING2 --> FED
    BED --> SVC
    BED --> SEC2
    FED --> CFG
    SVC --> PG2
    PG2 --> PVC
```

## Ingress routing

```mermaid
flowchart LR
    HOST[learning-platform.local] --> ING[Ingress]

    ING -->|path /api| BE[backend:8080]
    ING -->|path /| FE[frontend:80]
```

Frontend built with `REACT_APP_API_URL=/api` so API calls go through the same host (no CORS issues).

## Helm values (key settings)

| Value | Default | Description |
|-------|---------|-------------|
| `secrets.openaiApiKey` | placeholder | OpenAI key |
| `secrets.jwtSecret` | placeholder | JWT secret |
| `backend.replicaCount` | 2 | API pods |
| `frontend.replicaCount` | 2 | UI pods |
| `ingress.host` | learning-platform.local | Hostname |
| `postgresql.enabled` | true | Bundled Postgres |

## Deploy with Helm

```bash
cd helm/learning-platform
helm dependency update
helm install learning-platform . \
  --namespace learning-platform \
  --create-namespace \
  --set secrets.openaiApiKey=sk-... \
  --set secrets.jwtSecret=your-secret
```

## Health checks

- Backend: `GET /health` on port 8080
- Readiness/liveness probes configured in Helm templates
- DB migrations run on backend startup (same as Compose)
