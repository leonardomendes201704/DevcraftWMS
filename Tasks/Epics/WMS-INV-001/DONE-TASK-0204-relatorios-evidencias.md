# TASK-0204 - Relatorios e evidencias (PDF/Excel/print)

## Summary
Gerar evidencias para atendimento ao cliente (PDF/Excel/print-friendly e copiar resumo).

## Objetivo
- Exportar resumo e detalhes.
- Gerar print-friendly com data/hora e usuario.

## Escopo
- Endpoint de exportacao.
- UI com botoes: Exportar PDF, Excel, Print, Copiar resumo.

## API
- GET /api/inventory-visibility/export?format=pdf|xlsx|print

## Tests
- Integration: validar exportacao.

## Status
DONE

## Implementacao
- Endpoint `/api/inventory-visibility/export` com formatos `print`, `pdf` e `xlsx` (CSV).
- HTML print-friendly com resumo + localizacoes, data/hora e usuario.
- CSV com resumo e localizacoes (compatível com Excel).
- PDF retorna HTML print-friendly (usar "Print to PDF" no navegador).
- UI adiciona botoes de PDF/Excel/Print e "Copy summary".
- Copiar resumo usa clipboard do navegador.

## How to Test
- UI: abrir `/InventoryVisibility` e usar botoes Export PDF/Excel/Print/Copy summary.
- Swagger: `GET /api/inventory-visibility/export?format=print`.
- Integration: `InventoryVisibilityEndpointsTests.Export_Inventory_Visibility_Should_Return_Print_View`.
