# TASK-0088 - Portaria: permissoes por papel

## Resumo
Aplicar permissoes de portaria por papel.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Roles no Portaria
- Restricoes de acoes criticas
- Mensagens amigaveis

## Dependencias
- TASK-0051,TASK-0063

## Estimativa
- 4h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Acoes criticas bloqueadas sem role
- Logs registram bloqueio
- UI indica falta de permissao

## Status
DONE

## Implementacao
- Adicionado contexto de usuario no Portaria (roles a partir do JWT).
- Bloqueio e log de acoes criticas (create, send to queue, assign dock, cancel).
- UI indica falta de permissao e desabilita botoes.
- Mensagens amigaveis para 403 no cliente HTTP.

## Como testar
1) Entre no Portaria com usuario sem role Portaria/Admin e tente criar/atualizar um check-in.
2) Verifique que os botoes ficam desabilitados e aparece alerta de permissao.
3) Com usuario Portaria/Admin, confirme que as acoes funcionam normalmente.
