# agent.md - PerfectApiTemplate.Domain

## Purpose
- Core domain entities and rules.

## Allowed dependencies
- None (no external dependencies).

## Mandatory patterns
- Entities are persistence-agnostic.
- Keep invariants inside domain where possible.
- Domain events (if introduced) must be serializable for outbox usage.

## Forbidden actions
- References to Application/Infrastructure.
- Framework-specific annotations.

## Acceptance checklist
- Entities remain POCOs.
- No external dependencies introduced.

