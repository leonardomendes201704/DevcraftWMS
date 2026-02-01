# agent.md - PerfectApiTemplate.Infrastructure

## Purpose
- Persistence (EF Core) and external integrations (clients, messaging, etc.).
 - Outbox processing lives in Infrastructure.

## Allowed dependencies
- Application and Domain.
- EF Core and provider packages.
- Serilog / OpenTelemetry integrations (if present).

## Mandatory patterns
- EF `DbContext` lives in Infrastructure and is the source of EF mappings.
- Keep EF mappings in `Persistence/Configurations` (Fluent API).
- Keep migrations under `Persistence/Migrations` (or the chosen folder).
- Register infrastructure services via a single DI entry point (e.g., `DependencyInjection` extension).
- External clients must be behind Application abstractions (interfaces defined in Application).
- Outbox processor is a hosted service, and publishers implement `INotificationPublisher`.

## Forbidden actions
- Business rules in persistence/integration code.
- Direct references to Controllers or API project.
- Application logic duplicated inside repositories.
- Accessing HttpContext (belongs to API).

## Acceptance checklist
- DbContext mappings exist for all entities and are consistent.
- DI registration is centralized and called by API.
- Infrastructure implements Application abstractions (not the other way around).
- Migrations/dev provider configuration documented in README (if changed).

## Migrations rule (MANDATORY)
If any NEW FEATURE or FIX changes any entity, EF mapping, or DbContext in a way that impacts schema, you MUST:
- Add a migration (Infrastructure is the migrations project)
- Run database update locally (SQLite)

Use:
```bash
dotnet ef migrations add <MigrationName> --project src/PerfectApiTemplate.Infrastructure --startup-project src/PerfectApiTemplate.Api
dotnet ef database update --project src/PerfectApiTemplate.Infrastructure --startup-project src/PerfectApiTemplate.Api

