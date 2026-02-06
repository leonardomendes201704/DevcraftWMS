# TASK-0123 - Manual do Sistema WMS (Views + Mapa de Regras + Backlog de Gaps)

## Resumo
Gerar um Manual do Sistema completo (inbound + outbound) baseado no codigo real do projeto, publicado como Views (Razor/MVC ou Razor Pages), com secao de gaps/inconsistencias e backlog de tarefas futuras.

## Contexto
Precisamos de um manual acessivel ao usuario final (nao tecnico) que explique o fluxo ponta a ponta, baseado nas regras reais do sistema. Alem da documentacao, e obrigatorio identificar lacunas/inconsistencias e propor plano de implementacao, sem alterar o produto automaticamente.

## Objetivo
- Documentar o WMS (inbound/outbound) com base no codigo existente.
- Gerar Mapa de Regras (Markdown) com referencias exatas do codigo.
- Criar secao de Fluxos Ausentes/Inconsistentes com proposta tecnica e backlog de tasks.
- Implementar o manual dentro do sistema (Views + rota + menu).

## Escopo
- Varredura completa do codigo (controllers/pages, domain, handlers, validators, policies, enums, logs, integracoes).
- Manual em PT-BR, linguagem simples e procedural.
- Views com Bootstrap, anchors, componentes (accordions, badges, tables, alerts, cards).
- Pastas/arquivos do manual: /Views/Help/Manual.cshtml + partials.
- Arquivos Markdown para devs:
  - /Tasks/ManualWMS_MapaDeRegras.md
  - /Tasks/ManualWMS_Backlog.md

## Fora do escopo
- Implementar automaticamente fluxos ausentes.
- Alterar regras existentes sem validacao.

## Entregaveis
1) Manual em Views Razor (Help/Manual + partials) com indice e secao executiva.
2) Mapa de Regras (Markdown) com referencias precisas do codigo.
3) Secao "Fluxos Ausentes / Inconsistentes e Plano de Implementacao" (Views + resumo no backlog).
4) Backlog de tasks (WMS-TASK-0001...)
5) Rota /help/manual e link no menu (se houver).

## Requisitos obrigatorios (conteudo do manual)
- Visao geral do processo (inbound/outbound).
- Glossario WMS.
- Papeis e permissoes (roles e policies reais).
- Fluxos principais com passo a passo.
- Casos de uso e cenarios (N cenarios cobrindo variacoes reais do codigo).
- Estados e transicoes (tabelas origem -> destino -> condicao).
- Regras criticas (validacoes, bloqueios, excecoes, reprocessos).
- Tratamento de erros e mensagens (exemplos reais do codigo).
- Checklist operacional (antes/depois).
- FAQ + Troubleshooting.
- Secao "Como manter este manual" (para devs).

## Requisitos obrigatorios (gaps/inconsistencias)
Para cada gap:
- Diagnostico
- Impacto operacional
- Padrao de mercado
- Proposta no contexto do projeto (CQRS, handlers, services, validators, entities, migrations, tests)
- Criterios de aceite (DoD)
- Dependencias e riscos
- Estimativa (P/M/G) e prioridade
- Tasks detalhadas (WMS-TASK-0001...)

## Processo de execucao (passos)
1) Identificar arquitetura (MVC/Razor Pages, CQRS/MediatR, pastas).
2) Varredura do codigo por modulos inbound/outbound, entidades, enums, validators, policies, mensagens.
3) Consolidar dimensoes de variacao e cenarios reais.
4) Identificar gaps/inconsistencias vs codigo/padrao WMS.
5) Gerar secao de gaps + backlog de tasks.
6) Criar Views do manual com indice por ancoras.
7) Adicionar rota /help/manual e link no menu.
8) Gerar Mapa de Regras + Backlog em Markdown.

## Solucao tecnica sugerida (alto nivel)
- Criar area Help:
  - /Views/Help/Manual.cshtml
  - /Views/Help/Partials/_Resumo.cshtml
  - /Views/Help/Partials/_Inbound.cshtml
  - /Views/Help/Partials/_Outbound.cshtml
  - /Views/Help/Partials/_Status.cshtml
  - /Views/Help/Partials/_Regras.cshtml
  - /Views/Help/Partials/_Gaps.cshtml
  - /Views/Help/Partials/_TasksBacklog.cshtml
  - /Views/Help/Partials/_Faq.cshtml
- Controller/rota:
  - /Controllers/HelpController.cs -> /help/manual
- Menu: adicionar link (se layout tiver menu).
- Markdown:
  - /Tasks/ManualWMS_MapaDeRegras.md
  - /Tasks/ManualWMS_Backlog.md

## Criterios de aceite
- Manual acessivel em /help/manual.
- Manual cobre inbound/outbound conforme regras reais do codigo.
- Todas as regras citadas tem referencia exata no codigo.
- Gaps/inconsistencias documentados com plano de implementacao.
- Backlog com WMS-TASK-0001... gerado.
- Mapa de Regras e Backlog em Markdown criados.

## How to test
- UI: abrir /help/manual e navegar pelas ancoras/accordions.
- UI: verificar links de telas reais (ou placeholders quando nao existir).
- API/Swagger: nao aplicavel (manual e view).

## Status
PENDENTE