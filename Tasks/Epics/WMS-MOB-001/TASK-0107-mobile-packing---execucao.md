# TASK-0107 - Mobile: Packing Execution

## Summary
Pack items into packages, confirm contents, and print labels.

## Context
Packing execution finalizes packages and prepares for shipping.

## Objective
- Scan items into packages.
- Choose package type and weight.
- Confirm and complete packing.

## Dependencies
- Packing task details API.
- Packing confirm API.

## API Integration
- GET /api/packing-tasks/{id}
- POST /api/packing-tasks/{id}/confirm

## Validation & Rules
- Require package type and weight.
- Validate item quantities vs expected.

## Acceptance Criteria
- Packing completed with labels generated.

## How to Test
- UI: pack with valid and invalid quantities.
- Swagger: confirm endpoint.

## Status
PENDING
