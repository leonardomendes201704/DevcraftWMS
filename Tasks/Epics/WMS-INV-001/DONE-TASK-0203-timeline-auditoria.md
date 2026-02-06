# TASK-0203 - Linha do tempo e auditoria de movimentacoes

## Summary
Disponibilizar rastreabilidade com timeline de movimentacoes e ordens relacionadas (OE/OB, transferencias, inventarios).

## Objetivo
- Mostrar ultima movimentacao e historico.
- Registrar auditoria de consulta (quem consultou, quando, cliente).

## Escopo
- Endpoint de timeline por produto/lote/localizacao.
- Auditoria no LogsDb.
- Timeline exibida na UI Backoffice.

## API
- GET /api/inventory-visibility/{productId}/timeline?lotCode=&locationId=
- POST /api/inventory-visibility/audit (interno) ou middleware.

## Tests
- Unit: timeline ordenada por data.
- Integration: consulta registra log.
- Integration: UoM com codigo unico para evitar conflitos.

## Status
DONE

## Implementacao
- Endpoint `/api/inventory-visibility/{productId}/timeline` retorna eventos de:
  - Movimentacoes (InventoryMovements Completed)
  - Recebimentos (ReceiptItems + Receipts)
  - Reservas outbound (OutboundOrderReservations)
  - Inventarios (InventoryCountItems)
- Ordenacao por `OccurredAtUtc` desc no service.
- UI Inventory Visibility agora exibe aba "Timeline" quando um produto e selecionado.
- Auditoria de consulta registrada via RequestLogs (LogsDb) com query string contendo `customerId`.

## How to Test
- UI: selecione um produto na tela `/InventoryVisibility` e abra a aba Timeline.
- Swagger: `GET /api/inventory-visibility/{productId}/timeline` com `customerId` e `warehouseId`.
- Unit: `InventoryVisibilityServiceTests.GetTimelineAsync_Should_Order_By_Date`.
- Integration: `InventoryVisibilityEndpointsTests.Get_Inventory_Visibility_Timeline_Should_Return_Movements`.
