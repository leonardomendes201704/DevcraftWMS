# TASK-0124 - Pagina de Login sem Layout (Portal/Portaria/DemoMvc)

## Summary
Remover o layout compartilhado da tela de login e exibir somente o formulario (sem menu, sem sidebar, sem header padrao) nos tres projetos web.

## Objetivo
- Exibir pagina de login isolada (layout limpo) em DemoMvc, Portal e Portaria.
- Manter padrao visual consistente (tipografia, cores, logo/brand).
- Preservar validacoes e mensagens atuais.

## Escopo
- Layout dedicado de login em cada projeto.
- Views de login apontando para o layout limpo.

## Fora de Escopo
- Alterar regras de autenticacao.
- Alterar endpoints ou fluxo de login.
- Mudancas de identidade visual fora da pagina de login.

## Implementacao
- `Views/Shared/_LoginLayout.cshtml` em DemoMvc, Portal e Portaria.
- `Views/Auth/Login.cshtml` usa `Layout = "_LoginLayout"`.

## How to Test
- DemoMvc: `https://localhost:7024/Auth/Login` (sem sidebar/topbar).
- Portal: `https://localhost:<porta>/Auth/Login`.
- Portaria: `https://localhost:<porta>/Auth/Login`.
- Validar mensagens de erro e responsividade.

## Status
DONE
