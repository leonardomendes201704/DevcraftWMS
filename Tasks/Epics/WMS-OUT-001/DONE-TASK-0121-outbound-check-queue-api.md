# TASK-0121 - Outbound checks: fila + start API (mobile support)

## Resumo
Adicionar endpoints e modelo de status para fila de conferencia outbound, habilitando uso no mobile.

## Objetivo
Permitir que o mobile liste e inicie tarefas de conferencia.

## Escopo
- Status e prioridade para OutboundCheck.
- Endpoint paginado de lista e endpoint de start.
- Regras de transicao e validacoes.
- Criacao automatica da OutboundCheck pendente ao concluir o picking.

## Detalhes tecnicos
- Campos novos no OutboundCheck:
  - Status (Pending/InProgress/Completed/Canceled).
  - Priority (OutboundOrderPriority).
  - StartedByUserId / StartedAtUtc.
- Criacao de check pendente:
  - Ao confirmar a ultima PickingTask da OS, criar OutboundCheck (se nao existir).
  - Priority deve herdar a prioridade da OS.
- Listagem (GET /api/outbound-checks):
  - Filtros: warehouseId, outboundOrderId, status, priority, isActive, includeInactive.
  - Paginacao: pageNumber/pageSize (max 100).
  - Ordenacao: orderBy + orderDir (default CreatedAtUtc desc).
  - Por padrao excluir IsActive=false.
- Start (POST /api/outbound-checks/{id}/start):
  - Bloquear se nao houver picking task ou se houver pendente.
  - Atualizar Status=InProgress e StartedAtUtc/StartedByUserId.

## Dependencias
- TASK-0109

## Estimativa
- 6h

## Entregaveis
- /api/outbound-checks (list) com filtros.
- /api/outbound-checks/{id}/start.
- Testes unitarios e integracao.

## Criterios de Aceite
- Lista paginada com filtros de status/prioridade.
- Start muda status para InProgress e registra inicio.

## Status
DONE

## How to test
- Unit tests: `dotnet test DevcraftWMS.sln --filter FullyQualifiedName~OutboundCheckServiceTests`
- Integration tests: `dotnet test DevcraftWMS.sln --filter FullyQualifiedName~OutboundCheckQueueEndpointsTests`
- UI (DemoMvc):
  1) Criar OS no Portal/DemoMvc e liberar.
  2) Executar picking e confirmar itens.
  3) Abrir tela de conferencia e validar que a OS esta disponivel para conferencia.
- Swagger/API:
  1) GET `/api/outbound-checks?pageNumber=1&pageSize=10&orderBy=CreatedAtUtc&orderDir=desc`.
  2) POST `/api/outbound-checks/{id}/start` e validar Status=InProgress e StartedAtUtc preenchido.
