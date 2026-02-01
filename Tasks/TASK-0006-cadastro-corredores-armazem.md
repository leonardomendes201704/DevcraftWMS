# TASK-0006 - Cadastro de Corredores (Aisles)

## Resumo
Criar o CRUD E2E de Corredores para organizar o fluxo fisico dentro das Secoes.

## Contexto
Corredores representam o layout de circulacao e ajudam a mapear enderecos.

## Problema
Nao existe representacao de corredores no modelo atual.

## Objetivos
- Implementar CRUD de Corredores.
- Relacionamento 1:N com Secao.

## Nao Objetivos
- Nao integrar com mapas ou dispositivos.

## Stakeholders
- Operacoes
- TI / Arquitetura

## Premissas
- Secao deve existir para criar Corredor.

## User Stories / Casos de Uso
- Como gestor, quero cadastrar corredores por secao.
- Como operador, quero listar corredores ativos.

## Requisitos Funcionais
- Entidade Corredor: Code, Name, SectionId, IsActive.
- CRUD completo.
- Filtros: sectionId, code, name, isActive.
- Ordenacao por qualquer campo.

## Requisitos Nao Funcionais
- Observabilidade e logs padrao.

## Criterios de Aceitacao
- API + DemoMvc completos.
- Migrations criadas/aplicadas.
- Testes unitarios e integracao.

## Mudancas de API
- /api/sections/{sectionId}/aisles (POST)
- /api/sections/{sectionId}/aisles (GET paginado)
- /api/aisles/{id} (GET/PUT/DELETE)

## Modelo de Dados / Migracoes
- Tabela Aisles (FK -> Sections).

## Observabilidade / Logging
- TransactionLogs para CRUD.

## Plano de Testes
- CRUD completo, filtros e ordenacao.

## Rollout / Deploy
- Aplicar migration.

## Riscos / Questões em Aberto
- Definir nomenclatura padrao dos corredores.
