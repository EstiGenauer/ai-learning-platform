#!/bin/sh
set -eu

API_URL="${API_URL:-http://backend:8080}"
MAX_ATTEMPTS=30

log() {
  echo "[compose-test] $1"
}

fail() {
  echo "[compose-test] FAIL: $1" >&2
  exit 1
}

wait_for() {
  url="$1"
  name="$2"
  attempt=1
  while [ "$attempt" -le "$MAX_ATTEMPTS" ]; do
    if curl -sf "$url" >/dev/null 2>&1; then
      log "$name is ready"
      return 0
    fi
    log "Waiting for $name ($attempt/$MAX_ATTEMPTS)..."
    attempt=$((attempt + 1))
    sleep 2
  done
  fail "$name did not become ready in time"
}

assert_status() {
  method="$1"
  url="$2"
  expected="$3"
  shift 3

  status=$(curl -s -o /tmp/response.json -w "%{http_code}" -X "$method" "$@" "$url")
  if [ "$status" != "$expected" ]; then
    echo "Response body:" >&2
    cat /tmp/response.json >&2 || true
    fail "$method $url expected HTTP $expected, got $status"
  fi
  log "OK $method $url -> $status"
}

log "Starting Docker Compose integration tests against $API_URL"

wait_for "$API_URL/health" "health endpoint"
wait_for "$API_URL/test-db" "database connection"

assert_status GET "$API_URL/api/Categories" 200

assert_status POST "$API_URL/api/Auth/login" 401 \
  -H "Content-Type: application/json" \
  -d '{"email":"admin@admin.com","password":"wrong-password"}'

curl -s "$API_URL/api/Categories" -o /tmp/categories.json

attempt=1
login_status=000
while [ "$attempt" -le "$MAX_ATTEMPTS" ]; do
  login_status=$(curl -s -o /tmp/login.json -w "%{http_code}" \
    -X POST "$API_URL/api/Auth/login" \
    -H "Content-Type: application/json" \
    -d '{"email":"admin@admin.com","password":"Admin123!"}')
  if [ "$login_status" = "200" ]; then
    break
  fi
  log "Waiting for admin seed + login ($attempt/$MAX_ATTEMPTS)..."
  attempt=$((attempt + 1))
  sleep 2
done

if [ "$login_status" != "200" ]; then
  cat /tmp/login.json >&2
  fail "Admin login expected HTTP 200, got $login_status"
fi
log "OK admin login -> 200"

TOKEN=$(sed -n 's/.*"token"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' /tmp/login.json | head -n 1)
if [ -z "$TOKEN" ]; then
  fail "Login response did not contain a JWT token"
fi

curl -s "$API_URL/api/Categories" -o /tmp/categories.json
CATEGORY_ID=$(tr ',' '\n' < /tmp/categories.json | grep '"id"' | head -n 1 | grep -o '[0-9]*')
SUB_ID=$(tr ',' '\n' < /tmp/categories.json | grep '"id"' | sed -n '2p' | grep -o '[0-9]*')

if [ -z "$CATEGORY_ID" ] || [ -z "$SUB_ID" ]; then
  cat /tmp/categories.json >&2
  fail "Could not parse category/sub-category ids from /api/Categories"
fi

prompt_status=$(curl -s -o /tmp/prompt.json -w "%{http_code}" \
  -X POST "$API_URL/api/Prompts" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer $TOKEN" \
  -d "{\"categoryId\":$CATEGORY_ID,\"subCategoryId\":$SUB_ID,\"promptText\":\"Explain containerization briefly\"}")

if [ "$prompt_status" != "200" ]; then
  cat /tmp/prompt.json >&2
  fail "Prompt generation expected HTTP 200, got $prompt_status"
fi

if ! grep -q "Test lesson" /tmp/prompt.json; then
  cat /tmp/prompt.json >&2
  fail "Prompt response did not contain fake AI lesson text"
fi

log "OK prompt generation with fake AI -> 200"
log "All Docker Compose integration tests passed"
