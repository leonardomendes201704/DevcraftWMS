# TASK-0120 - Mobile: Quick Master Data (Product/Lot)

## Summary
Allow quick creation of product or lot when missing during execution.

## Context
Receiving or picking may require quick registration of lot/product.

## Objective
- Create product and/or lot with minimal fields.
- Validate required tracking settings.

## API Integration
- POST /api/products
- POST /api/lots

## Acceptance Criteria
- User can create product/lot and resume flow.

## How to Test
- UI: create product/lot.
- Swagger: create endpoints.

## Status
PENDING
