# TASK-0110 - Mobile: Receiving Queue / Check-in

## Summary
Queue for inbound check-in and receipt start.

## Context
Inbound check-in starts the receiving workflow.

## Objective
- List inbound appointments/check-ins.
- Start receiving session.

## API Integration
- GET /api/gate-checkins
- POST /api/receipts/start

## Acceptance Criteria
- User can start a receipt session from a check-in.

## How to Test
- UI: list and start a receipt.
- Swagger: start receipt endpoint.

## Status
PENDING
