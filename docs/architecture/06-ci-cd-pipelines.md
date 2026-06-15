# CI/CD Pipelines

Two separate GitHub Actions workflows with **path-based triggers** — backend and frontend changes run independently.

## Pipeline overview

```mermaid
flowchart TB
    subgraph Triggers
        PUSH[Push to main/master/ci-cd]
        PR[Pull Request]
    end

    subgraph Backend["ci-backend.yaml"]
        B1[Unit tests<br/>dotnet test]
        B2[Build Docker image]
        B3[Integration tests<br/>docker-compose.test.yml]
        B4[Push DockerHub + GHCR]
        B5[Git tag v0.0.x]
        B6[Email notification]
    end

    subgraph Frontend["ci-frontend.yaml"]
        F1[Unit tests<br/>npm test]
        F2[Build Docker image]
        F3[Push DockerHub + GHCR]
        F4[Email notification]
    end

    PUSH --> Backend
    PUSH --> Frontend
    PR --> Backend
    PR --> Frontend

    B1 --> B2 --> B3 --> B4 --> B5 --> B6
    F1 --> F2 --> F3 --> F4
```

## Path triggers

| Workflow | Triggers when changed |
|----------|----------------------|
| `ci-backend.yaml` | `backend/**`, `tests/**`, `docker-compose.test.yml` |
| `ci-frontend.yaml` | `frontend/**` |

## Backend pipeline detail

```mermaid
flowchart LR
    A[checkout] --> B[.NET 9 restore/build/test]
    B --> C[Docker build backend]
    C --> D[Compose integration tests]
    D --> E{Push to main?}
    E -->|Yes| F[Push images]
    E -->|No| G[Stop]
    F --> H[Auto tag v0.0.N]
    F --> I[Email summary]
```

### Image tags (on push to main)

| Registry | Tag pattern |
|----------|-------------|
| Docker Hub | `{DOCKERHUB_USERNAME}/learning-platform-backend:latest` |
| Docker Hub | `{DOCKERHUB_USERNAME}/learning-platform-backend:{sha}` |
| Docker Hub | `{DOCKERHUB_USERNAME}/learning-platform-backend:v0.0.x` |
| GHCR | `ghcr.io/EstiGenauer/ai-learning-platform/backend:latest` |

## Frontend pipeline detail

```mermaid
flowchart LR
    A2[checkout] --> B2[npm ci + test]
    B2 --> C2[Docker build frontend]
    C2 --> D2{Push to main?}
    D2 -->|Yes| E2[Push images]
    E2 --> F2[Email summary]
```

Build arg: `REACT_APP_API_URL=http://localhost:5055/api` (Compose default)

## Required GitHub Secrets

| Secret | Used by | Required |
|--------|---------|----------|
| `DOCKERHUB_USERNAME` | Both pipelines | Yes (for push) |
| `DOCKERHUB_TOKEN` | Both pipelines | Yes (for push) |
| `EMAIL_USERNAME` | Notifications | Optional |
| `EMAIL_PASSWORD` | Notifications | Optional |
| `GITHUB_TOKEN` | GHCR push | Auto-provided |

## How CI connects to deployment

```mermaid
flowchart LR
    DEV[Developer push] --> GH[GitHub Actions]
    GH --> DH[Docker Hub / GHCR]
    DH --> DC[docker compose pull]
    DH --> K8S[kubectl / Helm]
    DH --> TF[Terraform<br/>image vars]
    DH --> EKS[AWS EKS]
```

1. **CI** builds and pushes Docker images on every main-branch push
2. **Docker Compose** uses locally built images (`docker compose build`)
3. **Kubernetes / Helm / Terraform** pull from registry using image repository + tag from values/tfvars

## Local testing (same as CI)

```powershell
# Backend
cd backend && dotnet test

# Frontend
cd frontend && npm test -- --watchAll=false

# Integration
docker compose -f docker-compose.test.yml up --build --abort-on-container-exit --exit-code-from tests

# All (Windows)
.\scripts\run-all-tests.ps1
```

## Notifications

Both pipelines send email via Gmail SMTP (`dawidd6/action-send-mail`) to the configured address. Fails gracefully if email secrets are not set (`continue-on-error: true`).
