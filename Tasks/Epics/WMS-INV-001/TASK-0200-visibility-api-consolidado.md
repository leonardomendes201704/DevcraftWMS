# TASK-0200 - Visibilidade Total: API base de consulta e visao consolidada

## Summary
Criar API de consulta consolidada de estoque por Cliente + Armazem, com agrupamentos por produto e distribuicao fisica por endereco, incluindo lotes/validade e unidade logistica.

## Contexto
Backoffice precisa responder rapidamente onde estao os produtos do Cliente X no Armazem Y. Hoje os dados estao espalhados em InventoryBalances, movimentos, ordens e recebimentos. Esta task cria o backbone de consulta.

## Objetivo
- Unificar dados de saldo, endereco e rastreio.
- Permitir filtros basicos (cliente, armazem, SKU, lote, validade).
- Retornar resultados nos 3 niveis: consolidado, distribuicao, rastreabilidade (stub neste passo).

## Escopo
- Query API: consulta consolidada por cliente+armazem.
- DTOs de resposta para 3 niveis (consolidado, distribuicao fisica, rastreio resumido).
- Paginacao e ordenacao padrao.

## Fora de escopo
- UI completa.
- Exportacoes.
- Alertas de inconsistencias.

## Requisitos funcionais
- Filtro por CustomerId e WarehouseId (obrigatorios).
- Suporte a filtro por ProductId/SKU, LotCode, ExpirationDate.
- Agrupamento por produto com totais (OnHand, Reserved, Blocked, InProcess).
- Distribuicao por endereco (estrutura + localizacao + unidade logistica).

## Regras de negocio
- Disponibilidade calculada como: OnHand - Reserved - Blocked - InProcess.
- Lotes/validade obrigatorios quando produto possui rastreio (TrackingMode != None).
- Excluir IsActive=false por padrao.

## API (proposta)
- GET /api/inventory-visibility
  - Query: customerId, warehouseId, productId?, sku?, lotCode?, expirationFrom?, expirationTo?, includeInactive?, pageNumber, pageSize, orderBy, orderDir.

## Modelos de resposta (exemplo)
```json
{
  "summary": [
    {
      "productId": "...",
      "productCode": "SKU-001",
      "productName": "Produto A",
      "uomCode": "EA",
      "quantityOnHand": 1200,
      "quantityReserved": 300,
      "quantityBlocked": 100,
      "quantityInProcess": 0,
      "quantityAvailable": 800
    }
  ],
  "locations": [
    {
      "locationId": "...",
      "locationCode": "A-01-01",
      "structureCode": "PR-01",
      "sectorCode": "S01",
      "productId": "...",
      "lotCode": "L-2026-01",
      "expirationDate": "2026-03-15",
      "unitLoadCode": "PLT-0001",
      "quantityOnHand": 200,
      "status": "Available"
    }
  ],
  "trace": []
}
```

## Onde mexer (sugestao tecnica)
- Application/Features/InventoryVisibility/Queries/GetInventoryVisibility
- Application/Services/InventoryVisibilityService
- Infrastructure: repositorios de InventoryBalance, Locations, Products, Lots, UnitLoads
- API Controller: InventoryVisibilityController
- Tests: unit (service), integration (endpoint)

## Acceptance Criteria
- Dado customer+warehouse validos, quando consultar, entao retorna consolidado e distribuicao.
- Itens inativos nao aparecem por padrao.
- Disponivel calculado corretamente.

## How to Test
- Unit: validar calculo de disponibilidade.
- Integration: chamar GET /api/inventory-visibility com filtros.
- Swagger: verificar resposta com summary + locations.

## Status
PENDING
