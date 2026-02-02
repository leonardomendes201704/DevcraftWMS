# agent.md - Controllers (Portaria)

## Purpose
- Thin MVC controllers for Portaria views and flow orchestration.

## Allowed dependencies
- Portaria ApiClients, ViewModels, Infrastructure (session, options).

## Mandatory patterns
- Controllers must stay thin (no business logic).
- Use PRG after POST.
- Friendly validation messages only.

## Forbidden actions
- Direct API URL usage (use ApiClients).
- Accessing database or domain logic.

## Acceptance checklist
- Uses ApiClients only.
- PRG used for mutations.
- Clear success/error feedback via TempData.

