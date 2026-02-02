# agent.md - DevcraftWMS.DemoMvc

## Purpose
- Demo MVC UI layer for showcasing API features and reusable UI patterns.

## Allowed dependencies
- ASP.NET Core MVC, Bootstrap 5, Bootstrap Icons.
- DemoMvc ApiClients (HTTP only), ViewModels, Infrastructure helpers.

## Mandatory patterns
- Server-rendered MVC with PRG (Post-Redirect-Get).
- No SPA frameworks; minimal JS (only Bootstrap behaviors).
- UI must use shared partials and GridBuilder for list screens.
- UI components must be documented/visible in the UI Showcase.
- All form fields must use Bootstrap input groups with left icons.
- Address blocks must use the reusable Address component (CEP lookup + IBGE UF/city).
- External endpoints must be configured via appsettings-bound Options with startup validation; no hardcoded URLs in DemoMvc.
- Enums must include DisplayName for every member.
- All user-facing error messages must be friendly, clear, and actionable (no raw exceptions).

## Forbidden actions
- Direct references to Api/Application/Infrastructure projects.
- Business logic in views or controllers.
- Storing JWT in browser storage (use server-side session only).

## Acceptance checklist
- Uses shared partials (grid, filters, alerts, etc.).
- Forms use validation and PRG.
- UI Showcase updated when components change.
- EN-US for all UI labels and code identifiers.

