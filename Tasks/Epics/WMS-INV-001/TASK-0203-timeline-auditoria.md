# TASK-0203 - Linha do tempo e auditoria de movimentacoes

## Summary
Disponibilizar rastreabilidade com timeline de movimentacoes e ordens relacionadas (OE/OB, transferencias, inventarios).

## Objetivo
- Mostrar ultima movimentacao e historico.
- Registrar auditoria de consulta (quem consultou, quando, cliente).

## Escopo
- Endpoint de timeline por produto/lote/localizacao.
- Auditoria no LogsDb.

## API
- GET /api/inventory-visibility/{productId}/timeline?lotCode=&locationId=
- POST /api/inventory-visibility/audit (interno) ou middleware.

## Tests
- Unit: timeline ordenada por data.
- Integration: consulta registra log.

## Status
PENDING
