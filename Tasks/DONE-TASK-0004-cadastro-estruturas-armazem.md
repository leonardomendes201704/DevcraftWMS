# TASK-0004 - Cadastro de Estruturas (Racks) do Armazem

## Resumo
Criar o CRUD E2E de Estruturas para representar racks/estantes dentro das Secoes.

## Contexto
Estruturas fisicas sao essenciais para enderecamento e controle de estoque.

## Problema
Nao existe representacao de estruturas fisicas no modelo.

## Objetivos
- Implementar CRUD completo de Estruturas.
- Relacionamento 1:N com Secao.

## Nao Objetivos
- Nao implementar regras de capacidade detalhadas por produto.

## Stakeholders
- Operacoes
- TI / Arquitetura

## Premissas
- Secao deve existir para criar Estrutura.

## User Stories / Casos de Uso
- Como gestor, quero cadastrar racks por secao.
- Como operador, quero listar racks ativos.

## Requisitos Funcionais
- Entidade Estrutura: Code, Name, StructureType, SectionId, Levels, IsActive.
- CRUD completo.
- Filtros: sectionId, code, name, type, isActive.

## Requisitos Nao Funcionais
- Observabilidade e logs padrao.

## Criterios de Aceitacao
- Endpoints API + DemoMvc completos.
- Migrations criadas/aplicadas.
- Testes unitarios e integracao.

## Mudancas de API
- /api/sections/{sectionId}/structures (POST)
- /api/sections/{sectionId}/structures (GET paginado)
- /api/structures/{id} (GET/PUT/DELETE)

## Modelo de Dados / Migracoes
- Tabela Structures (FK -> Sections).

## Observabilidade / Logging
- TransactionLogs para CRUD.

## Plano de Testes
- CRUD, filtros e ordenacao.

## Rollout / Deploy
- Aplicar migration.

## Riscos / Questões em Aberto
- Definir enum StructureType com DisplayName.

## Status
Concluida (DONE)

## Progresso / Notas
- Entidade Estrutura criada com relacionamento 1:N com Secao.
- CRUD API implementado com CQRS + MediatR e RequestResult.
- Telas DemoMvc (Index/Details/Create/Edit/Delete) implementadas com Grid + filtros.
- Migrations criadas e aplicadas (AddStructures).
- Testes unitarios e de integracao adicionados.

## Checklist de Aceitacao
- [x] Endpoints API + DemoMvc completos.
- [x] Migrations criadas/aplicadas.
- [x] Testes unitarios e integracao.
