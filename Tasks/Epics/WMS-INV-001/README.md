# EPIC WMS-INV-001 - Visibilidade Total de Estoque por Cliente e Armazem

## Titulo
"Onde estao os produtos do Cliente X no Armazem Y?" – Visao Operacional + Rastreabilidade + Auditoria

## Contexto e motivacao
O Backoffice recebe solicitacoes diarias de clientes sobre localizacao, disponibilidade e restricoes do estoque. Hoje a informacao existe, mas esta fragmentada entre recebimentos, estoque, movimentacoes, bloqueios/qualidade e ordens abertas. Em um armazem multi-cliente, isso gera retrabalho e risco de resposta incorreta.

## Objetivo
Entregar uma experiencia completa para localizar e explicar, com confianca, onde estao os produtos do Cliente X no Armazem Y, com quantidade, status, restricoes, lotes/validade, reservas, ordens associadas e historico de movimentacoes, gerando evidencias (prints/relatorios/trilha de auditoria) para atendimento e decisao.

## Problemas que o epic resolve (resumo)
- Localizacao fisica por endereco/estrutura.
- Quantidade real por SKU/lote/validade.
- Disponivel vs indisponivel (reservas, bloqueios, processos).
- Em transito (recebimento, conferencia, staging, doca, picking).
- Ultima movimentacao e auditoria.
- Impacto de retirada parcial/total hoje.
- Unidades logisticas (pallet/caixa/unidade) e impacto operacional.

## Entregaveis macro
- Tela de consulta com filtros e drill-down.
- Servico de consolidacao de estoque + disponibilidade real.
- Integracao com reservas/outbound e bloqueios.
- Linha do tempo de movimentacoes e ordens relacionadas.
- Exportacoes (PDF/Excel/print) e resumo copiavel.
- Auditoria de consultas sensiveis.
- Documentacao/Manual interno.

## Ordem sugerida (prioridade e dependencias)
- [DONE-TASK-0200] Visao consolidada + API base de consulta | Prioridade: P0 | Dependencias: - | Estimativa: 12h
- [DONE-TASK-0201] UI Backoffice (consulta + filtros + drill-down) | Prioridade: P0 | Dependencias: TASK-0200 | Estimativa: 12h
- [DONE-TASK-0202] Disponibilidade real (reservas/bloqueios/processos) | Prioridade: P0 | Dependencias: TASK-0200 | Estimativa: 10h
- [DONE-TASK-0203] Linha do tempo e auditoria de movimentacoes | Prioridade: P1 | Dependencias: TASK-0200 | Estimativa: 10h
- [TASK-0204] Relatorios e evidencias (PDF/Excel/print) | Prioridade: P1 | Dependencias: TASK-0201,TASK-0202 | Estimativa: 10h
- [TASK-0205] Alertas operacionais e deteccao de inconsistencias | Prioridade: P1 | Dependencias: TASK-0202 | Estimativa: 12h
- [TASK-0206] Documentacao/Manual interno | Prioridade: P2 | Dependencias: TASK-0201 | Estimativa: 6h

## Referencias
- Manual do sistema: /help/manual
- Mapa de regras: /Tasks/ManualWMS_MapaDeRegras.md
