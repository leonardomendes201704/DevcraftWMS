# TASK-0104 - Mobile: Outbound Check Queue

## Summary
Show the outbound checking queue for verification tasks.

## Context
After picking, outbound check tasks ensure accuracy before packing. Operators need a queue to select tasks.

## Objective
- List check tasks with status and priority.
- Start a check task.

## Scope
- Queue list and filters.
- Start action.

## Dependencies
- Outbound check list API.

## UI/UX Requirements
- Default filter: Pending only.
- Show linked order and picking task.

## Data Fields (List)
- CheckTaskCode
- OutboundOrderCode
- Status
- Priority
- ItemsCount

## API Integration
- GET /api/outbound-checks
- POST /api/outbound-checks/{id}/start

## Acceptance Criteria
- User can list and start check tasks.

## How to Test
- UI: queue list + start.
- Swagger: list/start endpoints.

## Status
PENDING
