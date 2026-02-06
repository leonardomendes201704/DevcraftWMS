# TASK-0012 - Separacao (Picking) e Expedicao

## Resumo
Fluxo de saida com tarefas de picking e confirmacao.

## Contexto
Saida de pedidos e expedicao requer controle por tarefas.

## Problema
Nao existe fluxo de picking e expedicao.

## Objetivos
- CRUD de Picking e Itens.
- Confirmacao e baixa de estoque.

## Nao Objetivos
- Nao integrar com transportadoras.

## Stakeholders
- Operacoes
- Expedicao
- TI / Arquitetura

## Premissas
- Saldos e produtos existem.

## User Stories / Casos de Uso
- Como operador, quero gerar tarefas de picking.
- Como supervisor, quero confirmar expedicao e baixar estoque.

## Requisitos Funcionais
- Entidades: PickingTask, PickingItem, Shipment.
- Status: Open, Picking, Picked, Shipped, Canceled.
- Ao concluir, reduzir saldo.

## Requisitos Nao Funcionais
- Paginacao, logs, auditoria.

## Mudancas de API
- /api/picking (POST/GET)
- /api/picking/{id} (GET/PUT)
- /api/picking/{id}/items (POST/GET)
- /api/shipments (POST/GET)

## Modelo de Dados / Migracoes
- Tabelas PickingTasks, PickingItems, Shipments.

## Observabilidade / Logging
- TransactionLogs para CRUD e mudancas de status.

## Plano de Testes
- Unit: validar disponibilidade de saldo.
- Integration: criar picking e confirmar.

## Rollout / Deploy
- Migration AddPickingAndShipments.

## Riscos / Questoes em Aberto
- Definir regras de alocacao para picking.

## Status
OBSOLETA (substituida pelo epic WMS-OUT-001)

## Referencias
- WMS-OUT-001: TASK-0106 (picking model), TASK-0107 (geracao), TASK-0108 (fila/execucao)
- WMS-OUT-001: TASK-0113 (expedicao API), TASK-0114 (expedicao UI), TASK-0115 (documentos transporte)