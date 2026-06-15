# Docker Compose Architecture

Primary deployment method for local development and project evaluation.

## Container topology

```mermaid
flowchart TB
    subgraph Host["Developer machine"]
        subgraph Compose["docker compose"]
            WEB[learning_platform_web<br/>frontend :3000→80]
            API[learning_platform_api<br/>backend :5055→8080]
            DB[learning_platform_db<br/>postgres :5434→5432]
        end
    end

    USER[User browser] -->|localhost:3000| WEB
    WEB -->|REACT_APP_API_URL<br/>localhost:5055/api| API
    API -->|Connection string| DB
    API -->|HTTPS| OAI[OpenAI API]

    VOL[(pgdata volume)] --- DB
```

## Service dependencies

```mermaid
flowchart LR
    DB[db] -->|healthy| API[backend]
    API --> FE[frontend]
```

- **db** starts first; healthcheck `pg_isready`
- **backend** waits for db healthy, then starts API
- **frontend** depends on backend (no health gate)

## Environment variables

| Variable | Service | Purpose |
|----------|---------|---------|
| `OPENAI_API_KEY` | backend | OpenAI authentication |
| `OPENAI_MODEL` | backend | Model name (default gpt-4o-mini) |
| `JWT_SECRET` | backend | JWT signing key |
| `POSTGRES_*` | db | Database credentials |
| `REACT_APP_API_URL` | frontend build | API base URL |

Configuration file: `.env` (from `.env.example`)

## Integration test stack

Separate compose file: `docker-compose.test.yml`

```mermaid
flowchart LR
    TDB[(test postgres)] --> TAPI[backend<br/>USE_FAKE_AI=true]
    TAPI --> TRUN[tests container<br/>dotnet test]
```

- Uses `FakeAiService` — no OpenAI key needed
- Runs in CI backend pipeline (job: integration-tests)

## Commands

```bash
# Start
docker compose up -d --build

# Reset database
docker compose down -v && docker compose up -d --build

# Logs
docker compose logs -f backend
```
