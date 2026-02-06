# TASK-0101 - Mobile: Authentication and Warehouse Selection

## Summary
Provide login, session start, and warehouse selection for the mobile app.

## Context
Mobile operators must authenticate and select a customer/warehouse context before executing tasks. This mirrors the DemoMvc/Portal flows and should use existing auth endpoints.

## Objective
- Allow secure login with username/password.
- Load available customers/warehouses and set active context.
- Persist session securely (server session or secure storage).

## Scope
- Login screen.
- Customer/Warehouse selection screen.
- Session handling (token refresh, logout).

## Out of Scope
- RBAC management screens.
- MFA or SSO (unless already available).

## Dependencies
- Existing auth endpoints and policies.
- Customer/warehouse lookup endpoints.

## UI/UX Requirements
- Splash -> Login -> Select Context -> Home.
- Show active customer and warehouse on header.
- Session timeout message with re-login.

## Data Fields
- Username
- Password
- Customer
- Warehouse

## Validation & Rules
- Required fields on login.
- Prevent entering home without context.
- Store token securely (no localStorage).

## API Integration
- POST /api/auth/login
- GET /api/customers (filtered by user access)
- GET /api/warehouses (filtered by customer)

## Deliverables
- Mobile login screen.
- Context selection screen.
- Session persistence and logout.

## Acceptance Criteria
- User can login and select customer/warehouse.
- Invalid credentials show friendly error.
- Session expires with a clear prompt to re-authenticate.

## How to Test
- UI: login with valid and invalid credentials.
- UI: select customer/warehouse and verify header shows the context.
- API/Swagger: validate auth and lookup endpoints.

## Status
PENDING
