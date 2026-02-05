# History Log

## 2026-02-05
- TASK-0113 - Outbound shipping API completed.
  - Key changes: added outbound shipment entities/configs/migration, shipping endpoint with status updates, and unit/integration tests.

## 2026-02-05
- TASK-0112 - Outbound packing DemoMvc UI completed.
  - Key changes: added packing queue/details screens with label preview/print and updated tests.

## 2026-02-05
- TASK-0111 - Packing API completed.
  - Key changes: added outbound package entities/configs/migration, pack endpoint, and unit/integration tests.

## 2026-02-05
- TASK-0110 - Outbound check DemoMvc UI completed.
  - Key changes: added DemoMvc queue/details screens for outbound checks with evidence upload, plus test updates.

## 2026-02-05
- TASK-0109 - Outbound check API completed.
  - Key changes: added outbound check entities/configs/migration, register endpoint, and unit/integration tests.

## 2026-02-05
- TASK-0108 - Picking queue DemoMvc completed.
  - Key changes: added picking tasks API (list/get/confirm), DemoMvc UI (index/details), and unit/integration tests.

## 2026-02-05
- TASK-0107 - Picking task generation completed.
  - Key changes: added picking task repository and generation on outbound release with single/batch/cluster grouping and FEFO ordering, plus tests.

## 2026-02-05
- TASK-0106 - Picking tasks model completed.
  - Key changes: added PickingTask/PickingTaskItem entities, status enum, EF configs, and migration AddPickingTasks.

## 2026-02-05
- TASK-0105 - Reserva de estoque ao liberar OS.
  - Key changes: added stock availability validation and inventory reservation during outbound release, plus unit/integration tests.

## 2026-02-05
- Fix - Outbound order release form binding.
  - Key changes: bind Release form fields to correct model prefix to avoid missing Priority/PickingMethod.

## 2026-02-05
- TASK-0104 - Outbound order release workflow completed.
  - Key changes: added picking method/shipping window fields, release endpoint, DemoMvc release UI, and tests.

## 2026-02-05
- TASK-0103 - Portal outbound orders UI completed.
  - Key changes: added portal list/create/details for outbound orders, new API client, viewmodels, and menu entry.

## 2026-02-05
- TASK-0102 - Outbound order CQRS completed.
  - Key changes: added OutboundOrders CQRS (create/list/get), API controller, repository/service, and unit/integration tests.

## 2026-02-05
- TASK-0101 - Outbound order model completed.
  - Key changes: added OutboundOrder/OutboundOrderItem entities with status/priority enums, EF configs, DbSets, and migration AddOutboundOrders.

## 2026-02-05
- TASK-0100 - Epic Outbound index completed.
  - Key changes: created epic folder, README index, and references to outbound specification (txt/html).

## 2026-02-05
- Tasking - Epic WMS-OUT-001 created.
  - Key changes: created outbound epic README and tasks (0100-0120) based on Especificacao-Fluxo-Saida.

## 2026-02-05
- Doc - Manual WMS-INB-001 atualizado.
  - Key changes: reescreveu o manual HTML com escopos usuario/tecnico, fluxos, RBAC, anexos, KPIs e checklists.

## 2026-02-05
- TASK-0092/0093/0094 - ASN attachments real storage completed.
  - Key changes: added FileStorage abstraction + FileSystem provider, moved ASN attachments to external storage with metadata only, added download endpoint and Portal download/preview UI, updated tests, and migration UpdateAsnAttachmentsStorage.

## 2026-02-05
- TASK-0095 - Seed usuarios e perfis RBAC base.
  - Key changes: added RBAC user seeder with default passwords and role assignments for operational accounts.

## 2026-02-05
- TASK-0089/0090/0091 - RBAC roles/permissions/users completed.
  - Key changes: added RBAC domain model + migrations, seeded base roles/permissions, exposed API CRUD for roles/permissions/users, and added DemoMvc roles/users UI with tests.


