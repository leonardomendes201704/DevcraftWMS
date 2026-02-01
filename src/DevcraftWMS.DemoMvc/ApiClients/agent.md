# agent.md - DemoMvc ApiClients

## Purpose
- Typed HTTP clients for the API (no direct project references).

## Allowed dependencies
- HttpClient, DemoMvc Infrastructure (session + base URL), DTOs.

## Mandatory patterns
- Always send JWT from server-side session.
- Handle RequestResult and ProblemDetails gracefully.
- No business logic; map HTTP to UI-friendly results.

## Forbidden actions
- Accessing EF Core or API internals.
- Hard-coded URLs (use ApiUrlProvider).

## Acceptance checklist
- Uses ApiClientBase.
- Errors mapped to UI messages.
- EN-US naming.
