# AI-Driven Learning Platform

A full-stack learning platform that generates personalized AI lessons based on selected topics. Built with .NET 9, React, PostgreSQL, OpenAI, and Docker Compose.

## Tech Stack

| Layer | Technology |
|-------|------------|
| Backend | .NET 9 Web API, EF Core, JWT, BCrypt |
| Frontend | React 19, TypeScript, Tailwind CSS, Framer Motion |
| Database | PostgreSQL 15 |
| AI | OpenAI GPT-4o-mini |
| DevOps | Docker, Docker Compose, Nginx |

## Features

- User registration & JWT authentication
- Topic selection (categories & sub-categories)
- AI-powered lesson generation
- Personal learning history
- Admin dashboard (all users + all prompts)
- Containerized deployment with health checks

## Project Structure

```
ai-learning-platform/
├── backend/                 # .NET REST API
│   ├── Dockerfile
│   └── LearningPlatformApi/
│       ├── Controllers/     # Auth, Prompts, Categories, Admin
│       ├── Services/        # AI, Password hashing
│       ├── Models/          # User, Category, Prompt...
│       ├── Data/            # DbContext, Seeder
│       └── DTOs/            # Request/Response models
├── frontend/                # React SPA
│   ├── Dockerfile
│   └── src/
│       ├── components/      # Layout, ProtectedRoute
│       ├── context/         # AuthContext
│       └── pages...         # Login, Dashboard, History, Admin
├── docker-compose.yaml
└── .env.example
```

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/)
- OpenAI API key

## Quick Start

1. **Clone and configure environment**

```bash
cp .env.example .env
# Edit .env and set your OPENAI_API_KEY
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

## Environment Variables

| Variable | Description | Default |
|----------|-------------|---------|
| `OPENAI_API_KEY` | OpenAI API key | required |
| `JWT_SECRET` | JWT signing key | appsettings default |
| `REACT_APP_API_URL` | Frontend API base URL | `http://localhost:5055/api` |

## Assumptions

- Passwords are hashed with BCrypt before storage
- Admin user is seeded automatically on first startup
- Database migrations run automatically on backend startup
- Frontend communicates with backend via REST + JWT Bearer token
- OpenAI key must be valid for AI generation to work

## Troubleshooting

**Backend crashes on startup:** Wait for PostgreSQL healthcheck, or run `docker compose restart backend`.

**Fresh database:** Reset volumes with `docker compose down -v` then `docker compose up -d --build`.

**CORS errors:** Ensure frontend URL is in backend CORS policy (`localhost:3000`).

## License

Academic / evaluation project.