## 2026-02-04
- TASK-0088 - Portaria role permissions completed.
  - Key changes: added role-aware Portaria user context, blocked critical actions with friendly messages, logged permission denials, and disabled UI actions when unauthorized.
- TASK-0087 - Portal receiving reports completed.
  - Key changes: added Portal "Receiving Reports" screen with period filters, view/export actions, date filtering in inbound orders list, and help manual.
- Fix - Receipts help updated for measurements.
  - Key changes: documented actual weight/volume capture and deviation blocking in Receipts help modal.
- TASK-0086 - Receipt measurements (weight/volume) completed.
  - Key changes: added expected/actual measurement fields, deviation calculation with optional blocking, DemoMvc capture + grid comparison, and migration AddReceiptItemMeasurements with tests.
- TASK-0085 - UL/SSCC relabel completed.
  - Key changes: added UnitLoadRelabelEvent history, relabel endpoint + validation, DemoMvc relabel form + history table, and migration AddUnitLoadRelabelEvents with tests.
- TASK-0083 - Alternative inbound flows completed.
  - Key changes: emergency inbound order creation from gate check-in with approval, pending lines in OE receipt report, and UI updates for emergency flow.
- TASK-0084 - Cross-dock exception flow completed.
  - Key changes: added cross-dock zone seed/locations, skipped putaway generation for cross-dock receipts, excluded cross-dock receipts from OE close validation, and added cross-dock section in receipt report UI.
- TASK-0082 - Inbound KPIs completed.
  - Key changes: added inbound KPI endpoint + repository, dashboard filter and stat cards, integration test for inbound KPIs, and updated README/Help.
- TASK-0081 - Inbound order notifications completed.
  - Key changes: added notification entity/repository, email/webhook/portal delivery with templates, Portal UI with resend, API endpoints, integration tests, and migration AddInboundOrderNotifications.
- TASK-0080 - OE receipt report completed.
  - Key changes: added receipt report service/queries + export endpoint, Portal/DemoMvc report views and CSV download, updated receipt repository includes, and integration tests for report/export.
- TASK-0079 - OE completion validations completed.
  - Key changes: added inbound order completion endpoint/command, status events, partial completion status, putaway/unit load validations, updated Portal/DemoMvc UI, and integration tests with migration AddInboundOrderStatusEvents.
- TASK-0078 - Putaway manual reassignment completed.
  - Key changes: added assignment events, reassign endpoint/command/validator, DemoMvc reassignment UI + history, and unit/integration tests with migration AddPutawayTaskAssignments.
- TASK-0077 - Putaway confirmation stabilization.
  - Key changes: avoid EF tracking conflicts in inventory movement response mapping, adjust tracked balance query includes, and disable integration test parallelization to prevent disposed provider failures.
- TASK-0077 - Putaway execution and confirmation completed.
  - Key changes: added confirm endpoint/command, moved balances via inventory movements, updated UL status, added DemoMvc confirmation UI, and unit/integration tests.
- TASK-0076 - Putaway suggestion engine completed.
  - Key changes: added suggestion query/endpoint, ranking and compatibility filters (zone/capacity/tracking), DemoMvc suggestions on task details, ensured putaway tasks persist on label print, and unit/integration tests.
- TASK-0075 - Putaway task model and generation completed.
  - Key changes: added PutawayTask entity/repository/queries, created task on UL label print, added API + DemoMvc list/details UI, migration AddPutawayTasks, and unit test coverage.
- Guideline - Require "How to test" steps in chat after completing tasks.
  - Key changes: added PR checklist rule to include concrete testing steps in the response and in the completed task file.
- TASK-0074 - Quarantine stock blocking completed.
  - Key changes: blocked inventory movements for quarantined lots or blocked balances, set available quantity to zero for blocked balances, added unit tests, and marked task as done.
- TASK-0071/0072/0073 - Quarantine and quality inspections completed.
  - Key changes: added quarantine enforcement on receipt items, quality inspection entities/CQRS/API, DemoMvc queue/detail UI with evidence download, quarantine seed zone/location, and unit/integration tests.
