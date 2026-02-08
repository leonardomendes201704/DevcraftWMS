# TASK-0129 - DemoMvc: Pagination Defaults Guard

## Summary
Prevent API pagination requests with `PageNumber`/`PageSize` set to 0 by applying defaults across all API calls.

## Context
Several DemoMvc screens trigger API validation errors when page parameters are missing or zero, causing API responses like:
`PageSize must be between 1 and 200. PageNumber must be greater than 0.`

## Objective
- Normalize pagination query parameters before sending API requests.
- Ensure minimum valid defaults across all paginated calls.

## Scope
- DemoMvc API client URL builder normalization.
- No API or database changes.

## Out of Scope
- Changing API validation rules.
- UI form changes per screen.

## Acceptance Criteria
- Any request with `pageNumber=0` or `pageSize=0` is automatically normalized to `pageNumber=1` and `pageSize=20`.
- No API validation errors from pagination defaults on DemoMvc screens.

## How to Test
- UI: Open a screen with pagination and verify no validation errors in API calls.
- Swagger/Network: Inspect requests and confirm `pageNumber` and `pageSize` are never `0`.

## Status
DONE

## Implementation Notes
- Added pagination normalization in ApiClientBase.BuildUrl for pageNumber/pageSize defaults.
- Added Normalize() in Returns and DockSchedules query models to avoid pageNumber/pageSize = 0.
