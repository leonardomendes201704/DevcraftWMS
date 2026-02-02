# TASK-0015 - Feedback para cadastros com dependencias

## Resumo
Refatorar cadastros dependentes para exibir feedback claro quando o cadastro pai nao existe, oferecendo acao de cadastro antes de redirecionar.

## Contexto
Cadastros como Setores, Secoes, Estruturas, Enderecos e Corredores dependem de entidades anteriores. Hoje o fluxo redireciona sem explicacao.

## Problema
O usuario nao entende o motivo do redirecionamento para outro cadastro, gerando confusao.

## Objetivos
- Exibir mensagem clara quando dependencia esta ausente.
- Oferecer acao primaria para cadastrar a dependencia.
- Padronizar a UX em todos os cadastros dependentes.

## Nao Objetivos
- Nao alterar regras de negocio do backend.

## Stakeholders
- Operacoes
- TI / Arquitetura

## Premissas
- As entidades dependentes ja estao implementadas.

## User Stories / Casos de Uso
- Como usuario, quero saber porque fui redirecionado para outro cadastro.
- Como usuario, quero uma opcao direta para cadastrar a dependencia.

## Requisitos Funcionais
- Criar view reutilizavel de aviso de dependencia.
- Aplicar em Setores, Secoes, Estruturas, Enderecos, Corredores (e Produtos quando faltar UoM).

## Requisitos Nao Funcionais
- UX consistente com padrao do DemoMvc.

## Mudancas de API
- Nenhuma.

## Modelo de Dados / Migracoes
- Nenhuma.

## Observabilidade / Logging
- N/A.

## Plano de Testes
- Validar fluxo sem dependencias (mensagem + botao).
- Validar fluxo normal (segue para formulario).

## Rollout / Deploy
- Nenhum passo especial.

## Riscos / Questoes em Aberto
- Definir mensagem e acoes para cada dependencia.

## Status
Concluida (DONE)

## Progresso / Notas
- View reutilizavel DependencyPrompt criada.
- Fluxos de Create ajustados para exibir prompt antes de redirecionar.
- Aplicado em Setores, Secoes, Estruturas, Enderecos, Corredores e Produtos (UoM).

## Checklist de Aceitacao
- [x] Feedback de dependencia exibido.
- [x] Acoes de cadastro e retorno disponiveis.
