# Kubernetes Manifests — AI Learning Platform

Plain Kubernetes YAML for deploying the full stack without Helm.

## Prerequisites

- Kubernetes cluster (minikube, kind, AKS, EKS, etc.)
- NGINX Ingress Controller installed
- Docker images built locally or pushed to a registry

## Build images

```bash
docker build -t learning-platform-backend:latest ./backend
docker build -t learning-platform-frontend:latest --build-arg REACT_APP_API_URL=/api ./frontend
```

For minikube, load images into the cluster:

```bash
minikube image load learning-platform-backend:latest
minikube image load learning-platform-frontend:latest
```

## Deploy

```bash
# 1. Create namespace
kubectl apply -f namespace.yaml

# 2. Create secrets (copy example and edit first)
cp secret.example.yaml secret.yaml
# Edit secret.yaml with real OPENAI_API_KEY
kubectl apply -f secret.yaml

# 3. Apply resources
kubectl apply -f configmap.yaml
kubectl apply -f postgres.yaml
kubectl apply -f backend.yaml
kubectl apply -f frontend.yaml
kubectl apply -f ingress.yaml
```

## Access

Add to hosts file:

```
127.0.0.1 learning-platform.local
```

For minikube:

```bash
minikube tunnel
# or: kubectl port-forward -n learning-platform svc/frontend 3000:80
```

Open: http://learning-platform.local

## Useful commands

```bash
kubectl get pods -n learning-platform
kubectl logs -n learning-platform deploy/backend
kubectl describe ingress -n learning-platform learning-platform-ingress
```

## Notes

- PostgreSQL uses a 5Gi PVC — adjust `postgres.yaml` for your storage class.
- For production, use the **Helm chart** (`helm/learning-platform`) with Bitnami PostgreSQL instead of the manual postgres manifest.
