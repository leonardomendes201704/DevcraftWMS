# TASK-0105 - Mobile: Outbound Check Execution

## Summary
Execute outbound check by scanning items and confirming quantities.

## Context
Outbound checks validate picked items against order. This reduces packing errors.

## Objective
- Scan items and validate quantities.
- Record discrepancies with evidence.
- Complete the check task.

## Scope
- Execution screen for check task.
- Evidence capture (photo/notes).

## Dependencies
- Outbound check details API.
- Outbound check confirm API.

## UI/UX Requirements
- Show expected items list and progress.
- Allow scanning SKU/lot.
- Capture discrepancy reason and photo.

## Data Fields
- ProductSku (scan)
- LotCode (optional)
- QuantityChecked
- DivergenceReason
- EvidencePhoto

## Validation & Rules
- Require reason if quantity differs.
- Attach evidence when divergence is flagged.

## API Integration
- GET /api/outbound-checks/{id}
- POST /api/outbound-checks/{id}/confirm

## Acceptance Criteria
- Operator can complete check with or without divergences.

## How to Test
- UI: complete check with exact match and with divergence.
- Swagger: confirm endpoint with both scenarios.

## Status
PENDING
