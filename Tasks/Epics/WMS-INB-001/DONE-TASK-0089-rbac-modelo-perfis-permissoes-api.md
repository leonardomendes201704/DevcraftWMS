# TASK-0089 - RBAC: modelo de perfis e permissoes (API)

## Resumo
Introduzir modelo de perfis/roles e permissoes persistidas (RBAC) e endpoints de gestao.

## Objetivo
Permitir definir roles e permissoes via API (CRUD) e expor para o front.

## Escopo
- Entidades: Role, Permission, RolePermission, UserRole (ou User.Role se evoluir)
- CQRS + endpoints CRUD (listar/criar/editar/desativar)
- Seeds iniciais para perfis base (Admin/Backoffice/Portaria/Conferente/Qualidade/Putaway)
- Atualizar token/claims conforme perfis do usuario

## Dependencias
- DONE-TASK-0051

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints.
- Migrations.
- Tests (unit + integration).
- README/ENVs se appsettings mudar.

## Criterios de Aceite
- CRUD de perfis e permissoes funcionando.
- Usuario pode ter mais de um perfil (quando aplicavel).
- Seeds criam perfis base.
- Logs registram mudancas de permissao.

## Testes
- API: criar/listar/editar/desativar roles e permissoes no Swagger.
- API: validar seeds (roles base e permissoes) ao iniciar a API.
- Autenticacao: confirmar claims de roles no token.
- Testes automatizados: `dotnet test DevcraftWMS.sln` (unit + integration).

## Status
DONE
