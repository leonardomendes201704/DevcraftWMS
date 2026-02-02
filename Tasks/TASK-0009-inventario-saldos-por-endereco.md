# TASK-0009 - Inventario e Saldos por Endereco

## Resumo
Controle de estoque por endereco (quantity, reserved, available, status).

## Contexto
Inventario por endereco e base para operacoes de picking e transferencias.

## Problema
Nao existe tabela de saldos por location.

## Objetivos
- - CRUD/consultas de saldo por endereco.
- - Regras para disponibilidade e reserva.
- - Preparar base para movimentacoes.

## Nao Objetivos
- - Nao implementar custo medio.
- - Nao implementar inventario cego.

## Stakeholders
- Operacoes
- Planejamento
- TI / Arquitetura

## Premissas
- Location e Product devem existir.
- Saldo inicial pode ser zero.

## User Stories / Casos de Uso
- Como operador, quero ver saldo por endereco.
- Como gestor, quero filtrar saldos por produto e status.

## Requisitos Funcionais
- - Entidade InventoryBalance: LocationId, ProductId, LotId?, QuantityOnHand, QuantityReserved, QuantityAvailable, Status.
- - CRUD e consultas.
- - Regras: QuantityAvailable = OnHand - Reserved (derivado).
- - Filtros: locationId, productId, lotId, status, isActive.

## Requisitos Nao Funcionais
- - Paginacao obrigatoria.
- - Indices para consultas por Location/Product.

## Mudancas de API
- - /api/inventory/balances (GET paginado)
- - /api/locations/{locationId}/inventory (POST/GET)
- - /api/inventory/balances/{id} (GET/PUT/DELETE)

## Modelo de Dados / Migracoes
- - Tabela InventoryBalances (FK -> Locations, Products, Lots).

## Observabilidade / Logging
- TransactionLogs para CRUD e mudancas de status.

## Plano de Testes
- - Unit: validacoes de quantidade e vinculos.
- - Integration: CRUD, filtros, consistencia de available.

## Rollout / Deploy
- - Migration AddInventoryBalances.

## Riscos / Questoes em Aberto
- - Definir status de saldo (Available/Blocked/Damaged).

## Status
Aberta (TODO)
