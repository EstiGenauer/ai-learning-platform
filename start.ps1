# Quick start script for Windows
Write-Host "=== AI Learning Platform - Starting ===" -ForegroundColor Cyan

if (-not (Test-Path ".env")) {
    Copy-Item ".env.example" ".env"
    Write-Host "Created .env file - add your OPENAI_API_KEY if you want AI lessons" -ForegroundColor Yellow
}

Write-Host "Stopping old containers..." -ForegroundColor Gray
docker compose down -v

Write-Host "Building and starting (may take 2-3 minutes)..." -ForegroundColor Gray
docker compose up -d --build

Write-Host "Waiting for backend health..." -ForegroundColor Gray
$ready = $false
for ($i = 1; $i -le 30; $i++) {
    try {
        $r = Invoke-WebRequest -Uri "http://localhost:5055/health" -UseBasicParsing -TimeoutSec 3
        if ($r.StatusCode -eq 200) { $ready = $true; break }
    } catch { Start-Sleep -Seconds 3 }
}

if ($ready) {
    Write-Host "`n SUCCESS! Open http://localhost:3000" -ForegroundColor Green
    Write-Host " Admin: admin@admin.com / Admin123!" -ForegroundColor Green
} else {
    Write-Host "`n Backend not ready yet. Run: docker logs learning_platform_api" -ForegroundColor Red
}

docker compose ps
