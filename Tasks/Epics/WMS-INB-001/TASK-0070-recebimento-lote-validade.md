# TASK-0070 - Recebimento: captura lote/validade

## Resumo
Captura obrigatoria de lote/validade em SKUs controlados.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Campos no recebimento
- Validacoes por SKU
- Persistencia em Lot

## Dependencias
- TASK-0047,TASK-0068

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Sem lote/validade nao finaliza
- SKU None nao exige
- UI com mascaras de data

## Status
PENDENTE
