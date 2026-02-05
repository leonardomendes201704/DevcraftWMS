# TASK-0091 - RBAC: gestao de perfis e usuarios (UI Backoffice)

## Resumo
Adicionar telas de gestao de perfis/usuarios no DemoMvc (Backoffice).

## Objetivo
Permitir criar/editar perfis e usuarios e atribuir roles via UI.

## Escopo
- Menu: Usuarios / Perfis
- List/Create/Edit/Details/Deactivate para usuarios
- List/Create/Edit/Details/Deactivate para perfis
- Atribuicao de roles ao usuario
- Help/manual por tela

## Dependencias
- TASK-0090

## Estimativa
- 6h

## Entregaveis
- UI completa (CRUD + filtros + grid + help).
- Tests de integracao (smoke) se aplicavel.

## Criterios de Aceite
- Admin consegue gerenciar usuarios e perfis via UI.
- Validacoes amigaveis e logs de erros.


## Testes
- DemoMvc: criar/editar/desativar usuario e perfis nas telas de Usuarios/Perfis.
- DemoMvc: atribuir perfis e confirmar permissoes refletidas no token.
- Validacoes: conferir mensagens amigaveis e erros tratados.


## Status
DONE

