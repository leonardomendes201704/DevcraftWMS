# TASK-0090 - RBAC: gestao de usuarios e perfis (API)

## Resumo
Criar endpoints para gestao de usuarios internos e atribuicao de perfis/roles.

## Objetivo
Permitir criar/editar/desativar usuarios e atribuir perfis com rastreabilidade.

## Escopo
- CRUD de usuarios internos (list/get/create/update/deactivate)
- Atribuicao de perfis (add/remove)
- Resets de senha (fluxo admin)
- Auditoria em logs

## Dependencias
- TASK-0089

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints.
- Tests (unit + integration).
- UI/ENVs se necessário.

## Criterios de Aceite
- Admin consegue criar/editar usuarios e atribuir perfis.
- Perfis impactam acesso (claims role no token).
- Logs de alteracoes registradas.


## Testes
- API: criar usuario, editar, desativar e atribuir perfis via endpoints.
- API: reset de senha admin (confirmar login com nova senha).
- Autenticacao: confirmar roles do usuario no token.
- Testes automatizados: `dotnet test DevcraftWMS.sln` (unit + integration).


## Status
DONE

