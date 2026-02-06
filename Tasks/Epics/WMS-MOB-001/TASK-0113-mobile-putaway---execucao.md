# TASK-0113 - Mobile: Putaway Execution

## Summary
Execute putaway by scanning source and destination locations.

## Context
Putaway confirms moves from staging to storage locations.

## Objective
- Scan source and target locations.
- Confirm quantities moved.
- Complete the putaway task.

## API Integration
- GET /api/putaway-tasks/{id}
- POST /api/putaway-tasks/{id}/confirm

## Acceptance Criteria
- Task completes and balances updated.

## How to Test
- UI: execute putaway with valid scans.
- Swagger: confirm endpoint.

## Status
PENDING
