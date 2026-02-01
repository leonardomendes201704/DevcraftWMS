# agent.md - PerfectApiTemplate.Application

## Purpose
- Application layer: CQRS (Commands/Queries), handlers, validators, and business orchestration via Services.
- Enqueue outbox messages and define notification contracts.

## Allowed dependencies
- Domain.
- MediatR, FluentValidation (and Application-internal abstractions).

## Mandatory patterns
- Commands mutate state; Queries are read-only and must not cause side-effects.
- Handlers return `RequestResult<T>` and remain orchestration-focused.
- Business rules must live in Application Services (handlers call services).
- Validation:
  - Use FluentValidation for inputs.
  - Domain invariants belong in Domain.
- Use async APIs for any I/O (through abstractions, never direct EF).
- Any outbound side-effects must be enqueued via `IOutboxEnqueuer`.

## Forbidden actions
- Any Infrastructure dependencies (EF Core, HttpClient implementations, file system, etc.).
- Direct DbContext usage.
- Static service locator usage.
- �Fat handlers� containing complex business logic.

## Acceptance checklist
- Feature folders follow `/Features/<FeatureName>/Commands|Queries/<ActionName>/...`.
- Each command/query has: request + handler (+ validator when applicable).
- Result pattern used consistently (no exceptions for flow control).
- Any new I/O requirement is expressed as an abstraction (interface) in Application.

