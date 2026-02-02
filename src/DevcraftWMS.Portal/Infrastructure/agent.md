# agent.md - Infrastructure (Portal)

## Purpose
- Portal infrastructure helpers (options, session, URL providers).

## Allowed dependencies
- ASP.NET Core abstractions, Options, configuration.

## Mandatory patterns
- External endpoints must come from Options and validated on startup.
- Keep helpers small and reusable.

## Forbidden actions
- Hardcoded external URLs.
- Business logic or data access.

## Acceptance checklist
- Options validated with ValidateOnStart.
- Helpers unit-testable.
