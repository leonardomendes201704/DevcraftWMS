# EPIC WMS-MOB-001 - Mobile App (Operacao WMS)

## Objetivo
Entregar o app mobile operacional integrado ao WMS para picking, conferencia, packing, expedicao, recebimento e inventario, com foco em produtividade e rastreabilidade.

## Escopo geral
- Telas operacionais (fila + execucao).
- Consulta rapida de estoque e movimentacoes.
- Alertas e tarefas do operador.
- KPIs rapidos.

## Ordem sugerida (prioridade e dependencias)
- [TASK-0100] Epic Mobile: indice e escopo | Prioridade: P0 | Dependencias: - | Estimativa: 2h
- [TASK-0101] Mobile: autenticacao + selecao de armazem | Prioridade: P0 | Dependencias: TASK-0100 | Estimativa: 6h
- [TASK-0102] Mobile: Picking - fila de tarefas | Prioridade: P0 | Dependencias: TASK-0100 | Estimativa: 6h
- [TASK-0103] Mobile: Picking - execucao da tarefa | Prioridade: P0 | Dependencias: TASK-0102 | Estimativa: 8h
- [DONE-TASK-0104] Mobile: Conferencia - fila | Prioridade: P0 | Dependencias: TASK-0100 | Estimativa: 6h
- [TASK-0105] Mobile: Conferencia - execucao | Prioridade: P0 | Dependencias: TASK-0104 | Estimativa: 8h
- [TASK-0106] Mobile: Packing - fila | Prioridade: P0 | Dependencias: TASK-0100 | Estimativa: 6h
- [TASK-0107] Mobile: Packing - execucao | Prioridade: P0 | Dependencias: TASK-0106 | Estimativa: 8h
- [TASK-0108] Mobile: Expedicao - fila | Prioridade: P0 | Dependencias: TASK-0100 | Estimativa: 6h
- [TASK-0109] Mobile: Expedicao - execucao | Prioridade: P0 | Dependencias: TASK-0108 | Estimativa: 8h
- [TASK-0110] Mobile: Recebimento - fila/check-in | Prioridade: P1 | Dependencias: TASK-0100 | Estimativa: 6h
- [TASK-0111] Mobile: Recebimento - execucao (conferencia) | Prioridade: P1 | Dependencias: TASK-0110 | Estimativa: 8h
- [TASK-0112] Mobile: Putaway - fila | Prioridade: P1 | Dependencias: TASK-0100 | Estimativa: 6h
- [TASK-0113] Mobile: Putaway - execucao | Prioridade: P1 | Dependencias: TASK-0112 | Estimativa: 8h
- [TASK-0114] Mobile: Inventario - consulta saldo/posicao | Prioridade: P1 | Dependencias: TASK-0100 | Estimativa: 6h
- [TASK-0115] Mobile: Movimentacoes - transferencia/ajuste | Prioridade: P1 | Dependencias: TASK-0114 | Estimativa: 8h
- [TASK-0116] Mobile: Reposicao de picking | Prioridade: P2 | Dependencias: TASK-0100 | Estimativa: 6h
- [TASK-0117] Mobile: Minhas tarefas | Prioridade: P1 | Dependencias: TASK-0100 | Estimativa: 4h
- [TASK-0118] Mobile: Alertas e divergencias | Prioridade: P2 | Dependencias: TASK-0100 | Estimativa: 6h
- [TASK-0119] Mobile: KPIs rapidos | Prioridade: P2 | Dependencias: TASK-0100 | Estimativa: 4h
- [TASK-0120] Mobile: Cadastro rapido (produto/lote) | Prioridade: P3 | Dependencias: TASK-0100 | Estimativa: 6h
- [TASK-0121] Mobile: Auditoria/historico | Prioridade: P3 | Dependencias: TASK-0100 | Estimativa: 6h
