# WMS-TASK-0002 - Inventario ciclico (cycle count)

## Resumo
Implementar processo de contagem ciclica por localizacao, com aprovacao e ajuste auditado de saldo.

## Contexto
Nao existe processo de contagem ciclica no codigo. O WMS precisa de contagens periodicas para garantir acuracidade.

## Objetivo
- Criar InventoryCount e InventoryCountItem.
- Permitir abrir contagem por Location/Zone.
- Registrar contagem e aprovar ajustes.

## Escopo
- API + Application + Domain + Infrastructure + DemoMvc.
- Listagem paginada, detalhe, criar, iniciar e aprovar.
- Ajustes de InventoryBalance e registro de InventoryMovement.

## Fora de escopo
- Planejamento automatico por ABC.
- Integracao externa.

## Regras de negocio
- Status: Draft -> InProgress -> Completed (ou Canceled).
- Divergencia gera ajuste de saldo.
- Aprovacao exige permissao especifica.

## Solucao tecnica sugerida
- Domain:
  - Enums: InventoryCountStatus.
  - Entities: InventoryCount, InventoryCountItem.
- Application:
  - Features/InventoryCounts (commands/queries/validators/service/mapping/dtos).
  - Commands: CreateInventoryCount, StartInventoryCount, CompleteInventoryCount.
  - Queries: ListInventoryCountsPaged, GetInventoryCountById.
- Infrastructure:
  - EF configs + repositories + migration.
- API:
  - InventoryCountsController + Contracts.
- DemoMvc:
  - InventoryCountsController + ApiClient + ViewModels + Views.
- Tests:
  - Unit: transicoes e ajustes.
  - Integration: create -> start -> complete.

## Telas/rotas
- /InventoryCounts (lista)
- /InventoryCounts/{id} (detalhe + acoes)

## Endpoints
- GET /api/inventory-counts
- GET /api/inventory-counts/{id}
- POST /api/inventory-counts
- POST /api/inventory-counts/{id}/start
- POST /api/inventory-counts/{id}/complete

## Criterios de aceite
- Dado contagem em Draft, quando iniciar, entao status muda para InProgress.
- Dado contagem em InProgress, quando finalizar, entao ajustes sao aplicados e status Completed.

## Logs/Auditoria
- Registrar ajustes e movimentos de estoque com motivo.

## Testes necessarios
- Unit: validacoes e ajustes.
- Integration: endpoints principais.
- UI: lista/detalhe + acoes.

## Dependencias/Riscos
- Definir politica de bloqueio de movimentacao durante contagem.

## Status
PENDING
