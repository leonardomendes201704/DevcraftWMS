# TASK-0042 - Movimentacoes Internas (Testes)

## Resumo
Adicionar testes unitarios e integracao para movimentacoes internas.

## Contexto
Garantir regressao e regras de saldo.

## Problema
Nenhum teste cobre movimentacoes internas.

## Objetivos
- Unit: validacao de saldo e regras.
- Integration: criar movimento e validar saldos atualizados.

## Nao Objetivos
- Nao testar UI.

## Stakeholders
- QA
- TI / Arquitetura

## Premissas
- API e modelo implementados.

## User Stories / Casos de Uso
- Como QA, quero garantir que saldo nunca fica negativo.

## Requisitos Funcionais
- Teste de falha quando saldo insuficiente.
- Teste de sucesso criando movimento.

## Requisitos Nao Funcionais
- Limpeza de dados e regressao (sem lixo na base).

## Mudancas de API
- Nenhuma.

## Modelo de Dados / Migracoes
- Nenhuma.

## Observabilidade / Logging
- Validar TransactionLogs quando aplicavel.

## Plano de Testes
- dotnet test

## Rollout / Deploy
- Nenhum.

## Riscos / Questoes em Aberto
- Dependencias de seed para dados base.

## Status
Aberta (TODO)
