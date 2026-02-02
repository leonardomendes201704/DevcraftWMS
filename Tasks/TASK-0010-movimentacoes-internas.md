# TASK-0010 - Movimentacoes Internas

## Resumo
Transferencias entre enderecos com historico de movimentacao.

## Contexto
Movimentacoes garantem rastreio de alteracoes de estoque.

## Problema
Nao existe historico de transferencias.

## Objetivos
- - Registrar transferencias internas.
- - Atualizar saldos de origem/destino.

## Nao Objetivos
- - Nao integrar com dispositivos RF.

## Stakeholders
- Operacoes
- TI / Arquitetura

## Premissas
- Saldos por endereco ja existentes.

## User Stories / Casos de Uso
- Como operador, quero mover estoque entre enderecos.
- Como gestor, quero auditar movimentacoes.

## Requisitos Funcionais
- - Entidade InventoryMovement: FromLocationId, ToLocationId, ProductId, LotId?, Quantity, Reason, Reference, Status, PerformedAt.
- - Regras: nao permitir saldo negativo.
- - CRUD + confirmacao.

## Requisitos Nao Funcionais
- - Transacao atomica.
- - Logs e auditoria.

## Mudancas de API
- - /api/inventory/movements (POST/GET paginado)
- - /api/inventory/movements/{id} (GET)

## Modelo de Dados / Migracoes
- - Tabela InventoryMovements (FK -> Locations/Products/Lots).

## Observabilidade / Logging
- TransactionLogs para CRUD e mudancas de status.

## Plano de Testes
- - Unit: validar saldo insuficiente.
- - Integration: criar movimento e verificar saldos.

## Rollout / Deploy
- - Migration AddInventoryMovements.

## Riscos / Questoes em Aberto
- - Definir regras de bloqueio e status de movimento.

## Status
Aberta (TODO)
