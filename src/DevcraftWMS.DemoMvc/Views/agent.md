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
- Every Index view must include a Help button that renders a versioned HTML manual for the module.
- The Help manual must include instructions for the Index screen and its related CRUD screens (Create/Edit/Details/Delete) within the same context.
- All form fields must use Bootstrap input groups with left icons.
- Address blocks must use the reusable Address component (CEP lookup + IBGE UF/city).

## Forbidden actions
- Direct API calls or heavy logic in views.

## Acceptance checklist
- EN-US labels.
- Consistent layout and Bootstrap usage.
- Frontend telemetry views are separated from backend logs.
- Help manual updated for the screen (Index + related CRUD screens).
