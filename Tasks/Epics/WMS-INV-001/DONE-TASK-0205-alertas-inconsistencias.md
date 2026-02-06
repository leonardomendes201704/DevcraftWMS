# TASK-0205 - Alertas operacionais e inconsistencias

## Summary
Detectar alertas operacionais (validade proxima, bloqueios/qualidade, fragmentacao e restricoes de endereco) e exibir na consulta de Inventory Visibility.

## Contexto
A tela de visibilidade precisa sinalizar riscos e inconsistencias para Backoffice e Supervisao sem exigir analise manual de varias telas. Os alertas devem ser calculados a partir de dados reais do estoque e exibidos em nivel de produto (summary) e localizacao.

## Objetivo
- Calcular e exibir alertas por produto e por localizacao.
- Permitir ajuste de thresholds via appsettings.
- Incluir alertas em exportacoes (CSV/print).

## Escopo
- Regras de alerta e severidade.
- Indicadores no Summary e no Locations.
- Exportacao com coluna de alertas.

## Fora de escopo
- Regras novas de negocio nao existentes no dominio.
- Workflow de correcao de inconsistencias.

## Regras implementadas
- FEFO/validade proxima: lotes com validade <= InventoryVisibility:Alerts:ExpirationAlertDays geram alerta.
- Bloqueio/qualidade: status Blocked/Damaged ou QualityInspection pendente geram alerta.
- Restricao de endereco:
  - Location.AllowLotTracking = false com lote -> alerta.
  - Location.AllowExpiryTracking = false com validade -> alerta.
- Fragmentacao: produto em mais de InventoryVisibility:Alerts:FragmentationLocationThreshold localizacoes gera alerta no Summary.

## Severidade
- critical: bloqueio de saldo por status.
- warning: validade proxima, quality inspection, restricao de endereco, fragmentacao.

## Alteracoes tecnicas
- DTOs de Inventory Visibility agora retornam lista de alertas.
- Service calcula alertas com base nas entidades de estoque/lot/inspecao.
- Opcao de configuracao: InventoryVisibility:Alerts.
- UI: coluna Alerts no Summary e Locations (badges).
- Exportacao: coluna Alerts no CSV/HTML.

## Appsettings
- InventoryVisibility:Alerts:ExpirationAlertDays (default 15)
- InventoryVisibility:Alerts:FragmentationLocationThreshold (default 5)

## Tests
- Unit: InventoryVisibilityServiceTests cobre alertas de validade e fragmentacao.

## How to Test
- UI:
  1) Acesse /InventoryVisibility.
  2) Filtre por um produto com lote vencendo nos proximos dias e valide o badge "expiry".
  3) Verifique alertas no Summary e Locations.
  4) Exporte CSV/Print e confirme a coluna Alerts.
- Swagger/API:
  1) GET /api/inventory-visibility?customerId=...&warehouseId=...
  2) Verifique o campo alerts em summary/items e locations/items.
  3) GET /api/inventory-visibility/export?format=xlsx|print e confirme coluna Alerts.

## Status
DONE
