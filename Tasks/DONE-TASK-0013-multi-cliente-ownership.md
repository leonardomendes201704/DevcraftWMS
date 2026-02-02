# TASK-0013 - Ownership Multi-Cliente (Customer como Owner)

## Resumo
Introduzir CustomerId como dono dos registros para suportar WMS multi-cliente.

## Contexto
Armazem pode operar para varios clientes, exigindo segregacao de dados.

## Problema
Hoje os cadastros nao possuem Owner, gerando conflito de SKUs e estoque.

## Objetivos
- - Adicionar CustomerId como owner nas entidades principais.
- - Garantir unicidade por cliente.
- - Padronizar filtro por cliente em queries.

## Nao Objetivos
- - Nao implementar billing por cliente.
- - Nao implementar RBAC completo nesta fase.

## Stakeholders
- Operacoes
- TI / Arquitetura
- Comercial

## Premissas
- Customers ja existem no sistema.
- Usuario autenticado pode selecionar cliente ativo.

## User Stories / Casos de Uso
- Como operador, quero cadastrar produtos por cliente.
- Como gestor, quero ver estoque segregado por cliente.

## Requisitos Funcionais
- - Adicionar CustomerId em Product, Lot, InventoryBalance, InventoryMovement, Receipt, Picking.
- - Unicidade de SKU/EAN por (CustomerId, Code/Ean).
- - Endpoints exigem CustomerId (header/route).
- - Listagens filtram por CustomerId.
- - Entidades fisicas (setor, secao, estrutura, corredor, endereco) sao compartilhadas, com visibilidade controlada por mapeamentos *Customers.

## Requisitos Nao Funcionais
- - Indices por CustomerId para performance.
- - Auditoria com CustomerId nos logs.

## Mudancas de API
- - Atualizar endpoints existentes para aceitar CustomerId.
- - Padronizar header X-Customer-Id (ou route /customers/{id}/...).

## Modelo de Dados / Migracoes
- - Migracao AddCustomerOwnership.
- - Migracao AddPhysicalVisibilityMappings (tabelas de acesso por cliente).

## Observabilidade / Logging
- TransactionLogs para CRUD e mudancas de contexto.

## Plano de Testes
- - Unit: validacao de CustomerId obrigatorio.
- - Integration: dados de um cliente nao aparecem para outro.

## Rollout / Deploy
- - Aplicar migration AddCustomerOwnership.

## Riscos / Questoes em Aberto
- - Definir como selecionar cliente ativo no frontend.

## Status
Concluida (DONE)

## Progresso / Notas
- Produtos e UoM seguem com CustomerId direto; armazem permanece compartilhado.
- Entidades fisicas (setores, secoes, estruturas, corredores, enderecos) migraram para mapeamentos de visibilidade (*Customers) com FK para Customer.
- Repositorios filtram por CustomerAccesses (CustomerId do contexto).
- Indices por cliente aplicados onde necessario (SKU/EAN/ERP).
- Migrations AddCustomerOwnership e AddPhysicalVisibilityMappings aplicadas no MainDb.
- Testes atualizados com isolamento por cliente.
