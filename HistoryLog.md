# History Log

## 2026-02-01
- Initialized DevcraftWMS repository from PerfectApiTemplate.
- Renamed solution/projects/namespaces to DevcraftWMS.
- Removed SignalR auto-refresh toast banners in log grids.
- Handled API call timeouts/cancellations in DemoMvc ApiClientBase to avoid dashboard exceptions.
- Added Warehouse (Armazem) feature E2E with 1:N related entities, CRUD API, DemoMvc screens, KPI, tests, and migration AddWarehouses.
- Fixed LogsDbContext configuration scope and added migration to remove non-log tables from LogsDb.
- Refactored DemoMvc API clients to build query strings via QueryHelpers with filtered parameters.
- Added reusable address form component with CEP lookup and IBGE state/city loading, applied input-group icons across forms, and updated UI guidelines/showcase.
- Enforced no-hardcoded-URLs policy with validated external service options and refactored address lookup to use typed clients.
- TASK-0001 - Standards for task documentation and enum DisplayName metadata.
  - Key changes: added Tasks/ with TASK-0001 template, updated AGENTS rules, annotated all enums, added DisplayName helper.
- TASK-0002 to TASK-0006 - Planned E2E warehouse structure CRUDes (sectors, sections, structures, locations, aisles).
  - Key changes: created future task files under Tasks/ for E2E API + DemoMvc implementations.
- TASK-0002 - E2E sector management (Sectors) completed.
  - Key changes: added sector entity + mappings, API CRUD endpoints, DemoMvc screens, tests, and migration AddSectors.
- TASK-0003 - E2E section management (Sections) completed.
  - Key changes: added section entity + mappings, API CRUD endpoints, DemoMvc screens, tests, and migration AddSections.
