# TASK-0082 - KPIs inbound (chegada->doca->conferencia->armazenado)

## Resumo
KPIs inbound (chegada->doca->conferencia->armazenado).

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Calculo por periodo
- Dashboard KPI
- Configuracao de janelas

## Dependencias
- TASK-0079

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- KPIs exibidos no dashboard
- Filtros por periodo
- Valores consistentes

## Como testar
1) API: `GET /api/dashboard/inbound-kpis?days=7` deve retornar contadores (arrivals/dockAssigned/receiptsCompleted/putawayCompleted).
2) DemoMvc: abrir Dashboard, ajustar o seletor de periodo (7/14/30/90) e validar a atualizacao dos KPIs inbound.
3) Logs: validar que nao ha erros no carregamento dos KPIs (Request/Client logs).

## Status
DONE
