# comissions.app — Core API

The backend REST API for **comissions.app**, a platform where artists offer
commission services and customers request, pay for, and review them
(Skeb-style). This service owns users, artist profiles, commission requests,
payments, and the public discovery surface.

> Part of a multi-repo project: this Core API, a separate web UI
> (`comissions-app-ui`), and deployment manifests (`comissions-app-argocd`).

## Tech stack

- **.NET 8** / ASP.NET Core Web API
- **PostgreSQL** via **Entity Framework Core 8** (Npgsql)
- **Auth0** for authentication (JWT bearer) and scope-based authorization
- **Stripe Connect** for marketplace payments and artist payouts
- **Novu** for transactional notifications
- **Swagger / OpenAPI** for API docs and SDK generation
- Local file storage by default; an imgcdn provider is available behind the
  `IStorageService` abstraction

## Repository layout

```
src/
├── comissions.app.api/              # the Web API (controllers, entities, services, EF migrations)
├── comissions.app.database/         # shared database project
├── comissions.app.database.migrator/# standalone migration runner (for CI/CD / init jobs)
└── comissions.app.sdk/              # generated C# client SDK (from the OpenAPI spec)
```

### API surface (high level)

| Area | Routes | Notes |
| --- | --- | --- |
| Discovery | `GET /api/Discovery/Artists…` | Public: browse artists, portfolios, reviews |
| User | `/api/User` | Read/update the authenticated user |
| Artist | `/api/Artist…` | Profile, page settings, portfolio, payments, reviews |
| Artist access | `/api/ArtistAccessRequest` | Request to become a seller |
| Requests | `/api/Requests…` | Customer and artist sides of a commission |
| Payments | `/api/Requests/PaymentWebhook` | Stripe webhook (anonymous, signature-verified) |
| Admin | `/api/admin/…` | Admin-only: users, artists, seller requests |

## Authentication & authorization

Auth is handled by **Auth0**. The API validates JWT bearer tokens (issuer =
`Auth0:Domain`, audience = `Auth0:Audience`) and enforces **scopes** per
endpoint via authorization policies:

```
read:user   write:user
read:artist write:artist
read:request write:request
```

Admin endpoints require an `Admin` role claim. On the first authenticated
request, the API provisions the user record automatically.

## Configuration

Settings are read from `appsettings.json` (and environment-specific
overrides), or from environment variables using the standard .NET
double-underscore convention (e.g. `Database__password`, `Auth0__Domain`).

**Never commit real secrets.** Copy the template and fill in your own values
locally:

```bash
cp src/comissions.app.api/appsettings.example.json src/comissions.app.api/appsettings.json
```

| Key | Required | Description |
| --- | --- | --- |
| `Database:Host` / `:Port` / `:Database` | – | Postgres connection (sensible local defaults) |
| `Database:username` / `:password` | ✅ | Postgres credentials — the app fails fast if unset |
| `Database:RunMigrationsOnStartup` | – | `true` to apply EF migrations on boot (default `false`) |
| `Auth0:Domain` / `:Audience` | ✅ | Token issuer and API audience |
| `Auth0:ClientId` / `:ClientSecret` | – | Used by the Swagger UI OAuth login |
| `Stripe:ApiKey` / `:WebHookSecret` | – | Required only for payment / payout / webhook flows |
| `Novu:ApiKey` | – | Required only for notifications |
| `Storage:ImgCdn:ApiKey` | – | Only if using the imgcdn storage provider |
| `Cors:AllowedOrigins` | – | Comma-separated allow-list (e.g. `http://localhost:3000`) |

## Getting started (local)

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- A PostgreSQL 15 database (Docker is easiest)
- An Auth0 tenant with a custom API + the scopes listed above

### 1. Start Postgres

```bash
docker run -d --name comissions-pg \
  -e POSTGRES_USER=postgres -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=comissionsapp -p 5432:5432 postgres:15
```

### 2. Configure

Copy `appsettings.example.json` to `appsettings.json` and fill in your Auth0
values, or export environment variables:

```bash
export Database__username=postgres
export Database__password=postgres
export Database__RunMigrationsOnStartup=true
export Auth0__Domain="https://YOUR_TENANT.us.auth0.com/"
export Auth0__Audience="https://api.comissions.app"
```

### 3. Run

```bash
dotnet run --project src/comissions.app.api/comissions.app.api.csproj
```

The API listens on `http://localhost:5290` and Swagger UI is at
`http://localhost:5290/swagger`. With `RunMigrationsOnStartup=true` the schema
is created automatically on first boot.

### Database migrations

Migrations live in `src/comissions.app.api/Migrations`. In production, prefer
running them out-of-band via the migrator project rather than on app startup:

```bash
dotnet run --project src/comissions.app.database.migrator
```

## Docker

```bash
docker build -t comissions-app-core-api -f src/comissions.app.api/Dockerfile .
docker run -p 8080:8080 \
  -e Database__Host=... -e Database__username=... -e Database__password=... \
  -e Auth0__Domain=... -e Auth0__Audience=... \
  comissions-app-core-api
```

Released images are published to GitHub Container Registry:
`ghcr.io/d4m13n-d3v/comissions-app-core-api`.

## CI/CD

GitHub Actions workflows under `.github/workflows`:

| Workflow | Purpose |
| --- | --- |
| `ci.yml` | Build the API + migrator, build the Docker image, Trivy filesystem scan |
| `codeql.yml` | CodeQL static analysis (C#) |
| `gitleaks.yml` | Secret scanning across full history |
| `release.yml` | GitVersion tag → Trivy-gated GHCR image publish → GitHub Release |
| `sdk-publish.yml` | Publish the C# and JavaScript SDKs to GitHub Packages |

Dependencies are kept current by **Dependabot** (NuGet, GitHub Actions,
Docker). Versioning follows [GitVersion](https://gitversion.net/) using
conventional-commit messages.

## Contributing

Use conventional-commit messages (`feat:`, `fix:`, `chore:`, …) — they drive
version bumps. Open a pull request against `main`; CI, CodeQL, and the secret
scan must pass.

## License

See [LICENSE.md](LICENSE.md).
