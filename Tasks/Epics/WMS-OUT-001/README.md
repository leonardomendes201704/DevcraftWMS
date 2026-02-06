# EPIC WMS-OUT-001 - Outbound (Saida e Expedicao)

## Objetivo
Implementar o fluxo completo de saida e expedicao conforme o documento Especificacao-Fluxo-Saida.txt, incluindo OS, picking, conferencia, packing, expedicao e notificacoes, com rastreio por UL/SSCC e lote/validade.

## Documento base
- Especificacao-Fluxo-Saida.txt
- Especificacao-Fluxo-Saida.html

## Ja existente (reutilizar/ajustar)
- Cadastros base: Warehouse, Zone, Location, Products, UoM, Lots, InventoryBalances.
- RBAC base e perfis.
- Portal do Cliente e Backoffice (DemoMvc).

## Ordem sugerida (prioridade e dependencias)
- [DONE-TASK-0100] Epic Outbound: indice e rastreio de escopo | Prioridade: P0 | Dependencias: - | Estimativa: 2h
- [DONE-TASK-0101] OS: modelo de dados e status | Prioridade: P0 | Dependencias: TASK-0100 | Estimativa: 6h
- [DONE-TASK-0102] OS: CQRS + endpoints (create/list/get) | Prioridade: P0 | Dependencias: TASK-0101 | Estimativa: 6h
- [DONE-TASK-0103] Portal Cliente: UI criar OS | Prioridade: P0 | Dependencias: TASK-0102 | Estimativa: 6h
- [DONE-TASK-0104] Backoffice: liberar OS + parametros (prioridade, metodo picking, janela) | Prioridade: P1 | Dependencias: TASK-0102 | Estimativa: 6h
- [DONE-TASK-0105] Reserva de estoque e validacao disponibilidade | Prioridade: P0 | Dependencias: TASK-0102 | Estimativa: 6h
- [DONE-TASK-0106] Picking: modelo de tarefas e status | Prioridade: P0 | Dependencias: TASK-0101 | Estimativa: 6h
- [DONE-TASK-0107] Picking: geracao de tarefas (wave/batch/zone) | Prioridade: P1 | Dependencias: TASK-0106 | Estimativa: 6h
- [DONE-TASK-0108] DemoMvc: fila de picking + execucao/confirmacao | Prioridade: P1 | Dependencias: TASK-0107 | Estimativa: 6h
- [DONE-TASK-0109] Conferencia outbound: modelo + API | Prioridade: P1 | Dependencias: TASK-0106 | Estimativa: 6h
- [DONE-TASK-0110] DemoMvc: conferencia + divergencias | Prioridade: P1 | Dependencias: TASK-0109 | Estimativa: 6h
- [DONE-TASK-0111] Packing: modelo + API (volumes, peso, etiqueta) | Prioridade: P1 | Dependencias: TASK-0109 | Estimativa: 6h
- [DONE-TASK-0112] DemoMvc: tela de packing + etiquetas | Prioridade: P2 | Dependencias: TASK-0111 | Estimativa: 6h
- [DONE-TASK-0113] Expedicao: doca/carregamento + API | Prioridade: P1 | Dependencias: TASK-0111 | Estimativa: 6h
- [DONE-TASK-0114] DemoMvc: expedicao (carregar, finalizar) | Prioridade: P2 | Dependencias: TASK-0113 | Estimativa: 6h
- [DONE-TASK-0115] Documentos de transporte (romaneio/relatorio) | Prioridade: P2 | Dependencias: TASK-0113 | Estimativa: 6h
- [DONE-TASK-0116] Notificacoes outbound (portal/email/webhook) | Prioridade: P2 | Dependencias: TASK-0115 | Estimativa: 6h
- [DONE-TASK-0117] KPIs outbound (separacao, conferencia, OS->expedido) | Prioridade: P2 | Dependencias: TASK-0113 | Estimativa: 6h
- [DONE-TASK-0118] Fluxos alternativos (parcial, cancelamento pos separacao, cross-dock) | Prioridade: P2 | Dependencias: TASK-0106 | Estimativa: 6h
- [DONE-TASK-0119] Reposicao automatica de picking | Prioridade: P2 | Dependencias: TASK-0106 | Estimativa: 6h
- [DONE-TASK-0121] Outbound checks: fila + start API (mobile) | Prioridade: P1 | Dependencias: TASK-0109 | Estimativa: 6h
- [DONE-TASK-0122] Seed outbound check flow (picking + checks) | Prioridade: P2 | Dependencias: TASK-0121 | Estimativa: 2h
- [TASK-0120] RBAC outbound: permissoes por papel | Prioridade: P1 | Dependencias: TASK-0100 | Estimativa: 4h