- TASK-0070 - Recebimento: captura lote/validade completed.
  - Key changes: added lot code/expiration capture on receipt items, auto-create lots when needed, updated UI/help, and unit test coverage.


## 2026-02-03
- Fix - Receipt counts form binding for Save Count.
  - Key changes: bind ReceiptCount form using NewCount prefix to avoid missing ReceiptId.
- TASK-0069 - Receipt divergences with evidence completed.
  - Key changes: added divergence entities, evidence upload, API endpoints, DemoMvc counts UI, options config, and integration tests.
- TASK-0068 - Recebimento conferencia cega/assistida completed.
  - Key changes: added ReceiptCount entity/mode, API endpoints for expected items and counts, DemoMvc counts screen with help, unit/integration tests, and migration AddReceiptCounts.
- TASK-0067 - Unit load SSCC model and label printing completed.

  - Key changes: added UnitLoad entity/status with internal SSCC generation, print label endpoint, DemoMvc UI with help modal, tests, and migration AddUnitLoads.

- TASK-0066 - Recebimento session linked to inbound order completed.

  - Key changes: added Receipt link to inbound order with start/finish endpoints, OE status updates on start/complete, DemoMvc start action + receipt display, unit/integration tests, and migration AddReceiptInboundOrderSession.

- TASK-0065 - Portaria dock assignment completed.

  - Key changes: added dock assignment endpoint, status/timestamp updates, Portaria assign dock screen, and migration AddGateCheckinDockAssignment.

- TASK-0064 - Portaria check-in UI completed.

  - Key changes: added Portaria gate check-in screens, queue list with triage actions, new API clients, and help manual.

- Fix - Sample data access seeding concurrency.

  - Key changes: insert sector/section/structure/aisle/location access via join tables to avoid tracking/concurrency issues.

- Fix - Ensure SQLite connection strings resolve to writable paths.

  - Key changes: normalize SQLite Data Source to content root and create directories on startup to avoid "unable to open database file".

- TASK-0063 - Portaria check-in API completed.

  - Key changes: added GateCheckin entity/status, CQRS/service/repository, API CRUD endpoints, migration AddGateCheckins, and unit/integration tests.

- Fix - Portal ASN approve action and UI button.

  - Key changes: added Approve API call + controller action and enabled approve button when status is Pending.

- Fix - Inbound Orders list view grid rendering.

  - Key changes: build GridViewModel before rendering in DemoMvc Index view.

- TASK-0058/0059/0060/0061/0062 - Inbound Orders (OE) completed.

  - Key changes: added InboundOrder model/status, ASN-to-OE conversion, parameters + cancel endpoints, backoffice queue UI, Portal details/actions, and migration AddInboundOrders with integration tests.

- TASK-0057 - ASN status update fix.

  - Key changes: updated ASN status persistence to avoid concurrency errors and aligned unit tests.



## 2026-02-02

- TASK-0057 - ASN status workflow completed.

  - Key changes: added status transitions, status history events, API endpoints, and Portal history UI.

- TASK-0056 - ASN tracking validations completed.

  - Key changes: added ASN items, tracking mode validations for lot/expiry, and Portal item entry UI.

- Tasking - Added ASN real storage tasks.

  - Key changes: created TASK-0092..0094 for storage abstraction, API download, and Portal preview.

- TASK-0055 - ASN attachments completed.

  - Key changes: added AsnAttachment entity/table, API upload/list endpoints, Portal upload/list UI, and tests.

- UX - Portal customer context error message.

  - Key changes: friendly message when X-Customer-Id is missing in Portal API calls.

- Fix - Seed zone customer access concurrency.

  - Key changes: insert ZoneCustomer directly to avoid tracking conflicts during seed.

- TASK-0054 - Portal ASN UI completed.

  - Key changes: added ASN list/create/details screens in Portal, API clients for ASN/warehouses, and help manual on index.

