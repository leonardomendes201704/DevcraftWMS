# Manual WMS - Backlog de Implementacao (Gaps)

## Status (2026-02-06)
Os itens abaixo foram implementados no backlog tecnico e encerrados.


## WMS-TASK-0001 - Modulo de devolucoes (returns)
- Status: CONCLUIDO (2026-02-06)
- Contexto: nao ha fluxo de devolucao registrado no codigo.
- Objetivo: permitir registrar devolucoes de clientes, com inspecao e destino de estoque.
- Escopo:
  - Entidades: ReturnOrder, ReturnItem, ReturnStatus.
  - Fluxo: abrir devolucao -> receber -> inspecionar -> decidir (reintegrar/descartar/quarentena).
- Regras de negocio:
  - Associar devolucao a OS original (quando houver).
  - Lote/validade obrigatorios quando produto exige rastreio.
- Solucao tecnica sugerida:
  - Controllers: `DevcraftWMS.Api/Controllers/ReturnsController.cs`
  - CQRS: `Application/Features/Returns` (commands/queries/validators)
  - Domain: entidades + enum status
  - Infrastructure: configs, repository, migration
  - Tests: unit + integration
- Telas afetadas: /Returns (lista), /Returns/{id} (detalhe)
- Endpoints afetados: /api/returns, /api/returns/{id}
- Criterios de aceite (Given/When/Then):
  - Dado uma OS, quando registrar devolucao com itens, entao devolucao deve ser criada com status Draft.
  - Dado devolucao em Draft, quando confirmar recebimento, entao status muda para InProgress.
- Logs/Auditoria: registrar transicoes e movimentos de estoque.
- Testes: unit (validacoes), integration (fluxo completo), UI (list/detail).
- Riscos/Dependencias: definicao de destino de estoque e regra de reuso de lotes.
- Prioridade: Media | Esforco: G

## WMS-TASK-0002 - Inventario ciclico (cycle count)
- Status: CONCLUIDO (2026-02-06)
- Contexto: nao existe processo de contagem ciclica no codigo.
- Objetivo: permitir contagem periodica por localizacao e ajustes auditados.
- Escopo:
  - Tarefas de contagem por Location/Zone.
  - Aprovação de ajustes.
- Regras de negocio:
  - Divergencia gera ajuste e movimentacao de estoque.
  - Permissoes especificas para aprovacao.
- Solucao tecnica sugerida:
  - Controllers: `InventoryCountsController`
  - CQRS: `Application/Features/InventoryCounts`
  - Domain: InventoryCount, InventoryCountItem, InventoryCountStatus
  - Infrastructure: configs, repository, migration
- Telas afetadas: /InventoryCounts
- Endpoints afetados: /api/inventory-counts
- Criterios de aceite:
  - Dado uma contagem, quando aprovado, entao saldo e atualizado e mov. registrado.
- Logs/Auditoria: incluir transaction log.
- Testes: unit/integration/UI.
- Riscos/Dependencias: politica de bloqueio durante contagem.
- Prioridade: Media | Esforco: M

## WMS-TASK-0003 - Agenda de doca outbound
- Status: CONCLUIDO (2026-02-06)
- Contexto: shipping nao possui agenda formal de doca no codigo.
- Objetivo: controlar janelas de doca para embarque e evitar conflito de horarios.
- Escopo:
  - Criar DockSchedule com slots.
  - Associar embarques a slots.
- Regras de negocio:
  - Bloquear slots sobrepostos.
  - Permitir reagendamento com motivo.
- Solucao tecnica sugerida:
  - Controllers: `DockSchedulesController`
  - CQRS: `Application/Features/DockSchedules`
  - Domain: DockSchedule, DockStatus
  - Infra: migration + repo
- Telas afetadas: /DockSchedules, /OutboundShipping (selecionar slot)
- Endpoints afetados: /api/dock-schedules
- Criterios de aceite:
  - Dado um slot ocupado, quando tentar usar, entao retorno erro de conflito.
- Logs/Auditoria: logar reagendamentos.
- Testes: unit/integration/UI.
- Riscos/Dependencias: definicao de docas e capacidade.
- Prioridade: Media | Esforco: M
