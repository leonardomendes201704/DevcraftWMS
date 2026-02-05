# TASK-0095 - Seed usuarios e perfis padrao (API)

## Resumo
Criar seed de usuarios operacionais e atribuir perfis RBAC padrao.

## Objetivo
Garantir usuarios base para testes/operacao com senha default.

## Escopo
- Seed de usuarios: portaria, cliente, supervisor, operador, backoffice, expedicao, picking.
- Atribuicao de roles/perfis respectivos.
- Senha padrao: Naotemsenha0!

## Dependencias
- DONE-TASK-0089
- DONE-TASK-0090
- DONE-TASK-0091

## Estimativa
- 3h

## Entregaveis
- Seeder com usuarios base.
- Logs de seed.

## Criterios de Aceite
- Usuarios criados com senha padrao.
- Roles/perfis atribuidos corretamente.
- Seed idempotente.

## Testes
- Start API e confirmar usuarios/roles via endpoints de Users/Roles.
- Login com cada usuario e validar roles no token.

## Status
DONE
