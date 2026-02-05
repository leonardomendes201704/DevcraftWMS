# TASK-0104 - Backoffice: liberar OS + parametros

## Resumo
Backoffice libera OS e define parametros de picking e prioridade.

## Objetivo
Habilitar workflow de liberacao e definicao de metodo de separacao.

## Escopo
- Endpoint /api/outbound-orders/{id}/release.
- Campos: prioridade, metodo picking, janela de expedicao.
- UI DemoMvc para liberar.

## Dependencias
- TASK-0102

## Estimativa
- 6h

## Entregaveis
- Release API + UI Backoffice.

## Criterios de Aceite
- OS muda para Liberada com parametros persistidos.

## Status
PENDENTE
