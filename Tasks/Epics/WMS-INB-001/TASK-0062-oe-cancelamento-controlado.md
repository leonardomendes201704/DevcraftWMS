# TASK-0062 - OE: cancelamento controlado com motivo

## Resumo
Cancelamento controlado de OE com motivo e permissao.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Command de cancelamento
- Registro de motivo
- Permissao por role

## Dependencias
- TASK-0058,TASK-0051

## Estimativa
- 4h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Somente autorizado cancela
- Motivo obrigatorio
- Status atualizado

## Status
PENDENTE
