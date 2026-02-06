# TASK-0202 - Disponibilidade real (reservas/bloqueios/processos)

## Summary
Ampliar calculo de disponibilidade incluindo reservas outbound, bloqueios de qualidade/quarentena/fiscal e itens em processo.

## Objetivo
- Calcular disponibilidade real para expedir.
- Expor breakdown: OnHand, Reserved, Blocked, InProcess, Available.

## Escopo
- Integrar InventoryBalances, OutboundOrderReservations, QualityInspections, Receipts/Putaway (em processo).
- Explicar motivos de bloqueio.

## Regras
- Available = OnHand - Reserved - Blocked - InProcess.
- Itens com status != Available devem ser separados.

## API
- Extensao do GET /api/inventory-visibility com breakdown + motivos.

## Tests
- Unit: calculo de disponibilidade e motivos.
- Integration: endpoint retorna breakdown.

## Status
PENDING
