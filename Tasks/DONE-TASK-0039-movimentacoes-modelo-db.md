# TASK-0039 - Movimentacoes Internas (Modelo/DB)

## Resumo
Criar o modelo de dados e mapeamentos para movimentacoes internas de estoque.

## Contexto
Movimentacoes internas precisam de historico persistido e integracao com auditoria.

## Problema
Nao existe entidade/tabela para registrar transferencias entre enderecos.

## Objetivos
- Criar entidades e enums necessarios.
- Criar mapeamentos EF Core e migration.

## Nao Objetivos
- Nao implementar endpoints ou UI.

## Stakeholders
- Operacoes
- TI / Arquitetura

## Premissas
- Locations, Products, Lots e InventoryBalances ja existem.

## User Stories / Casos de Uso
- Como operador, quero registrar movimentacoes internas para auditoria.

## Requisitos Funcionais
- Entidade InventoryMovement com:
  - FromLocationId, ToLocationId, ProductId, LotId?
  - Quantity, Reason, Reference
  - Status, PerformedAtUtc
- Regras de integridade: FKs para Location/Product/Lot.

## Requisitos Nao Funcionais
- Auditado (CreatedAt/UpdatedAt/IsActive).
- Indexes por ProductId, FromLocationId, ToLocationId, PerformedAtUtc.

## Mudancas de API
- Nenhuma.

## Modelo de Dados / Migracoes
- Tabela InventoryMovements
- Migration: AddInventoryMovements

## Observabilidade / Logging
- TransactionLogs devem registrar alteracoes.

## Plano de Testes
- Nenhum nesta task.

## Rollout / Deploy
- Criar migration + update database.

## Riscos / Questoes em Aberto
- Definir Status enum e regras de transicao.

## Status
Concluida (DONE)
