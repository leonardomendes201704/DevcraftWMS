# TASK-0108 - DemoMvc: fila de picking + execucao/confirmacao

## Resumo
UI Backoffice para listar e executar tarefas de picking.

## Objetivo
Permitir operador confirmar separacao por item.

## Escopo
- Tela Index + Details.
- Confirmacao de itens e status.
- Mensagens amigaveis.

## Dependencias
- TASK-0107

## Estimativa
- 6h

## Entregaveis
- UI DemoMvc para picking.

## Criterios de Aceite
- Tarefa pode ser executada e concluida via UI.

## Status
DONE

## Progresso
- UI DemoMvc criada (Index + Details) com confirmacao de itens.
- API de picking tasks adicionada (list/get/confirm) para suportar a execucao.
- Testes unitarios e de integracao adicionados.

## How to test
```bash
dotnet build DevcraftWMS/DevcraftWMS.sln
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj
```
