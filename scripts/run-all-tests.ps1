# Run unit/integration tests locally, then Docker Compose integration tests.
# Requires Docker Desktop.

$ErrorActionPreference = "Stop"
$Root = Split-Path -Parent $PSScriptRoot

Write-Host "=== Backend unit + integration tests ===" -ForegroundColor Cyan
dotnet test "$Root\backend\LearningPlatformApi.sln" -c Release --verbosity minimal

Write-Host "`n=== Frontend unit tests ===" -ForegroundColor Cyan
Push-Location "$Root\frontend"
npm test -- --watchAll=false --passWithNoTests
Pop-Location

Write-Host "`n=== Docker Compose integration tests ===" -ForegroundColor Cyan
Push-Location $Root
docker compose -f docker-compose.test.yml up --build --abort-on-container-exit --exit-code-from tests
$exitCode = $LASTEXITCODE
docker compose -f docker-compose.test.yml down -v
Pop-Location

if ($exitCode -ne 0) { exit $exitCode }
Write-Host "`nAll tests passed." -ForegroundColor Green
