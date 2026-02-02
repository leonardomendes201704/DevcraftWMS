# TASK-0002 - Cadastro de Setores do Armazem (Zones/Sectors)

## Resumo
Criar o CRUD E2E de Setores do Armazem para organizar areas operacionais por finalidade (recebimento, picking, expedicao, etc.).

## Contexto
Com o cadastro de Armazens pronto, precisamos decompor o espaco interno em Setores para suportar processos e regras operacionais.

## Problema
Nao existe estrutura para segmentar o armazem em setores com regras e metas operacionais.

## Objetivos
- Implementar CRUD completo de Setores (API + DemoMvc).
- Garantir relacionamento 1:N com Armazem.
- Permitir filtros, ordenacao e paginacao.

## Nao Objetivos
- Nao implementar regras WMS avancadas (reabastecimento, slotting, etc.).
- Nao integrar com dispositivos externos.

## Stakeholders
- Operacoes
- TI / Arquitetura
- QA

## Premissas
- Padrao Clean Architecture + CQRS + MediatR.
- Todos endpoints de listagem paginados.

## User Stories / Casos de Uso
- Como gestor, quero criar setores para organizar o armazem.
- Como operador, quero consultar setores ativos por armazem.
- Como admin, quero desativar setores sem excluir dados historicos.

## Requisitos Funcionais
- Entidade Setor: Code, Name, Description, SectorType, IsActive, WarehouseId.
- CRUD completo (Create, Update, GetById, ListPaged, Deactivate).
- Filtros: warehouseId, code, name, type, isActive.
- Ordenacao por qualquer campo.
- Excluir inativos por padrao.

## Requisitos Nao Funcionais
- Performance: listagem paginada com filtros.
- Seguranca: seguir regras de auth existentes.
- Observabilidade: logs e correlationId.

## Criterios de Aceitacao
- Endpoints API funcionando com RequestResult.
- Views DemoMvc (Index/Details/Create/Edit/Delete) funcionando.
- Listagem com filtros, ordenacao e paginacao.
- Migrations criadas e aplicadas.
- Testes unitarios e integracao (happy + sad).

## Mudancas de API
- /api/warehouses/{warehouseId}/sectors (POST)
- /api/warehouses/{warehouseId}/sectors (GET paginado)
- /api/sectors/{id} (GET/PUT/DELETE)

## Modelo de Dados / Migracoes
- Tabela Sectors (FK -> Warehouses).

## Observabilidade / Logging
- Registrar criacao/atualizacao em TransactionLogs.

## Plano de Testes
- Unit: validacoes e servicos.
- Integration: CRUD E2E com filtros.

## Rollout / Deploy
- Aplicar migration e atualizar DB.

## Riscos / Questões em Aberto
- Definir lista de tipos de setor (enum) e DisplayName.

## Status
Concluida (DONE)

## Progresso / Notas
- Entidade Setor criada com relacionamento 1:N com Armazem.
- CRUD API implementado com CQRS + MediatR e RequestResult.
- Telas DemoMvc (Index/Details/Create/Edit/Delete) implementadas com Grid + filtros.
- Migrations criadas e aplicadas (AddSectors).
- Testes unitarios e de integracao adicionados.

## Checklist de Aceitacao
- [x] Endpoints API funcionando com RequestResult.
- [x] Views DemoMvc (Index/Details/Create/Edit/Delete) funcionando.
- [x] Listagem com filtros, ordenacao e paginacao.
- [x] Migrations criadas e aplicadas.
- [x] Testes unitarios e integracao (happy + sad).
