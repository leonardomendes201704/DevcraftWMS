# TASK-0005 - Cadastro de Enderecos (Locations/Bins) do Armazem

## Resumo
Criar o CRUD E2E de Enderecos para representar posicoes fisicas dentro das Estruturas.

## Contexto
Enderecos sao a base para armazenagem e movimentacao no WMS.

## Problema
Nao existe cadastro de enderecos e regras de enderecamento.

## Objetivos
- Implementar CRUD de Enderecos.
- Relacionamento 1:N com Estrutura.

## Nao Objetivos
- Nao implementar algoritmo de slotting.

## Stakeholders
- Operacoes
- TI / Arquitetura

## Premissas
- Estrutura deve existir para criar Endereco.

## User Stories / Casos de Uso
- Como operador, quero cadastrar enderecos por rack.
- Como gestor, quero filtrar enderecos por status.

## Requisitos Funcionais
- Entidade Endereco: Code, Barcode, Level, Row, Column, StructureId, IsActive.
- CRUD completo.
- Filtros: structureId, code, barcode, isActive.
- Ordenacao por qualquer campo.

## Requisitos Nao Funcionais
- Performance e paginacao.

## Criterios de Aceitacao
- API + DemoMvc completos.
- Migrations criadas/aplicadas.
- Testes unitarios e integracao.

## Mudancas de API
- /api/structures/{structureId}/locations (POST)
- /api/structures/{structureId}/locations (GET paginado)
- /api/locations/{id} (GET/PUT/DELETE)

## Modelo de Dados / Migracoes
- Tabela Locations (FK -> Structures).

## Observabilidade / Logging
- TransactionLogs para CRUD.

## Plano de Testes
- CRUD completo, filtros e ordenacao.

## Rollout / Deploy
- Aplicar migration.

## Riscos / Questões em Aberto
- Definir regra de codigo de endereco (padrao).
