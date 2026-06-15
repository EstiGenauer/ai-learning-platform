# Documentation — AI Learning Platform

תיעוד ארכיטקטורה, דיאגרמות וזרימות עבודה של הפרויקט.

## Architecture (`architecture/`)

| Document | Description |
|----------|-------------|
| [System Overview](architecture/01-system-overview.md) | High-level stack, components, ports |
| [Application Flow](architecture/02-application-flow.md) | Auth, lessons, admin — request flows |
| [Docker Compose](architecture/03-docker-compose.md) | Local dev deployment |
| [Kubernetes & Helm](architecture/04-kubernetes-helm.md) | Cluster deployment |
| [Terraform](architecture/05-terraform.md) | Infrastructure as Code (local + AWS EKS) |
| [CI/CD Pipelines](architecture/06-ci-cd-pipelines.md) | GitHub Actions workflows |

## Quick links

- Main README: [../README.md](../README.md)
- K8s manifests: [../k8s/README.md](../k8s/README.md)
- Helm chart: [../helm/learning-platform/README.md](../helm/learning-platform/README.md)
- Terraform: [../terraform/README.md](../terraform/README.md)

## Viewing diagrams

All diagrams use [Mermaid](https://mermaid.js.org/). They render automatically on **GitHub** when viewing the markdown files.

In VS Code / Cursor, install the **Markdown Preview Mermaid Support** extension for live preview.
