# TASK-0053 - ASN: CQRS + endpoints (create/list/get)

## Resumo
Implementar CQRS e endpoints para ASN (create/list/get).

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Commands/Queries + Validators
- Controllers MediatR-only
- RequestResult padrao

## Dependencias
- TASK-0052

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- POST/GET funcionam
- Validacoes retornam ProblemDetails
- Swagger atualizado

## Status
DONE
