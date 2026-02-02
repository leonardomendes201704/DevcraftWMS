# agent.md - DevcraftWMS.Portaria

## Purpose
- Customer Portaria UI for inbound (ASN/OE) interactions and reporting.

## Allowed dependencies
- ASP.NET Core MVC, Bootstrap 5, Bootstrap Icons.
- Portaria ApiClients (HTTP only), ViewModels, Infrastructure helpers.

## Mandatory patterns
- Server-rendered MVC with PRG (Post-Redirect-Get).
- No SPA frameworks; minimal JS (only Bootstrap behaviors).
- UI must use shared partials and input groups with left icons.
- Every Index screen must include a Help button with HTML manual (Index + CRUD flows).
- External endpoints must be configured via appsettings-bound Options with startup validation.
- Enums must include DisplayName for every member.
- All user-facing error messages must be friendly and actionable.

## Forbidden actions
- Direct references to Api/Application/Infrastructure projects.
- Business logic in views or controllers.
- Storing JWT in browser storage (use server-side session only).

## Acceptance checklist
- Uses shared partials (alerts, breadcrumbs, sidebar/topbar).
- Forms use validation and PRG.
- Help manual added/updated for the screen (Index + related CRUD screens).
- EN-US for all UI labels and code identifiers.

