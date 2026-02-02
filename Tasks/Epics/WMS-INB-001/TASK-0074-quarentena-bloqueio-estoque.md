# TASK-0074 - Quarentena: bloqueio de estoque

## Resumo
Bloquear estoque em quarentena para movimentacao/picking.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Regras no estoque
- Validacoes em movimentacoes
- Mensagens claras

## Dependencias
- TASK-0071,TASK-0073

## Estimativa
- 5h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Movimentacao bloqueada
- Picking ignora quarentena
- Logs registrados

## Status
PENDENTE
