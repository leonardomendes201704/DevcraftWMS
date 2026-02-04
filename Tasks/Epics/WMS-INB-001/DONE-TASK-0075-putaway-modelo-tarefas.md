# TASK-0075 - Putaway: modelo e geracao de tarefas

## Resumo
Modelo e geracao de tarefas de putaway por UL.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Entidade PutawayTask
- Criacao automatica
- Status da tarefa

## Dependencias
- TASK-0067,TASK-0049,TASK-0050

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Tarefas criadas ao liberar UL
- Status inicial Pendente
- UI listagem basica

## How to test
1) Crie um Unit Load e execute Print Label (liberar UL).
2) Acesse DemoMvc -> Putaway Tasks.
   - Esperado: tarefa criada com Status = Pending e SSCC do UL.
3) Opcional via API:
   - GET /api/putaway-tasks (com X-Customer-Id)
   - Esperado: tarefa listada.

## Status
DONE
