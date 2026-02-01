# agent.md - Tests

## Purpose
- Unit and integration tests for the solution.

## Allowed dependencies
- Test frameworks and the solution projects as needed.

## Mandatory patterns
- Unit tests avoid infrastructure.
- Integration tests use WebApplicationFactory.

## Forbidden actions
- Long-running or external dependencies.
- Mutating production data.

## Acceptance checklist
- Tests compile and run in CI.
- Integration tests hit HTTP endpoints.

