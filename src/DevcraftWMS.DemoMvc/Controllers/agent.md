# agent.md - DemoMvc Controllers

## Purpose
- MVC controllers for routing, binding, and orchestration of UI flows.

## Allowed dependencies
- ViewModels, ApiClients, Infrastructure helpers.

## Mandatory patterns
- Thin controllers: call ApiClients only.
- Use PRG (redirect after POST).
- Set TempData for success/error feedback.
- Client telemetry views must query logs with `source=client` and remain separate from backend logs.
- On errors, return the shared error view and keep correlation id visible.

## Forbidden actions
- Business logic in controllers.
- Direct HttpClient usage (must use ApiClients).

## Acceptance checklist
- Input validation handled with ModelState.
- Redirect after successful POST.
- EN-US naming.
- Frontend logs are routed under ClientLogs controller/actions.
