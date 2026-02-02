# TASK-0058 - OE: modelo de dados e status

## Resumo
Criar modelo de OE com status e vinculo ao ASN.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Entidade InboundOrder + Items
- Status OE
- Migrations

## Dependencias
- TASK-0052

## Estimativa
- 5h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- OE persiste e lista
- Status inicial Emitida
- Vinculo ASN obrigatorio

## Status
PENDENTE
