# TASK-0111 - Mobile: Receiving Execution (Counts)

## Summary
Execute receiving counts with scan and quantity confirmation.

## Context
Receiving validates ASN/OE items and captures actual quantities.

## Objective
- Scan items and record quantities.
- Capture lot/expiry if required.
- Close receipt when complete.

## Dependencies
- Receipt details API.
- Receipt count/confirm API.

## API Integration
- GET /api/receipts/{id}
- POST /api/receipts/{id}/counts
- POST /api/receipts/{id}/complete

## Acceptance Criteria
- Items counted and receipt completed successfully.

## How to Test
- UI: count items, complete receipt.
- Swagger: counts/complete endpoints.

## Status
PENDING
