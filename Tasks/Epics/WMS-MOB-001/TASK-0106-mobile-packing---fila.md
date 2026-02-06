# TASK-0106 - Mobile: Packing Queue

## Summary
Queue for packing tasks generated after check completion.

## Context
Packing tasks group items into packages and print labels.

## Objective
- List packing tasks with status.
- Start packing.

## Dependencies
- Packing task list API.

## API Integration
- GET /api/packing-tasks
- POST /api/packing-tasks/{id}/start

## Acceptance Criteria
- User can list and start packing tasks.

## How to Test
- UI: list/start.
- Swagger: list/start endpoints.

## Status
PENDING
