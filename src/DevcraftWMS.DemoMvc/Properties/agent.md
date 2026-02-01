# agent.md - DemoMvc Properties

## Purpose
- Project launch settings and environment-specific configuration.

## Allowed dependencies
- ASP.NET Core project settings.

## Mandatory patterns
- Keep profiles aligned with README instructions.
- Ensure DemoMvc telemetry settings are present in appsettings and do not store secrets in source control.

## Forbidden actions
- Hard-coded secrets.

## Acceptance checklist
- Ports documented in README.
- EN-US comments (if any).
- Telemetry keys are configured via secrets or environment variables in production.
