# EPIC WMS-INB-001 - Inbound & Putaway (Entrada e Armazenagem)

## Objetivo
Implementar o fluxo completo de entrada e armazenagem conforme o documento Especificacao-Fluxo-Entrada.txt, incluindo ASN, OE, portaria, recebimento, qualidade/quarentena, putaway e encerramento com notificacoes.

## Ja existente (reutilizar/ajustar)
- Cadastros base: Warehouse, Sector, Section, Structure, Location, Products, UoM, Lots.
- Inventario: InventoryBalances e InventoryMovements.
- Receipts basico (precisa alinhar com OE/romaneio).
- Logs/telemetria e contexto de cliente (X-Customer-Id).

## Projetos web novos (obrigatorios)
- Portal do Cliente (novo projeto web separado).
- Portaria (novo projeto web separado).

## Ordem sugerida (prioridade e dependencias)
- [TASK-0044] Epic Inbound: indice e rastreio de escopo | Prioridade: P0 | Dependencias: - | Estimativa: 2h
- [TASK-0045] Portal do Cliente: novo projeto web | Prioridade: P0 | Dependencias: TASK-0044 | Estimativa: 6h
- [TASK-0046] Portaria: novo projeto web | Prioridade: P0 | Dependencias: TASK-0044 | Estimativa: 6h
- [TASK-0047] SKU: TrackingMode (None/Lot/LotAndExpiry) | Prioridade: P0 | Dependencias: TASK-0044 | Estimativa: 4h
- [TASK-0048] SKU/Cliente: regra de validade minima | Prioridade: P0 | Dependencias: TASK-0047 | Estimativa: 4h
- [DONE-TASK-0049] Zonas do armazem (staging/picking/bulk/quarantine/cross-dock) | Prioridade: P0 | Dependencias: TASK-0044 | Estimativa: 5h
- [DONE-TASK-0050] Enderecos: capacidade e restricoes/compatibilidade | Prioridade: P0 | Dependencias: TASK-0049 | Estimativa: 6h
- [DONE-TASK-0051] RBAC: perfis e permissoes base por papel | Prioridade: P1 | Dependencias: TASK-0044 | Estimativa: 6h
- [DONE-TASK-0052] ASN: modelo de dados e status | Prioridade: P0 | Dependencias: TASK-0047 | Estimativa: 5h
- [DONE-TASK-0053] ASN: CQRS + endpoints (create/list/get) | Prioridade: P0 | Dependencias: TASK-0052 | Estimativa: 6h
- [DONE-TASK-0054] Portal Cliente: UI criar ASN | Prioridade: P0 | Dependencias: TASK-0045,TASK-0053 | Estimativa: 6h
- [DONE-TASK-0055] ASN: anexos (XML/PDF/imagem) | Prioridade: P1 | Dependencias: TASK-0053 | Estimativa: 6h
- [DONE-TASK-0056] ASN: validacoes por TrackingMode | Prioridade: P0 | Dependencias: TASK-0047,TASK-0053 | Estimativa: 4h
- [DONE-TASK-0057] ASN: workflow de status + auditoria | Prioridade: P1 | Dependencias: TASK-0053 | Estimativa: 4h
- [DONE-TASK-0058] OE: modelo de dados e status | Prioridade: P0 | Dependencias: TASK-0052 | Estimativa: 5h
- [DONE-TASK-0059] OE: converter ASN -> OE | Prioridade: P0 | Dependencias: TASK-0058,TASK-0053 | Estimativa: 5h
- [DONE-TASK-0060] OE: parametros de conferencia/prioridade/doca sugerida | Prioridade: P1 | Dependencias: TASK-0058 | Estimativa: 5h
- [DONE-TASK-0061] Backoffice: fila de OEs por status/prioridade | Prioridade: P1 | Dependencias: TASK-0058 | Estimativa: 6h
- [DONE-TASK-0062] OE: cancelamento controlado com motivo | Prioridade: P1 | Dependencias: TASK-0058,TASK-0051 | Estimativa: 4h
- [TASK-0063] Portaria: API check-in veiculo + vinculo OE | Prioridade: P0 | Dependencias: TASK-0046,TASK-0058 | Estimativa: 6h
- [TASK-0064] Portaria: UI check-in e fila de docas | Prioridade: P0 | Dependencias: TASK-0063 | Estimativa: 6h
- [TASK-0065] Portaria: atribuicao de doca e status | Prioridade: P0 | Dependencias: TASK-0063 | Estimativa: 4h
- [TASK-0066] Recebimento: sessao (romaneio) vinculada a OE | Prioridade: P0 | Dependencias: TASK-0058 | Estimativa: 6h
- [TASK-0067] UL/SSCC: modelo + etiqueta interna | Prioridade: P0 | Dependencias: TASK-0066 | Estimativa: 6h
- [TASK-0068] Recebimento: conferencia cega/assistida | Prioridade: P0 | Dependencias: TASK-0066,TASK-0060 | Estimativa: 6h
- [TASK-0069] Divergencias: ocorrencias + evidencias (foto) | Prioridade: P1 | Dependencias: TASK-0068 | Estimativa: 6h
- [TASK-0070] Recebimento: captura lote/validade | Prioridade: P0 | Dependencias: TASK-0047,TASK-0068 | Estimativa: 6h
- [TASK-0071] Validade minima: quarentena automatica | Prioridade: P0 | Dependencias: TASK-0048,TASK-0070 | Estimativa: 4h
- [TASK-0072] Qualidade: fila de inspecao | Prioridade: P1 | Dependencias: TASK-0068 | Estimativa: 5h
- [TASK-0073] Qualidade: registrar inspecao e decisao | Prioridade: P1 | Dependencias: TASK-0072 | Estimativa: 6h
- [TASK-0074] Quarentena: bloqueio de estoque | Prioridade: P0 | Dependencias: TASK-0071,TASK-0073 | Estimativa: 5h
- [TASK-0075] Putaway: modelo e geracao de tarefas | Prioridade: P0 | Dependencias: TASK-0067,TASK-0049,TASK-0050 | Estimativa: 6h
- [TASK-0076] Putaway: motor de sugestao de endereco | Prioridade: P0 | Dependencias: TASK-0075 | Estimativa: 6h
- [TASK-0077] Putaway: execucao e confirmacao | Prioridade: P0 | Dependencias: TASK-0075 | Estimativa: 6h
- [TASK-0078] Putaway: reatribuicao manual | Prioridade: P1 | Dependencias: TASK-0075 | Estimativa: 4h
- [TASK-0079] OE: validacoes de encerramento | Prioridade: P0 | Dependencias: TASK-0077,TASK-0074 | Estimativa: 5h
- [TASK-0080] OE: relatorio final de recebimento | Prioridade: P1 | Dependencias: TASK-0079 | Estimativa: 6h
- [TASK-0081] Notificacao ao cliente (portal/email/webhook) | Prioridade: P1 | Dependencias: TASK-0080,TASK-0045 | Estimativa: 6h
- [TASK-0082] KPIs inbound (chegada->doca->conferencia->armazenado) | Prioridade: P1 | Dependencias: TASK-0079 | Estimativa: 6h
- [TASK-0083] Fluxos alternativos: sem OE e entrada parcial | Prioridade: P2 | Dependencias: TASK-0058,TASK-0066 | Estimativa: 6h
- [TASK-0084] Cross-dock: excecao sem putaway | Prioridade: P2 | Dependencias: TASK-0075 | Estimativa: 6h
- [TASK-0085] Reetiquetagem UL/SSCC | Prioridade: P2 | Dependencias: TASK-0067 | Estimativa: 4h
- [TASK-0086] Cubagem/Pesagem real no recebimento | Prioridade: P2 | Dependencias: TASK-0047 | Estimativa: 6h
- [TASK-0087] Portal Cliente: relatorios de recebimento | Prioridade: P2 | Dependencias: TASK-0080,TASK-0045 | Estimativa: 6h
- [TASK-0088] Portaria: permissoes por papel | Prioridade: P2 | Dependencias: TASK-0051,TASK-0063 | Estimativa: 4h
- [TASK-0089] RBAC: modelo de perfis e permissoes (API) | Prioridade: P1 | Dependencias: DONE-TASK-0051 | Estimativa: 6h
- [TASK-0090] RBAC: gestao de usuarios e perfis (API) | Prioridade: P1 | Dependencias: TASK-0089 | Estimativa: 6h
- [TASK-0091] RBAC: gestao de perfis e usuarios (UI Backoffice) | Prioridade: P1 | Dependencias: TASK-0090 | Estimativa: 6h
- [TASK-0092] ASN anexos: storage real (abstracao + config) | Prioridade: P1 | Dependencias: DONE-TASK-0055 | Estimativa: 6h
- [TASK-0093] ASN anexos: API upload/download real | Prioridade: P1 | Dependencias: TASK-0092 | Estimativa: 6h
- [TASK-0094] ASN anexos: Portal download/preview | Prioridade: P2 | Dependencias: TASK-0093 | Estimativa: 4h
