# TASK-0011 - Recebimento (Inbound)

## Resumo
Entrada de notas/ordens, conferencia e alocacao inicial.

## Contexto
Recebimento inicia ciclo do estoque no WMS.

## Problema
Nao existe fluxo de recebimento.

## Objetivos
- - CRUD de Recebimentos e Itens.
- - Confirmacao de recebimento e alocacao inicial.

## Nao Objetivos
- - Nao integrar com ERP.

## Stakeholders
- Operacoes
- Recebimento
- TI / Arquitetura

## Premissas
- Produtos e UoM existem.

## User Stories / Casos de Uso
- Como operador, quero registrar recebimento e itens.
- Como supervisor, quero confirmar recebimento e criar saldo.

## Requisitos Funcionais
- - Entidades: Receipt, ReceiptItem.
- - Status: Draft, InProgress, Completed, Canceled.
- - Ao completar, criar saldos por endereco.

## Requisitos Nao Funcionais
- - Paginacao e logs.

## Mudancas de API
- - /api/receipts (POST/GET)
- - /api/receipts/{id} (GET/PUT/DELETE)
- - /api/receipts/{id}/items (POST/GET)

## Modelo de Dados / Migracoes
- - Tabelas Receipts/ReceiptItems.

## Observabilidade / Logging
- TransactionLogs para CRUD e mudancas de status.

## Plano de Testes
- - Unit: validar status e quantidades.
- - Integration: criar receipt e completar.

## Rollout / Deploy
- - Migration AddReceipts.

## Riscos / Questoes em Aberto
- - Definir alocacao inicial (location target).

## Status
Concluida (DONE)
