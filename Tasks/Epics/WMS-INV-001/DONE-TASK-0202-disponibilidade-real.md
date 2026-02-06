# TASK-0202 - Disponibilidade real (reservas/bloqueios/processos)

## Summary
Ampliar calculo de disponibilidade incluindo reservas outbound, bloqueios de qualidade/quarentena/fiscal e itens em processo.

## Objetivo
- Calcular disponibilidade real para expedir.
- Expor breakdown: OnHand, Reserved, Blocked, InProcess, Available.

## Escopo
- Integrar InventoryBalances, OutboundOrderReservations, QualityInspections, Receipts/Putaway (em processo).
- Explicar motivos de bloqueio.
 - Expor colunas de bloqueio/processo na UI Backoffice.

## Regras
- Available = OnHand - Reserved - Blocked - InProcess.
- Itens com status != Available devem ser separados.

## API
- Extensao do GET /api/inventory-visibility com breakdown + motivos.

## Tests
- Unit: calculo de disponibilidade e motivos.
- Integration: endpoint retorna breakdown.
 - UI: Inventory Visibility mostra colunas de bloqueio/processo e razoes.

## Status
DONE

## Implementacao
- Repository agora calcula reservas por balance e filtra OS abertas.
- Inspecoes de qualidade (Pending/Rejected) entram como bloqueio.
- Receipts em Draft/InProgress entram como "InProcess".
- Location DTO inclui QuantityBlocked/QuantityInProcess/QuantityAvailable + BlockedReasons.
- UI mostra colunas de bloqueio/processo/available e badges de motivos.

## How to Test
- UI: abrir `/InventoryVisibility`, aplicar filtros, validar colunas Blocked/In process/Available e motivos.
- Swagger: `GET /api/inventory-visibility` e validar `quantityBlocked`, `quantityInProcess`, `quantityAvailable`, `blockedReasons`.
- Unit: `InventoryVisibilityServiceTests.GetAsync_Should_Use_Reservations_Inspections_And_InProcess`.