- TASK-0053 - ASN API CQRS completed.

  - Key changes: added ASN CQRS/service/repository, API endpoints, and unit/integration tests for create/list/get.

- TASK-0052 - ASN model and status completed.

  - Key changes: added Asn/AsnItem entities, AsnStatus enum, EF configs, DbSets, and migration AddAsnModel.

- Tasking - Added RBAC management tasks (API/UI).

  - Key changes: created TASK-0089/0090/0091 for roles, user management, and backoffice UI.

- TASK-0051 - RBAC base roles and policies completed.

  - Key changes: added UserRole, role claim in JWT, admin role seeding, role-based policies and controller guards, test auth default, and migration AddUserRole.

- TASK-0050 - Location capacity and compatibility completed.

  - Key changes: added max weight/volume and tracking flags to locations, enforced compatibility in receipts/movements, updated Location UI, tests, and migration AddLocationCapacityAndRestrictions.

- TASK-0049 - Zones completed.

  - Key changes: added Zone entity/type, customer access, and Location.ZoneId link; added Zones API/UI with filters; added zone seed data; added unit/integration tests; added migration AddZones.

- TASK-0048 - Minimum shelf-life per SKU completed.

  - Key changes: added MinimumShelfLifeDays to products, enforced lot requirement and quarantine on receipt, blocked balances for quarantined lots, updated UI and tests, and added migration AddProductMinimumShelfLifeDays.

- TASK-0047 - SKU Tracking Mode completed.

  - Key changes: added TrackingMode enum to products, API/UI support, lot validation rules, and migration AddProductTrackingMode.

- TASK-0046 - Portaria (projeto web) completed.

  - Key changes: created DevcraftWMS.Portaria MVC project with shared layout, login/session flow, customer context selector, and API client base.

- TASK-0045 - Portal do Cliente (projeto web) completed.

  - Key changes: created DevcraftWMS.Portal MVC project with shared layout, login/session flow, customer context selector, and API client base.

- Tasking - Created inbound epic tasks (WMS-INB-001).

  - Key changes: created epic folder with prioritized, small-scope tasks for ASN, OE, portaria, recebimento, qualidade/quarentena, putaway, encerramento, and new Portal/Portaria projects.

- Tasking - Marked TASK-0010 as done.

  - Key changes: renamed task file to DONE prefix and recorded completed subtasks.

- Fix - Seed movement lookup translation.

  - Key changes: replaced StartsWith with EF.Functions.Like for SQLite translation in seed detection.

- Fix - Inventory movements structure lookup.

  - Key changes: added customer-scoped structures list endpoint and DemoMvc client usage to populate movement form options reliably.

- TASK-0043 - Movimentacoes Internas (Seed + Observabilidade) completed.

  - Key changes: added optional seed for inventory movements/balances with configurable quantities and windows, plus README/env updates.

- TASK-0042 - Movimentacoes Internas (Testes) completed.

  - Key changes: added unit tests for movement service and integration tests for movement creation and insufficient balance scenarios.

- TASK-0041 - Movimentacoes Internas (DemoMvc UI) completed.

  - Key changes: added Inventory Movements UI (Index/Create/Details), filters, help modal, and navigation entry.

- TASK-0040 - Movimentacoes Internas (API + Application) completed.

  - Key changes: added CQRS, service, repository, and API endpoints for inventory movements with balance updates.

- TASK-0039 - Movimentacoes Internas (Modelo/DB) completed.

  - Key changes: added InventoryMovement entity/status, EF config, DbSet, and migration AddInventoryMovements.

- Tasking - Split TASK-0010 into smaller subtasks.

  - Key changes: created TASK-0039..TASK-0043 and updated TASK-0010 with split references and new guideline.

- Fix - Receipt AddItem model binding.

  - Key changes: bind AddItem to NewItem prefix to avoid required Receipt validation.

- Fix - Receipt AddItem numeric input handling.

  - Key changes: added validation summary, numeric input configuration, and decimal normalization on submit.

