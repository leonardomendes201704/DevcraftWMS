# agent.md - DemoMvc Views

## Purpose
- Razor views and partials for UI rendering.

## Allowed dependencies
- ViewModels and shared partials.

## Mandatory patterns
- Use shared partials (GridBuilder, alerts, empty state, etc.).
- Keep views presentational; no business logic.
- Update UI Showcase whenever a reusable component changes.
- Use the frontend logs views (ClientLogs) for client telemetry only; backend logs stay under Logs.
- When showing errors, include correlation id if available (from ErrorViewModel).

## Forbidden actions
- Direct API calls or heavy logic in views.

## Acceptance checklist
- EN-US labels.
- Consistent layout and Bootstrap usage.
- Frontend telemetry views are separated from backend logs.
