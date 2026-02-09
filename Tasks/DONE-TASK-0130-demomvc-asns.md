# TASK-0130 - DemoMvc: ASNs (List / Create / Edit / Details)

## Summary
Add ASN (Advance Shipping Notice) screens to DemoMvc with full CRUD-like flow (List, Create, Edit, Details) aligned with existing Portal behavior and API endpoints.

## Context
Today, ASN is only available in the customer Portal. DemoMvc (Backoffice) needs the same capability to register, review, and manage ASNs directly in the admin UI.

## Objective
- Provide DemoMvc screens for ASNs: List, Create, Edit, Details.
- Keep flow consistent with existing API and Portal UX patterns.
- Ensure permissions and customer context are enforced.

## Scope
- DemoMvc UI screens for:
  - List (/Asns)
  - Create (/Asns/Create)
  - Edit (/Asns/Edit/{id})
  - Details (/Asns/Details/{id})
- DemoMvc API client for ASN endpoints.
- ViewModels for ASN list/detail/form.
- Filters, pagination, and ordering aligned with API.
- Status actions (Submit/Approve/Convert/Cancel) surfaced in Details if supported by API.

## Out of Scope
- Changes to API behavior or domain rules.
- New ASN workflow rules not already in API.

## Dependencies
- Existing API endpoints for ASN (list, create, update, details, status actions).
- Customer context header `X-Customer-Id` set by DemoMvc.

## UI/UX Requirements
- Follow DemoMvc layout, grid patterns, and Help/Filters/New alignment.
- Use Bootstrap input groups with left icons.
- PageSize default 100 for lookups (AGENTS.md).
- Provide Help modal content for ASN screens.

## Data Fields (Create/Edit)
- WarehouseId
- AsnNumber
- SupplierName (if supported by API)
- DocumentNumber (if supported by API)
- ExpectedArrivalDate (if supported by API)
- Notes
- Items (SKU, Quantity, Lot/Expiry if applicable) if API supports inline items.

## API Integration (expected)
- GET /api/asns (paged, filters)
- GET /api/asns/{id}
- POST /api/asns
- PUT /api/asns/{id}
- POST /api/asns/{id}/submit
- POST /api/asns/{id}/approve
- POST /api/asns/{id}/convert
- POST /api/asns/{id}/cancel
- GET /api/asns/{id}/items
- POST /api/asns/{id}/items
- GET /api/asns/{id}/attachments
- POST /api/asns/{id}/attachments
- GET /api/asns/{id}/status-events

## Rules
- Respect existing status transitions defined by API.
- Do not allow Edit when ASN is in a locked status (if API enforces).
- Use RequestResult error mapping to show validation messages.

## Deliverables
- DemoMvc AsnsController + ApiClient + ViewModels.
- Views: Index, Create, Edit, Details.
- Help modal content for ASN.
- Menu item enabled in sidebar.
- HistoryLog entry.
- ASN update endpoint (API + Application).
- Unit/Integration test updates.

## Acceptance Criteria
- User can list ASNs with filters, ordering, and pagination.
- User can create a new ASN and see it in the list.
- User can edit a draft ASN (if allowed) and see changes in Details.
- User can view ASN details, items, attachments, and status timeline.
- Status actions (submit/approve/convert/cancel) are available when valid.
- Validation errors are shown inline.

## How to Test
- Unit tests: `dotnet test`
- Integration tests: `dotnet test`
- UI:
  1) Go to `/Asns`, verify list loads.
  2) Create a new ASN via `/Asns/Create`.
  3) Open Details and submit/approve/convert if available.
  4) Convert ASN and confirm the inbound order appears in `/InboundOrders`.
  5) Edit ASN (if allowed) and confirm updates.
- Swagger/API:
  - `GET /api/asns`
  - `POST /api/asns`
  - `GET /api/asns/{id}`
  - `PUT /api/asns/{id}`

## Status
DONE
