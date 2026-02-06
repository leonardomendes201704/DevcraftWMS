# TASK-0102 - Mobile: Picking Queue (Task List)

## Summary
Show the picking task queue with filters, priority, and start actions.

## Context
Picking tasks are created during outbound release. Operators need a mobile queue to view pending tasks and start work.

## Objective
- List picking tasks with key attributes.
- Filter by status, priority, zone, or method.
- Start a task from the queue.

## Scope
- Queue list screen.
- Task details preview drawer.
- Start/assign action.

## Dependencies
- PickingTask list API.
- PickingTask status updates.

## UI/UX Requirements
- Default filter: Pending only, IsActive=true.
- Sort by Priority then CreatedAt.
- Search by OutboundOrderCode or TaskCode.

## Data Fields (List)
- TaskCode
- OutboundOrderCode
- Status
- Priority
- PickingMethod
- ItemsCount
- CreatedAtUtc

## Validation & Rules
- Only allow start if status is Pending.
- If already in progress by another user, show blocked message.

## API Integration
- GET /api/picking-tasks (filters + pagination)
- POST /api/picking-tasks/{id}/start

## Deliverables
- Queue list screen with filters and pagination.
- Start button with confirmation.

## Acceptance Criteria
- Operator can see tasks, filter, and start a task.
- Status changes to InProgress after start.

## How to Test
- UI: open queue, apply filters, start a task.
- Swagger: call list/start endpoints.

## Status
PENDING
