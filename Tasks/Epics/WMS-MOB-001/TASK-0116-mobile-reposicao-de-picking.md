# TASK-0116 - Mobile: Picking Replenishment

## Summary
Request or execute replenishment to picking locations.

## Context
Low picking balances require replenishment from bulk storage.

## Objective
- List replenishment tasks.
- Execute replenishment move.

## API Integration
- GET /api/replenishments
- POST /api/replenishments/{id}/confirm

## Acceptance Criteria
- Replenishment tasks completed with balance updates.

## How to Test
- UI: list and complete replenishment.
- Swagger: list/confirm endpoints.

## Status
PENDING
