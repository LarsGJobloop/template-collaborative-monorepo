# Example Web API with Database

A .NET 10 web API service with PostgreSQL database integration, demonstrating database initialization and migration management.

## Database Management

This service follows Zitadel's three-phase initialization pattern, adapted for simplicity:

### Phase 1: Init

The `example-web-api-with-database-init` service is a C# CLI tool that creates:

- Database: `commentary`
- Service user: `commentary` (with password `commentary`)
- Required privileges

This phase runs once using admin credentials (`postgres`/`postgres`) and completes before the main service starts. The init tool is built from the same codebase and packaged in the same container image as the API.

### Phase 2: Setup

**Skipped** - We use EF Core migrations instead of a separate setup phase.

### Phase 3: Start

The main service (`example-web-api-with-database`) automatically applies EF Core migrations on startup using the service user credentials.

## Configuration

### Environment Variables

The init service accepts:

- `POSTGRES_HOST` - Database host (default: `database`)
- `POSTGRES_PORT` - Database port (default: `5432`)
- `POSTGRES_ADMIN_USER` - Admin username (default: `postgres`)
- `POSTGRES_ADMIN_PASSWORD` - Admin password (default: `postgres`)
- `COMMENTARY_DATABASE` - Database name (default: `commentary`)
- `COMMENTARY_USER` - Service user (default: `commentary`)
- `COMMENTARY_PASSWORD` - Service password (default: `commentary`)

The main service uses:

- `POSTGRES_HOST` - Database host (default: `database`)
- `POSTGRES_PORT` - Database port (default: `5432`)
- `POSTGRES_DATABASE` - Database name (required)
- `POSTGRES_USER` - Database user (required)
- `POSTGRES_PASSWORD` - Database password (required)

## Migrations

EF Core migrations are automatically applied on application startup. To create a new migration:

```sh
cd src/example-web-api-with-database/Api
dotnet ef migrations add MigrationName
```

## Local Development

Start the platform services first:

```sh
docker compose -f compose.platform.yaml up -d
```

Then start the application services:

```sh
docker compose up -d
```

The init service will run automatically, creating the database and user before the main service starts.
