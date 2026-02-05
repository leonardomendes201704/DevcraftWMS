# TASK-0109 - Conferencia outbound: modelo + API

## Resumo
Criar modelo de conferencia outbound e endpoints de registro.

## Objetivo
Registrar conferencia, divergencias e evidencias.

## Escopo
- Entidades de conferencia e ocorrencias.
- Endpoints /api/outbound-orders/{id}/check.
- Validacoes de quantidade e SKU.

## Dependencias
- TASK-0106

## Estimativa
- 6h

## Entregaveis
- API de conferencia + migration.

## Criterios de Aceite
- Conferencia registrada com divergencias.

## Status
DONE

## Progresso
- Modelo outbound check (check, itens, evidencias) criado com configuracoes EF e migration.
- API /api/outbound-orders/{id}/check adicionada com validacoes de quantidade/SKU.
- Testes unitarios e de integracao adicionados.

## How to test
```bash
dotnet build DevcraftWMS/DevcraftWMS.sln
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj
```
