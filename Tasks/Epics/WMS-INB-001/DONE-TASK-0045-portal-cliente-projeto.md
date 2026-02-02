# TASK-0045 - Portal do Cliente: novo projeto web

## Resumo
Criar novo projeto web Portal do Cliente com padrao UI/UX do DemoMvc.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Projeto MVC separado
- Layout/menus basicos
- Auth sessao JWT + selecao de cliente

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
- Projeto compila e roda
- UI consistente com DemoMvc
- Sem referencias diretas a API/Infra

## Status
DONE

## Progresso
- Projeto DevcraftWMS.Portal criado e adicionado à solução.
- Layout e partials alinhados ao DemoMvc.
- Login com JWT em sessão e seleção de cliente implementados.
