# AI-Driven Learning Platform

A full-stack learning platform that generates personalized AI lessons based on selected topics. Built with .NET 9, React, PostgreSQL, OpenAI, Docker Compose, Kubernetes, and Helm.

> Academic evaluation project — see `פרויקט הערכה (1) (1).pdf` for the original requirements.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Backend | .NET 9 Web API, EF Core, JWT, BCrypt, xUnit |
| Frontend | React 19, TypeScript, Tailwind CSS, Framer Motion |
| Database | PostgreSQL 15 |
| AI | OpenAI GPT-4o / GPT-4o-mini |
| DevOps | Docker Compose, Kubernetes, Helm, GitHub Actions CI |

## Features

- User registration & JWT authentication
- Topic selection (40+ categories & 250+ sub-categories, auto-seeded)
- AI-powered lesson generation
- Personal learning history
- Admin dashboard (users, categories, prompts)
- Containerized deployment with health checks
- Unit & integration tests
- Kubernetes manifests + Helm chart (Bitnami PostgreSQL)

## Project Structure

```
ai-learning-platform/
├── backend/
│   ├── LearningPlatformApi.sln
│   ├── LearningPlatformApi/          # .NET REST API
│   └── LearningPlatformApi.Tests/    # xUnit unit + integration tests
├── frontend/                         # React SPA
├── k8s/                              # Plain Kubernetes manifests
├── helm/learning-platform/           # Helm chart + PostgreSQL dependency
├── .github/workflows/ci.yml          # CI pipeline
├── .github/workflows/docker-publish.yml
├── docker-compose.yaml
├── docker-compose.test.yml           # Integration tests in Docker
├── tests/compose/run-tests.sh
├── scripts/run-all-tests.ps1
└── .env.example
```

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- OpenAI API key
- (Optional) Kubernetes + Helm 3 for cluster deployment

## Quick Start

1. **Clone and configure environment**

```bash
cp .env.example .env
# Edit .env — set OPENAI_API_KEY and OPENAI_MODEL=gpt-4o if needed
```

2. **Start all services**

```bash
docker compose up -d --build
```

3. **Open the app**

| Service | URL |
|---------|-----|
| Frontend | http://localhost:3000 |
| Backend API | http://localhost:5055 |
| Swagger | http://localhost:5055/swagger |
| PostgreSQL | localhost:5434 |

## Default Admin Account

| Field | Value |
|-------|-------|
| Email | admin@admin.com |
| Password | Admin123! |

## Running Tests

### 1. Backend unit + API integration tests (14 tests)

```bash
cd backend
dotnet test LearningPlatformApi.sln
```

Covers: password hashing, category seed data, DB seeder, auth flow, categories API, prompts API, health endpoint.

### 2. Frontend unit tests

```bash
cd frontend
npm ci
npm test -- --watchAll=false
```

### 3. Docker Compose integration tests (real Postgres + API)

Runs the backend in Docker with `USE_FAKE_AI=true` (no OpenAI key needed):

```bash
docker compose -f docker-compose.test.yml up --build --abort-on-container-exit --exit-code-from tests
docker compose -f docker-compose.test.yml down -v
```

Or run everything at once on Windows:

```powershell
.\scripts\run-all-tests.ps1
```

### CI

GitHub Actions on push/PR to `main`:
- Backend xUnit tests
- Frontend tests
- Docker Compose integration tests
- Docker image build

## Docker Images (GitHub / Docker Hub)

Images are published automatically to **GitHub Container Registry** on every push to `main`:

- `ghcr.io/EstiGenauer/ai-learning-platform/backend:latest`
- `ghcr.io/EstiGenauer/ai-learning-platform/frontend:latest`

**Do not share Docker Hub passwords in chat.** Add secrets in GitHub:

