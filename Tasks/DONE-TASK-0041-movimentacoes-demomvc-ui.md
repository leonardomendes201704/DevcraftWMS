# TASK-0041 - Movimentacoes Internas (DemoMvc UI)

## Resumo
Criar telas de Movimentacoes Internas no DemoMvc.

## Contexto
UI deve permitir registrar e consultar movimentacoes com filtros e paginacao.

## Problema
Nao existe interface para movimentacoes.

## Objetivos
- Criar Index (grid) com filtros.
- Criar Create/Details.
- Incluir Help manual da tela.

## Nao Objetivos
- Nao incluir analytics ou relatorios.

## Stakeholders
- Operacoes
- TI / Arquitetura

## Premissas
- API da TASK-0040 pronta.

## User Stories / Casos de Uso
- Como operador, quero criar movimentacoes.
- Como gestor, quero consultar historico.

## Requisitos Funcionais
- Index paginado com filtros (produto, origem, destino, status, data).
- Create com seletores dependentes (locations, produto, lote, UoM).
- Details somente leitura.
- Botao Help com manual da tela.

## Requisitos Nao Funcionais
- PRG e mensagens amigaveis.
- Inputs numericos com mascara/step conforme diretrizes.

## Mudancas de API
- Nenhuma.

## Modelo de Dados / Migracoes
- Nenhuma.

## Observabilidade / Logging
- Usar logs de client e correlation id.

## Plano de Testes
- Smoke manual na UI.

## Rollout / Deploy
- Nenhum.

## Riscos / Questoes em Aberto
- UX de selecao de origem/destino.

## Status
Concluida (DONE)
