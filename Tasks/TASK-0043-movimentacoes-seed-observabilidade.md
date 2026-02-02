# TASK-0043 - Movimentacoes Internas (Seed + Observabilidade)

## Resumo
Adicionar seed opcional e ajustes de observabilidade para movimentacoes internas.

## Contexto
Dados de demo ajudam a validar fluxo end-to-end.

## Problema
Sem dados, UX e dashboards ficam vazios.

## Objetivos
- Seed opcional de movimentacoes internas.
- Garantir logs/auditoria consistentes.

## Nao Objetivos
- Nao criar relatorios.

## Stakeholders
- Operacoes
- TI / Arquitetura

## Premissas
- SampleDataSeeder configurado.

## User Stories / Casos de Uso
- Como usuario, quero ver historico demo de movimentacoes.

## Requisitos Funcionais
- Criar X movimentos para cliente demo.
- Ajustar saldos de origem/destino coerentes.

## Requisitos Nao Funcionais
- Seed controlado por appsettings.

## Mudancas de API
- Nenhuma.

## Modelo de Dados / Migracoes
- Nenhuma.

## Observabilidade / Logging
- Verificar TransactionLogs.

## Plano de Testes
- Smoke com seed habilitado.

## Rollout / Deploy
- Nenhum.

## Riscos / Questoes em Aberto
- Evitar duplicacao de dados na seed.

## Status
Aberta (TODO)
