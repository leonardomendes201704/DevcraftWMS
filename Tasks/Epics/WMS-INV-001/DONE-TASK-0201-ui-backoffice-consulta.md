# TASK-0201 - UI Backoffice: Consulta + filtros + drill-down

## Summary
Criar a tela Backoffice "Inventory Visibility" para consulta consolidada de estoque por cliente e armazem, com filtros e visoes em abas (Resumo e Localizacoes).

## Contexto
Backoffice precisa responder rapidamente onde esta o estoque, o que esta reservado/bloqueado e a distribuicao fisica. Esta UI consome a API do TASK-0200.

## Objetivo
- Exibir filtros para cliente (via contexto), armazem, produto, SKU, lote, validade e status.
- Apresentar resumo por produto e distribuicao por localizacao.
- Permitir ordenacao e paginacao consistentes com a API.

## Escopo
- Controller `InventoryVisibilityController` no DemoMvc.
- ApiClient `InventoryVisibilityApiClient` para `GET /api/inventory-visibility`.
- View `Views/InventoryVisibility/Index.cshtml` com abas.
- Link no menu lateral.
- Ajustes no InventoryCounts para suportar debug banner e listar contagens com ItemsCount correto.
- Seed: InventoryCounts sempre ativos e reset hard-delete para evitar soft-delete inativo.

## UI/UX
- Drawer de filtros com campos principais.
- Aba "Summary" (resumo por produto) e "Locations" (distribuicao).
- Badges para status e ativo/inativo.
- Modal de ajuda com dicas de uso.
- Debug banner opcional em `/InventoryCounts?debug=1` com status da chamada API.

## API Integration
- GET /api/inventory-visibility (summary + locations)
- GET /api/inventory-counts (lista de contagens)

## Acceptance Criteria
- Usuario com contexto de cliente consegue filtrar e visualizar resumo e localizacoes.
- Filtros aplicados refletem na paginacao e ordenacao.
- Quando nao ha contexto de cliente, a tela informa o usuario.
- InventoryCounts lista exibe ItemsCount correto e nao zera quando existem itens.
- Seed de InventoryCounts cria registros com IsActive=true.

## How to Test
- UI: selecionar um cliente (selector), abrir "Inventory Visibility", aplicar filtros e alternar abas.
- UI: validar ordenacao clicando nos headers da grid.
- UI: abrir `/InventoryCounts?debug=1` e verificar banner de status.
- Swagger: `GET /api/inventory-visibility` com `X-Customer-Id` e `warehouseId`.
- Swagger: `GET /api/inventory-counts` com `X-Customer-Id` e `includeInactive=false`.

## Status
DONE
