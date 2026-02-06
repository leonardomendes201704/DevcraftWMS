# TASK-0108 - Mobile: Shipping Queue

## Summary
Queue for shipment tasks ready for dispatch.

## Context
Shipping tasks are created after packing. Operators prepare outbound shipments.

## Objective
- List shipments to dispatch.
- Start shipment task.

## API Integration
- GET /api/shipments
- POST /api/shipments/{id}/start

## Acceptance Criteria
- User can list and start shipments.

## How to Test
- UI: list/start shipments.
- Swagger: list/start endpoints.

## Status
PENDING
