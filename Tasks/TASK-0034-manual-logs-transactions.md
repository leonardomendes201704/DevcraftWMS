# TASK-0034 - Manual da tela Logs - Transactions

## Resumo
Criar o manual HTML (Help) para a tela **Logs / Transactions** no DemoMvc, com botao "Help" no Index e conteudo que explique a tela e seus fluxos relacionados (CRUDs e telas filhas).

## Contexto
Precisamos de um manual por tela para orientar usuarios finais. O Help deve estar disponivel no Index e cobrir tambem as telas filhas (Create/Edit/Details/Delete) dentro do contexto do modulo.

## Problema
Hoje as telas nao possuem documentacao contextual embutida.

## Objetivos
- Adicionar botao **Help** no Index da tela Logs / Transactions.
- Renderizar um HTML de manual (parcial/arquivo dedicado) com instrucoes claras.
- Incluir instrucoes para telas filhas (Create/Edit/Details/Delete) quando existirem.
- Garantir consistencia visual com Bootstrap e componentes existentes.

## Nao objetivos
- Nao criar documentacao externa (PDF, wiki).
- Nao adicionar logica de negocio na view.

## Stakeholders
- Operacoes
- Suporte / Treinamento
- TI / Arquitetura

## Premissas
- O Help sera um HTML versionado no repo.
- O conteudo deve ser escrito em linguagem simples e objetiva.

## User Stories / Casos de Uso
- Como usuario, quero entender rapidamente o que posso fazer na tela.
- Como suporte, quero apontar um Help padronizado para cada modulo.

## Requisitos Funcionais
- Botao **Help** no Index da tela Logs / Transactions.
- Help deve abrir/mostrar um HTML com:
  - Objetivo da tela.
  - Como filtrar/paginar/ordenar.
  - Como criar/editar/detalhar/desativar.
  - Alertas/erros comuns e como resolver.
- O manual deve cobrir telas filhas do modulo.

## Requisitos Nao Funcionais
- Conteudo em PT-BR.
- Layout Bootstrap consistente.
- Sem chamadas de API a partir do Help.

## Mudancas de UI
- Adicionar botao Help no header do Index.
- Criar parcial/arquivo de manual da tela.

## Mudancas de API
- N/A.

## Observabilidade / Logging
- N/A.

## Plano de Testes
- Verificar que o botao Help aparece no Index.
- Verificar que o conteudo HTML renderiza corretamente.
- Verificar que o Help cobre as telas filhas.

## Rollout / Deploy
- Deploy normal do DemoMvc.

## Riscos / Questoes em Aberto
- Padronizar estrutura do HTML de manual para todas as telas.

## Status
Aberta (TODO)
