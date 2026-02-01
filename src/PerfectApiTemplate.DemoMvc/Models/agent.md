# agent.md - DemoMvc Models

## Purpose
- MVC models used by views (avoid domain logic).

## Allowed dependencies
- None (simple POCOs only).

## Mandatory patterns
- Keep models focused on UI needs.
- Include fields needed for frontend telemetry screens (event type, severity, correlation ids).

## Forbidden actions
- Business logic or data access.

## Acceptance checklist
- Simple DTO-like structures.
- EN-US naming.
- Client log view models do not mix with backend log view models.
