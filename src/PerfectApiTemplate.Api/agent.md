# agent.md - PerfectApiTemplate.Api

## Purpose
- HTTP API layer: routing, authentication/authorization, serialization, and wiring requests to Application via MediatR.

## Allowed dependencies
- Application.
- Infrastructure ONLY through composition root (DI registration / extension methods), never for direct data access.
- ASP.NET Core, Swagger/OpenAPI, Serilog, ProblemDetails.

## Mandatory patterns
- Controllers call ONLY MediatR (no business logic).
- Map `RequestResult<T>` to HTTP responses via `ResultExtensions` (single mapping strategy).
- Ensure error responses include `correlationId`.
- Endpoints must be documented (summary + response types) and grouped by feature/tag.
- Outbox usage is not allowed in API; side-effects go through Application services.

## Forbidden actions
- Business logic in controllers or filters.
- Direct EF Core / DbContext / repositories usage in API.
- Returning raw exceptions or stack traces to clients.
- Ad-hoc response shapes (must use the standard Result mapping).

## Acceptance checklist
- New endpoints use MediatR and return `RequestResult<T>`.
- Routes are under `/api/...`.
- Auth rules applied (`[Authorize]` where required) and Swagger reflects it.
- Error responses include `correlationId` (and follow ProblemDetails).
- Swagger updated (summaries, response codes, schemas).

