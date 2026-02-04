# TASK-0078 - Putaway: reatribuicao manual

## Resumo
Reatribuicao manual de tarefa de putaway.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Endpoint de reatribuicao
- Motivo obrigatorio
- Permissao por role

## Dependencias
- TASK-0075

## Estimativa
- 4h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Reatribuicao registrada
- Motivo obrigatorio
- Historico visivel

## Status
DONE

## How to test
1) Run API + DemoMvc.
2) Go to Putaway Tasks > open a task details.
3) Fill Assignee email (e.g., admin@admin.com.br) and a Reason, then submit.
4) Verify Assigned To is updated and Assignment History shows the event.
5) Try reassigning a completed task and confirm it is blocked.
