# TASK-0077 - Putaway: execucao e confirmacao

## Resumo
Execucao do putaway com confirmacao UL + endereco.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Tela/endpoint de confirmacao
- Validacao dupla
- Atualizacao de estoque

## Dependencias
- TASK-0075

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Sem confirmar UL+endereco nao finaliza
- Estoque enderecado criado
- Status UL atualizado

## How to test
1) Crie um Receipt com item e finalize o recebimento (Complete Receipt).
2) Crie um Unit Load e imprima a etiqueta (gera Putaway Task).
3) DemoMvc -> Putaway Tasks -> Details -> selecione um endereco e confirme.
   - Esperado: status da tarefa como Completed e UL como PutawayCompleted.
4) API: POST /api/putaway-tasks/{id}/confirm com LocationId.
   - Esperado: sucesso e saldo movido para o novo endereco.

## Status
DONE
