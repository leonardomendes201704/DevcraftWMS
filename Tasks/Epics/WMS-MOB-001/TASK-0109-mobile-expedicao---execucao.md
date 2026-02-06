# TASK-0109 - Mobile: Shipping Execution

## Summary
Confirm packages, load carrier, and close shipment.

## Context
Shipping execution validates packages and finalizes shipment status.

## Objective
- Scan packages to load.
- Confirm carrier and dock.
- Complete shipment.

## Dependencies
- Shipment details API.
- Shipment confirm API.

## API Integration
- GET /api/shipments/{id}
- POST /api/shipments/{id}/confirm

## Acceptance Criteria
- Shipment is completed after all packages loaded.

## How to Test
- UI: scan packages and complete shipment.
- Swagger: confirm endpoint.

## Status
PENDING
