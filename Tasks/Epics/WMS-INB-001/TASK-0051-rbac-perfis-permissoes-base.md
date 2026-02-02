# TASK-0051 - RBAC: perfis e permissoes base por papel

## Resumo
Definir perfis e permissoes base por papel operacional.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Perfis: portaria/conferente/qualidade/putaway/backoffice
- Gate por role/claim
- Documentacao de permissoes

## Dependencias
- TASK-0044

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Rotas criticas protegidas
- Usuarios sem permissao bloqueados
- Logs registram bloqueios

## Status
PENDENTE
