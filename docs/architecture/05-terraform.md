# Terraform Architecture

Infrastructure as Code layer on top of Kubernetes + Helm.

## Purpose

Terraform automates what you would otherwise run manually:

1. Create/connect to a Kubernetes cluster
2. Install NGINX Ingress Controller
3. Deploy the `helm/learning-platform` chart with secrets and image settings

## Module structure

```mermaid
flowchart TB
    subgraph Environments
        LOCAL[environments/local]
        AWS[environments/aws]
    end

    subgraph Module["modules/platform"]
        NS[kubernetes_namespace]
        NGX[helm: ingress-nginx]
        APP[helm: learning-platform]
    end

    LOCAL --> Module
    AWS --> Module

    NS --> NGX --> APP
```

## Local environment

Uses existing cluster (Docker Desktop K8s, minikube) or optional **kind** cluster.

```mermaid
flowchart LR
    TF[Terraform apply] --> K8S[Kubernetes cluster]
    TF --> NGX2[NGINX Ingress]
    TF --> HM[Helm: learning-platform]
    HM --> BE[Backend pods]
    HM --> FE[Frontend pods]
    HM --> PG[PostgreSQL pods]
```

| Input | Example |
|-------|---------|
| `openai_api_key` | sk-... |
| `ingress_host` | learning-platform.local |
| `backend_image_repository` | learning-platform-backend |
| `create_kind_cluster` | false (default) |

## AWS environment

Creates full cloud infrastructure:

```mermaid
flowchart TB
    TF2[Terraform apply]

    subgraph AWSCloud
        VPC[VPC + subnets]
        NAT[NAT Gateway]
        EKS[EKS Cluster]
        NG[Managed Node Group]
    end

    subgraph K8sApp
        NGX3[NGINX Ingress<br/>LoadBalancer]
        HM2[Helm release]
    end

    TF2 --> VPC
    VPC --> EKS
    EKS --> NG
    TF2 --> K8sApp
    NGX3 -->|public DNS| USER2[Users]
```

| Resource | Module |
|----------|--------|
| VPC, subnets, NAT | `terraform-aws-modules/vpc` |
| EKS cluster + nodes | `terraform-aws-modules/eks` |
| App deployment | `modules/platform` |

## State & secrets

```mermaid
flowchart LR
    TFVARS[terraform.tfvars<br/>gitignored] --> TF3[Terraform]
    TF3 --> STATE[*.tfstate<br/>gitignored]
    TF3 --> K8S2[Kubernetes API]
    K8S2 --> SECK8[Kubernetes Secrets]
```

Never commit `terraform.tfvars` or state files with real API keys.

## Relationship to other deploy methods

| Method | Scope | Best for |
|--------|-------|----------|
| Docker Compose | Single machine | Dev, evaluation |
| k8s/ YAML | Existing cluster | Learning K8s |
| Helm | Existing cluster | Production K8s |
| Terraform | Cluster + app | Repeatable IaC, AWS EKS |

All paths deploy the **same application** — different orchestration layers.
