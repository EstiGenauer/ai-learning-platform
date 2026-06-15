# System Overview

## High-level architecture

```mermaid
flowchart TB
    subgraph Users
        U[Browser / User]
    end

    subgraph Frontend["Frontend — React + Nginx"]
        FE[SPA :3000 / :80]
    end

    subgraph Backend["Backend — .NET 9 API"]
        API[REST API :5055 / :8080]
        JWT[JWT Auth]
        AI[AI Service]
    end

    subgraph Data
        PG[(PostgreSQL 15)]
    end

    subgraph External
        OAI[OpenAI API]
    end

    U -->|HTTPS/HTTP| FE
    FE -->|REST + JWT Bearer| API
    API --> JWT
    API --> PG
    API --> AI
    AI --> OAI
```

## Technology stack

| Layer | Technology | Role |
|-------|------------|------|
| Frontend | React 19, TypeScript, Tailwind | UI, routing, auth context |
| Backend | .NET 9, EF Core, BCrypt | REST API, business logic |
| Database | PostgreSQL 15 | Users, categories, prompts, history |
| AI | OpenAI (gpt-4o / gpt-4o-mini) | Lesson generation |
| DevOps | Docker Compose, K8s, Helm, Terraform, GitHub Actions | Build, test, deploy |

## Repository structure

```mermaid
flowchart LR
    subgraph Code
        BE[backend/]
        FE2[frontend/]
    end

    subgraph Deploy
        DC[docker-compose.yaml]
        K8[k8s/]
        HM[helm/learning-platform/]
        TF[terraform/]
    end

    subgraph CI[".github/workflows/"]
        CB[ci-backend.yaml]
        CF[ci-frontend.yaml]
    end

    subgraph Docs
        DOC[docs/architecture/]
    end

    BE --> DC
    FE2 --> DC
    BE --> K8
    FE2 --> K8
    HM --> TF
    BE --> CB
    FE2 --> CF
```

## Default ports (Docker Compose)

| Service | Host port | Container port |
|---------|-----------|----------------|
| Frontend | 3000 | 80 |
| Backend API | 5055 | 8080 |
| PostgreSQL | 5434 | 5432 |
| Swagger | 5055/swagger | — |

## Security model

```mermaid
flowchart LR
    subgraph Public
        REG[POST /Auth/register]
        LOG[POST /Auth/login]
        CAT[GET /Categories]
    end

    subgraph Authenticated["JWT required"]
        PR[POST /Prompts]
        HI[GET /Prompts/history]
    end

    subgraph Admin["JWT + Admin role"]
        AD[GET/POST/DELETE /Admin/*]
    end

    LOG -->|returns JWT| PR
    LOG -->|returns JWT| AD
```

- Passwords hashed with **BCrypt** before storage
- JWT signed with configurable secret (`Jwt:Key` / `JWT_SECRET`)
- Admin user seeded on first startup: `admin@admin.com`