1. Repo → **Settings** → **Secrets and variables** → **Actions**
2. Add:
   - `DOCKERHUB_USERNAME` — your Docker Hub username
   - `DOCKERHUB_TOKEN` — access token from [hub.docker.com/settings/security](https://hub.docker.com/settings/security)

Then run **Actions → Docker Publish → Run workflow** to push to Docker Hub manually.

| Secret | Purpose |
|--------|---------|
| `GITHUB_TOKEN` | Built-in — pushes to GHCR automatically |
| `DOCKERHUB_USERNAME` | Optional — Docker Hub username |
| `DOCKERHUB_TOKEN` | Optional — Docker Hub access token |

## Local Development (without Docker)

### Backend

```bash
cd backend/LearningPlatformApi
dotnet run
```

### Frontend

```bash
cd frontend
npm install
npm start
```

Set `REACT_APP_API_URL=http://localhost:5055/api` in `frontend/.env` if needed.

## API Endpoints

| Method | Endpoint | Auth | Description |
|--------|----------|------|-------------|
| POST | `/api/Auth/register` | No | Register new user |
| POST | `/api/Auth/login` | No | Login, returns JWT |
| GET | `/api/Categories` | No | List categories + sub-categories |
| POST | `/api/Prompts` | Yes | Generate AI lesson |
| GET | `/api/Prompts/history` | Yes | User's learning history |
| GET | `/api/Admin/users` | Admin | All users |
| GET | `/api/Admin/prompts` | Admin | All prompts |
| GET/POST/DELETE | `/api/Admin/categories` | Admin | Manage categories |
| POST | `/api/Admin/categories/{id}/subcategories` | Admin | Add sub-category |
| DELETE | `/api/Admin/subcategories/{id}` | Admin | Delete sub-category |

Use `backend/LearningPlatformApi/LearningPlatformApi.http` for VS Code / Rider REST Client testing.

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `OPENAI_API_KEY` | OpenAI API key | required |
| `OPENAI_MODEL` | OpenAI model name | `gpt-4o-mini` (use `gpt-4o` if mini unavailable) |
| `JWT_SECRET` | JWT signing key | appsettings default |
| `REACT_APP_API_URL` | Frontend API base URL | `http://localhost:5055/api` |

## Kubernetes Deployment

Plain YAML manifests in `k8s/`:

```bash
docker build -t learning-platform-backend:latest ./backend
docker build -t learning-platform-frontend:latest --build-arg REACT_APP_API_URL=/api ./frontend

kubectl apply -f k8s/namespace.yaml
cp k8s/secret.example.yaml k8s/secret.yaml   # edit secrets first
kubectl apply -f k8s/secret.yaml
kubectl apply -f k8s/configmap.yaml
kubectl apply -f k8s/postgres.yaml
kubectl apply -f k8s/backend.yaml
kubectl apply -f k8s/frontend.yaml
kubectl apply -f k8s/ingress.yaml
```

See `k8s/README.md` for full instructions.

## Helm Deployment (recommended for K8s)

Includes **Bitnami PostgreSQL** subchart:

```bash
cd helm/learning-platform
helm repo add bitnami https://charts.bitnami.com/bitnami
helm dependency update

helm install learning-platform . \
  --namespace learning-platform \
  --create-namespace \
  --set secrets.openaiApiKey=sk-your-key \
  --set secrets.jwtSecret=your-jwt-secret
```

See `helm/learning-platform/README.md` for values and upgrades.

## Assumptions

- Passwords are hashed with BCrypt before storage
- Admin user and categories are seeded automatically on startup
- Database migrations run automatically on backend startup
- Frontend communicates with backend via REST + JWT Bearer token
- OpenAI key must be valid and model must be enabled on your OpenAI project

## Troubleshooting

**Backend crashes on startup:** Wait for PostgreSQL healthcheck, or run `docker compose restart backend`.

**AI service error:** Set `OPENAI_MODEL=gpt-4o` in `.env` if your OpenAI project lacks `gpt-4o-mini` access.

**Fresh database:** Reset volumes with `docker compose down -v` then `docker compose up -d --build`.

**CORS errors:** Ensure frontend URL is in backend CORS policy (`localhost:3000`).

## License

Academic / evaluation project.

