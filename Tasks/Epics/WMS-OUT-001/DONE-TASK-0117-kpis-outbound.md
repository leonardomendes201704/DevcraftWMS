# TASK-0117 - KPIs outbound

## Resumo
Criar KPIs de saida (separacao, conferencia, expedicao).

## Objetivo
Medir tempo e qualidade do processo outbound.

## Escopo
- Endpoint de KPIs.
- Dashboard com indicadores.

## Dependencias
- TASK-0113

## Estimativa
- 6h

## Entregaveis
- KPIs no backoffice.

## Criterios de Aceite
- KPIs calculados por periodo.

## Status
DONE

## Progresso
- KPI outbound por janela (picking concluido, conferencia concluida, expedicao concluida).
- Endpoint `GET /api/dashboard/outbound-kpis`.
- Dashboard DemoMvc com cards de outbound KPIs.
- Testes de integracao cobrindo endpoint.

## How to test
```bash
dotnet build DevcraftWMS/DevcraftWMS.sln
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj
```

### UI (DemoMvc)
1) Abrir o Dashboard no DemoMvc.
2) Verificar a secao **Outbound KPIs** e ajustar o filtro de janela.
3) Validar contadores de Picking/Checks/Shipments.

### Swagger (API)
1) Abrir Swagger do `DevcraftWMS.Api`.
2) Chamar `GET /api/dashboard/outbound-kpis?days=7`.
3) Validar os campos `pickingCompleted`, `checksCompleted`, `shipmentsCompleted`.
