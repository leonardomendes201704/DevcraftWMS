# agent.md - ApiClients (Portaria)

## Purpose
- Typed HTTP clients for Portaria to call the API.

## Allowed dependencies
- HttpClientFactory, ApiUrlProvider, ViewModels/DTOs.

## Mandatory patterns
- No hardcoded URLs; base URL from Options.
- Query strings built with QueryHelpers.
- Include Authorization and X-Customer-Id headers when available.

## Forbidden actions
- Direct database access.
- Inline HttpClient creation (use DI).

## Acceptance checklist
- All calls use ApiClientBase.
- Error handling returns friendly messages.

