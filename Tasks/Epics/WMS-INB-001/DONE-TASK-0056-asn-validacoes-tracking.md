# TASK-0056 - ASN: validacoes por TrackingMode

## Resumo
Aplicar validacoes do TrackingMode no ASN.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Lote/validade obrigatorio conforme SKU
- Mensagens por item
- Bloqueio de submissao

## Dependencias
- TASK-0047,TASK-0053

## Estimativa
- 4h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- SKU Lot+Expiry exige ambos
- SKU Lot exige lote
- SKU None nao exige

## Status
DONE
