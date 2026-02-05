# TASK-0102 - OS: CQRS + endpoints (create/list/get)

## Resumo
Expor endpoints para criar e consultar OS.

## Objetivo
Permitir que o cliente registre OS e o backoffice visualize a fila.

## Escopo
- Commands/Queries para create/list/get.
- Endpoints /api/outbound-orders.
- Validacoes de SKU e unidades.

## Dependencias
- TASK-0101

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints + testes basicos.

## Criterios de Aceite
- OS criada e consultavel com filtros e paginacao.

## Como testar
1) Subir a API: `dotnet run --project src/DevcraftWMS.Api`
2) Criar OS:
   - `POST /api/outbound-orders` com items, warehouseId, orderNumber.
3) Listar:
   - `GET /api/outbound-orders?pageNumber=1&pageSize=20&orderBy=CreatedAtUtc&orderDir=desc`
4) Buscar detalhes:
   - `GET /api/outbound-orders/{id}`
5) Rodar testes:
   - `dotnet test --filter FullyQualifiedName~OutboundOrderCrudTests`

## Status
DONE
