# TASK-0120 - RBAC outbound: permissoes por papel

## Resumo
Aplicar RBAC por papel para os fluxos outbound (picking, conferencia, packing, expedicao).

## Objetivo
Controlar acesso por papel operacional e proteger acoes criticas do outbound.

## Escopo
- Policies para outbound (picking, conferencia, packing/expedicao, pedidos).
- Controllers outbound protegidos por policy.
- Mensagens de acesso negado via middleware padrao.

## Dependencias
- TASK-0100

## Estimativa
- 4h

## Entregaveis
- Policies outbound adicionadas no API.
- Endpoints outbound com [Authorize] por papel.

## Mapeamento de roles/policies
- Role:Picking -> Admin, Backoffice, Supervisor, Picking
- Role:Conferente -> Admin, Conferente
- Role:Expedicao -> Admin, Backoffice, Supervisor, Expedicao
- Role:OutboundOrders -> Admin, Backoffice, Supervisor, Cliente
- Role:OutboundOrdersManage -> Admin, Backoffice, Supervisor

## Endpoints protegidos
- PickingTasksController: list/get/start/confirm -> Role:Picking
- OutboundChecksController: list/start -> Role:Conferente
- OutboundOrdersController:
  - create/list/get/report/notifications -> Role:OutboundOrders
  - release/cancel/resend -> Role:OutboundOrdersManage
  - check -> Role:Conferente
  - pack/packages/ship -> Role:Expedicao

## Criterios de Aceite
- Acoes criticas protegidas por role.
- Operadores so acessam suas filas (picking, conferencia, expedicao).

## How to test
- Unit tests: `dotnet test DevcraftWMS.sln --filter FullyQualifiedName~PickingTask` (smoke auth via integration)
- Integration tests: `dotnet test DevcraftWMS.sln --filter FullyQualifiedName~OutboundCheckQueueEndpointsTests`
- UI (DemoMvc):
  1) Logar como usuario de picking -> acessar fila e confirmar tarefa.
  2) Logar como conferente -> acessar fila de conferencia e iniciar.
  3) Logar como expedicao -> acessar packing/ship.
- Swagger/API:
  1) Chamar `/api/picking-tasks` com token de usuario Picking.
  2) Chamar `/api/outbound-checks` com token de usuario Conferente.
  3) Chamar `/api/outbound-orders/{id}/ship` com token de usuario Expedicao.

## Status
DONE