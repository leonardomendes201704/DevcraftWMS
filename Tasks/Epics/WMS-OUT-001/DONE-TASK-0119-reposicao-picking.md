# TASK-0119 - Reposicao automatica de picking

## Resumo
Gerar tarefas de reposicao quando picking abaixo do minimo.

## Objetivo
Evitar ruptura de picking durante separacao.

## Escopo
- Regra de estoque minimo em picking.
- Gerar tarefa de reposicao.

## Dependencias
- TASK-0106

## Estimativa
- 6h

## Entregaveis
- Reposicao automatica com logs.

## Criterios de Aceite
- Tarefas criadas ao atingir minimo.

## Status
DONE

## Progresso
- Criado modelo PickingReplenishmentTask + status.
- Repositorio e CQRS para gerar e listar reposicoes.
- Regra de minimo/target configuravel via Replenishment options.
- Endpoint API para gerar e listar tarefas.
- Testes unitarios e de integracao cobrindo geracao.

## How to test
```bash
dotnet build DevcraftWMS/DevcraftWMS.sln
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj
```

### UI (DemoMvc)
1) Nao ha UI para reposicao ainda (somente API).

### Swagger (API)
1) Abrir Swagger do `DevcraftWMS.Api`.
2) Chamar `POST /api/picking-replenishments/generate` com `warehouseId` e validar retorno com tarefas criadas.
3) Chamar `GET /api/picking-replenishments` para listar as tarefas geradas.
