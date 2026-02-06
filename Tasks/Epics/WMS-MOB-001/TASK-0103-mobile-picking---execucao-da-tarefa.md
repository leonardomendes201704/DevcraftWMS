# TASK-0103 - Mobile: Picking Execution

## Summary
Execute a picking task by scanning locations/items and confirming quantities.

## Context
Picking tasks contain ordered items and required quantities. Operators must scan locations and product/lot to confirm pick.

## Objective
- Guide the operator through pick sequence.
- Validate lot/expiry and location.
- Confirm picked quantities and complete the task.

## Scope
- Task execution screen.
- Item sequence and scan validation.
- Task completion.

## Dependencies
- Picking task details API.
- Picking confirmation API.

## UI/UX Requirements
- Show current item with target location and quantity.
- Large scan input (barcode).
- Allow partial pick with reason.

## Data Fields
- LocationCode (scan)
- ProductSku (scan)
- LotCode / ExpirationDate (if tracked)
- QuantityPicked
- ShortPickReason (optional)

## Validation & Rules
- Enforce FEFO/lot selection when required.
- Prevent completion if any item is missing without reason.

## API Integration
- GET /api/picking-tasks/{id}
- POST /api/picking-tasks/{id}/confirm

## Deliverables
- Mobile execution UI with scan-first flow.
- Completion confirmation dialog.

## Acceptance Criteria
- Operator can pick all items and complete the task.
- API validates and updates inventory status.

## How to Test
- UI: execute a task, scan valid/invalid barcodes.
- Swagger: confirm endpoint with valid payload.

## Status
PENDING
