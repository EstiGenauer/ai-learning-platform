# Terraform — AI Learning Platform

Infrastructure as Code for deploying the platform to **Kubernetes** using the existing **Helm chart** (`helm/learning-platform`).

## Layout

```
terraform/
├── modules/
│   └── platform/          # Namespace + NGINX Ingress + Helm release
└── environments/
    ├── local/             # Docker Desktop / minikube / kind
    └── aws/               # AWS EKS cluster + app deployment
```

## Prerequisites

- [Terraform](https://www.terraform.io/downloads) >= 1.5
- Kubernetes cluster **or** (local only) [kind](https://kind.sigs.k8s.io/)
- Docker images built or pushed to a registry
- Helm chart dependencies already vendored (`helm/learning-platform/charts/`)

## Local deployment (recommended for demo)

Works with **Docker Desktop Kubernetes**, **minikube**, or an optional **kind** cluster.

### 1. Enable Kubernetes

- **Docker Desktop:** Settings → Kubernetes → Enable
- **minikube:** `minikube start`

### 2. Build images

```bash
docker build -t learning-platform-backend:latest ./backend
docker build -t learning-platform-frontend:latest --build-arg REACT_APP_API_URL=/api ./frontend
```

For minikube/kind, load images into the cluster:

```bash
minikube image load learning-platform-backend:latest
minikube image load learning-platform-frontend:latest

# or with kind:
kind load docker-image learning-platform-backend:latest --name learning-platform
kind load docker-image learning-platform-frontend:latest --name learning-platform
```

### 3. Configure variables

```bash
cd terraform/environments/local
cp terraform.tfvars.example terraform.tfvars
# Edit terraform.tfvars — set openai_api_key
```

### 4. Deploy

```bash
terraform init
terraform plan
terraform apply
```

### 5. Access

Add to your hosts file:

```
127.0.0.1 learning-platform.local
127.0.0.1 grafana.learning-platform.local
```

Open: http://learning-platform.local  
Grafana: http://grafana.learning-platform.local

```bash
kubectl get pods -n learning-platform
kubectl get ingress -n learning-platform
```

### Optional: create kind cluster via Terraform

In `terraform.tfvars`:

```hcl
create_kind_cluster = true
kind_cluster_name   = "learning-platform"
```

Requires `kind` CLI installed.

## AWS deployment (EKS)

Provisions VPC, EKS cluster, managed node group, NGINX Ingress, and the Helm release.

### Prerequisites

- AWS account + credentials (`aws configure`)
- Images pushed to Docker Hub or GHCR (EKS must pull from a registry)
- DNS hostname for `ingress_host`

### Deploy

```bash
cd terraform/environments/aws
cp terraform.tfvars.example terraform.tfvars
# Edit: ingress_host, image repos, secrets

terraform init
terraform plan
terraform apply
```

After apply:

```bash
aws eks update-kubeconfig --region eu-west-1 --name learning-platform-eks
kubectl get svc -n ingress-nginx ingress-nginx-controller
```

Point your DNS record to the LoadBalancer hostname, then open `https://your-domain` (add TLS separately if needed).

### Cost note

EKS + NAT Gateway incur AWS charges. Use `single_nat_gateway = true` and small node groups for demos. Destroy when done:

```bash
terraform destroy
```

## What Terraform creates

| Resource | local | aws |
|----------|-------|-----|
| Kubernetes namespace | ✓ | ✓ |
| NGINX Ingress Controller (Helm) | ✓ | ✓ |
| learning-platform Helm release | ✓ | ✓ |
| Bitnami PostgreSQL (subchart) | ✓ | ✓ |
| Prometheus + Grafana (Helm) | ✓ optional | ✓ optional |
| VPC | — | ✓ |
| EKS cluster + nodes | — | ✓ |

## Secrets

Never commit `terraform.tfvars` with real keys. Files are gitignored:

- `terraform/**/terraform.tfvars`
- `terraform/**/.terraform/`
- `terraform/**/*.tfstate*`

## Destroy

```bash
terraform destroy
```

## Related docs

- Plain K8s YAML: `k8s/README.md`
- Helm chart: `helm/learning-platform/README.md`
- Docker Compose (evaluation requirement): root `README.md`
