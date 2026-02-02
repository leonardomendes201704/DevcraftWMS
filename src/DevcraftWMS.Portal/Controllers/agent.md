# agent.md - Controllers (Portal)

## Purpose
- Thin MVC controllers for Portal views and flow orchestration.

## Allowed dependencies
- Portal ApiClients, ViewModels, Infrastructure (session, options).

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
