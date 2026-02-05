# TASK-0107 - Picking: geracao de tarefas (wave/batch/zone)

## Resumo
Gerar tarefas de picking por metodo configurado.

## Objetivo
Criar listas otimizadas para separacao.

## Escopo
- Gerador de tarefas por OS liberada.
- Metodos: wave, batch, zone, single.
- Regras FEFO/FIFO.

## Dependencias
- TASK-0106

## Estimativa
- 6h

## Entregaveis
- Geracao automatica de tarefas.

## Criterios de Aceite
- Tarefas criadas conforme metodo escolhido.

## Como testar
- Liberar uma OS com metodo Single/Batch/Cluster e verificar a quantidade de tarefas criadas:
  - Single/Wave: 1 tarefa com todos os itens.
  - Batch: 1 tarefa por produto.
  - Cluster: 1 tarefa por item.
- Confirmar que as tarefas e itens foram persistidos em PickingTasks/PickingTaskItems.

## Status
DONE
