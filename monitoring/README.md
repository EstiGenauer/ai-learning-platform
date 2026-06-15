# Local observability stack for Docker Compose

Used by `docker compose --profile monitoring up -d`.

| Service | URL | Purpose |
|---------|-----|---------|
| Grafana | http://localhost:3001 | Dashboards (metrics + logs) |
| Prometheus | http://localhost:9090 | Metrics storage |
| Loki | http://localhost:3100 | Log aggregation |
| Promtail | — | Ships Docker container logs to Loki |

## Default Grafana login

- User: `admin`
- Password: `admin`

## Pre-configured datasources

- **Prometheus** — scrapes `backend:8080/metrics`
- **Loki** — container logs from all Compose services

## Dashboard

**Learning Platform Overview** — HTTP rate, latency, and live logs.

## Start with monitoring

```bash
docker compose --profile monitoring up -d --build
```

App only (no monitoring):

```bash
docker compose up -d --build
```