- Fix - Receipts AddItem binding.

  - Key changes: post model now binds NewItem fields to avoid Guid.Empty receipt id.

- Fix - Receipt dropdown cascade and default selection.

  - Key changes: added "Selecione..." prompts, cascading selection refresh, and guideline for dropdown defaults.

- Guideline - Respect API PageSize limits in DemoMvc lookups.

  - Key changes: added frontend rule to cap lookup PageSize to API max (100).

- Fix - Receipts page size validation.

  - Key changes: reduced lookup pageSize to 100 to satisfy API validation.

- UX - Surface API validation errors in DemoMvc.

  - Key changes: parse ValidationProblemDetails errors for friendlier messages in API client.

- TASK-0011 - Recebimento (Inbound) completed.

  - Key changes: added Receipt/ReceiptItem entities, API endpoints, DemoMvc screens, and inventory balance integration with tests and migration.

- Guideline - Prefix DONE for completed tasks.

  - Key changes: added rule to AGENTS and renamed completed task files with DONE- prefix.

- TASK-0020 - Login Help manual added.

  - Key changes: added Help button and modal manual for the Login screen.

- TASK-0019 - Dashboard Help manual added.

  - Key changes: added Help button and modal manual for the Dashboard screen.

- TASK-0038 - UI Showcase Help manual added.

  - Key changes: added Help button and modal manual for UI Showcase reference screen.

- TASK-0037 - Settings Help manual added.

  - Key changes: added Help button and modal manual for Settings screen (DemoMvc + API settings).

- TASK-0036 - Email Test Help manual added.

  - Key changes: added Help button and modal manual for Email Test screen.

- TASK-0035 - Frontend Errors Help manual added.

  - Key changes: added Help button and modal manual for Client Logs (filters, details, SignalR).

- TASK-0034 - Transaction Logs Help manual added.

  - Key changes: added Help button and modal manual for Transaction Logs (filters, details, SignalR).

- TASK-0033 - Error Logs Help manual added.

  - Key changes: added Help button and modal manual for Error Logs (filters, details, SignalR).

- TASK-0032 - Request Logs Help manual added.

  - Key changes: added Help button and modal manual for Request Logs (filters + SignalR).

- TASK-0031 - Inventory Balances Help manual added.

  - Key changes: added Help button and modal manual covering Inventory Balances Index + CRUD screens.

- TASK-0030 - Locations Help manual added.

  - Key changes: added Help button and modal manual covering Locations Index + CRUD screens.

- TASK-0029 - Aisles Help manual added.

  - Key changes: added Help button and modal manual covering Aisles Index + CRUD screens.

- TASK-0028 - Structures Help manual added.

  - Key changes: added Help button and modal manual covering Structures Index + CRUD screens.

- TASK-0027 - Sections Help manual added.

  - Key changes: added Help button and modal manual covering Sections Index + CRUD screens.

- TASK-0026 - Sectors Help manual added.

  - Key changes: added Help button and modal manual covering Sectors Index + CRUD screens.

- TASK-0025 - Warehouses Help manual added.

  - Key changes: added Help button and modal manual covering Warehouses Index + CRUD screens.

- TASK-0024 - UoM Help manual added.

  - Key changes: added Help button and modal manual covering UoM Index + CRUD screens.

- TASK-0023 - Lots Help manual added.

  - Key changes: added Help button and modal manual covering Lots Index + CRUD screens.

- TASK-0022 - Products Help manual added.

  - Key changes: added Help button and modal manual covering Products Index + CRUD and UoM conversions.

- TASK-0021 - Customers Help manual added.

  - Key changes: added Help button and modal manual covering Customers Index + CRUD screens.

- Guidelines - DemoMvc Help manuals per screen.

  - Key changes: added tasks for Help manuals and updated DemoMvc agent guidelines to require Help on Index + CRUD screens.

- TASK-0009 - Inventory balances per location completed.
