# TASK-0014 - Contexto de Cliente (UI + API)

## Resumo
Adicionar selecao de cliente ativo no front e propagacao no API.

## Contexto
Para multi-cliente, o sistema precisa de um contexto selecionado.

## Problema
Nao existe mecanismo de selecao de cliente ativo na UI.

## Objetivos
- - Adicionar seletor de cliente no topo do DemoMvc.
- - Propagar CustomerId em todas as chamadas API.
- - Exibir dados apenas do cliente selecionado.

## Nao Objetivos
- - Nao implementar troca automatica por perfil.

## Stakeholders
- Operacoes
- TI / Arquitetura

## Premissas
- Clientes estao cadastrados e acessiveis.

## User Stories / Casos de Uso
- Como usuario, quero escolher o cliente ativo para trabalhar.

## Requisitos Funcionais
- - UI: dropdown de clientes (topbar).
- - Persistir selecao em session.
- - ApiClient envia header X-Customer-Id.
- - Backend valida e aplica filtro.

## Requisitos Nao Funcionais
- - UX consistente em todas as telas.

## Mudancas de API
- - Middleware para resolver CustomerId do header/session.

## Modelo de Dados / Migracoes
- - Sem novas tabelas (reuso Customers).

## Observabilidade / Logging
- TransactionLogs para CRUD e mudancas de contexto.

## Plano de Testes
- - Integration: requests sem CustomerId retornam erro.
- - UI: troca de cliente reflete nas listagens.

## Rollout / Deploy
- - Atualizar README/diretrizes.

## Riscos / Questoes em Aberto
- - Definir estrategia quando cliente nao selecionado.

## Status
Aberta (TODO)
