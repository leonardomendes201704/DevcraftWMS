# TASK-0047 - SKU: TrackingMode (None/Lot/LotAndExpiry)

## Resumo
Adicionar TrackingMode no SKU (None/Lot/LotAndExpiry).

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Enum + DisplayName
- Persistencia + migrations
- Exposicao em API/UI

## Dependencias
- TASK-0044

## Estimativa
- 4h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- SKU salva TrackingMode
- Validacoes respeitam TrackingMode
- Swagger e UI atualizados

## Status
PENDENTE
