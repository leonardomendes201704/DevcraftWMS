# WMS-TASK-0001 - Modulo de devolucoes (returns)

## Resumo
Implementar o fluxo completo de devolucoes de clientes (returns) com registro, recebimento, inspecao e destinacao de estoque.

## Contexto
Hoje nao existe fluxo de devolucao no WMS. O manual identificou o gap e precisa de um modulo formal com status, itens e regras de rastreio.

## Objetivo
- Criar ReturnOrder e ReturnItem.
- Permitir registrar uma devolucao (Draft).
- Permitir receber a devolucao (InProgress).
- Permitir inspecionar e finalizar (Completed) com destino do estoque por item.

## Escopo
- API + Application + Domain + Infrastructure + DemoMvc.
- Listagem paginada, detalhe, criar, receber, finalizar.
- Regras de rastreio (lote/validade) quando aplicavel.

## Fora de escopo
- Workflow de RMA externo.
- Integracao com ERP.

## Regras de negocio
- ReturnOrder associado a Customer e Warehouse.
- Vinculo opcional com OutboundOrderId.
- Status: Draft -> InProgress -> Completed (ou Canceled).
- Itens com QuantityExpected e QuantityReceived.
- Disposicao por item: Restock, Quarantine, Discard.
- Para produtos com TrackingMode != None, lote obrigatorio.
- Para TrackingMode LotAndExpiry, validade obrigatoria.

## Solucao tecnica sugerida
- Domain:
  - Enums: ReturnStatus, ReturnItemDisposition.
  - Entities: ReturnOrder, ReturnItem (AuditableEntity).
- Application:
  - Features/Returns (commands/queries/validators/service/mapping/dtos).
  - Commands: CreateReturnOrder, ReceiveReturnOrder, CompleteReturnOrder.
  - Queries: ListReturnsPaged, GetReturnById.
- Infrastructure:
  - EF configs + repositories + migration.
- API:
  - ReturnsController + Contracts.
- DemoMvc:
  - ReturnsController + ApiClient + ViewModels + Views.
- Tests:
  - Unit: ReturnService tests.
  - Integration: endpoints list/create/receive/complete.

## Telas/rotas
- /Returns (lista)
- /Returns/{id} (detalhe + acoes)

## Endpoints
- GET /api/returns
- GET /api/returns/{id}
- POST /api/returns
- POST /api/returns/{id}/receive
- POST /api/returns/{id}/complete

## Criterios de aceite
- Dado uma OS, quando registrar devolucao com itens, entao a devolucao deve ser criada em Draft.
- Dado devolucao em Draft, quando receber, entao status muda para InProgress.
- Dado devolucao em InProgress, quando finalizar com disposicoes, entao status muda para Completed e saldo ajustado.
- Validacoes de lote/validade respeitam TrackingMode.

## Logs/Auditoria
- Registrar transicoes de status e movimentos de estoque.

## Testes necessarios
- Unit: validacoes e transicoes.
- Integration: fluxo create -> receive -> complete.
- UI: lista/detalhe + acao.

## Dependencias/Riscos
- Definir regra de ajuste de estoque por disposicao.
- Garantir que InventoryBalance e InventoryMovement sejam atualizados.

## Status
PENDING
