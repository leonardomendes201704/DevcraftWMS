# TASK-0049 - Zonas do armazem (staging/picking/bulk/quarantine/cross-dock)

## Resumo
Criar cadastro de zonas (staging/picking/bulk/quarantine/cross-dock).

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Entidade Zona
- CRUD + UI
- Vinculo com enderecos

## Dependencias
- TASK-0044

## Estimativa
- 5h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Zonas CRUD completo
- Enderecos referenciam zonas
- Filtros por zona funcionando

## Status
DONE
