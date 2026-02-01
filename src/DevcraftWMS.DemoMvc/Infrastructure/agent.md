# agent.md - DemoMvc Infrastructure

## Purpose
- UI infrastructure helpers (session, options, base URL).

## Allowed dependencies
- ASP.NET Core abstractions only.

## Mandatory patterns
- Keep helpers small and reusable.
- No business logic.
- Correlation/telemetry middleware must be registered early and never log secrets.
- Telemetry sending must be best-effort and non-blocking.

## Forbidden actions
- API calls or data access.

## Acceptance checklist
- Helpers are unit-testable.
- EN-US naming.
- Telemetry headers and correlation id are propagated in ApiClients.
