# Application Flow

## User registration & login

```mermaid
sequenceDiagram
    actor User
    participant FE as Frontend
    participant API as AuthController
    participant DB as PostgreSQL
    participant BC as BCrypt

    User->>FE: Fill register form
    FE->>API: POST /api/Auth/register
    API->>DB: Check email unique
    API->>BC: Hash password
    API->>DB: Insert User
    API-->>FE: 200 OK
    FE->>User: Redirect to login

    User->>FE: Fill login form
    FE->>API: POST /api/Auth/login
    API->>DB: Find user by email
    API->>BC: Verify password
    alt Invalid credentials
        API-->>FE: 401 Invalid email or password
    else Valid
        API->>API: Generate JWT (7 days)
        API-->>FE: token + user summary
        FE->>FE: Store token in localStorage
        FE->>User: Navigate to dashboard
    end
```

## AI lesson generation

```mermaid
sequenceDiagram
    actor User
    participant FE as Dashboard
    participant API as PromptsController
    participant DB as PostgreSQL
    participant AI as AiService
    participant OAI as OpenAI

    User->>FE: Select topic + enter request
    FE->>API: POST /api/Prompts (Bearer JWT)
    API->>API: Validate JWT
    API->>DB: Load category context
    API->>AI: Build prompt
    AI->>OAI: Chat completion
    OAI-->>AI: Generated lesson
    AI-->>API: Response text
    API->>DB: Save PromptHistory
    API-->>FE: Lesson content
    FE->>User: Display lesson
```

## Startup & database seeding

```mermaid
flowchart TD
    START[Backend starts] --> MIG[EF Core MigrateAsync]
    MIG --> SEED[DbSeeder.SeedAsync]
    SEED --> ADMIN{Admin exists?}
    ADMIN -->|No| CREATE[Create admin@admin.com]
    ADMIN -->|Yes| CAT
    CREATE --> CAT[Seed 40+ categories]
    CAT --> READY[API ready /health]
```

- Migrations run in background on startup (Docker Compose)
- Categories/sub-categories loaded from `CategoriesSeedData.cs`
- Health endpoint available before seed completes

## Frontend routing

```mermaid
flowchart TD
    ROOT[/] --> LOGIN[/login]
    ROOT --> REG[/register]
    LOGIN -->|success user| DASH[/dashboard]
    LOGIN -->|success admin| ADMIN[/admin]
    DASH --> HIST[/history]
    DASH --> PR2[Generate lesson]
    ADMIN --> USR[Users tab]
    ADMIN --> PRM[Prompts tab]
    ADMIN --> CAT2[Categories tab]
```

Protected routes use `ProtectedRoute` component — redirects to `/login` if no valid JWT.
