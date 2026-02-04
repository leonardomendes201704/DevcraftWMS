# TASK-0074 - Quarentena: bloqueio de estoque

## Resumo
Bloquear estoque em quarentena para movimentacao/picking.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Regras no estoque
- Validacoes em movimentacoes
- Mensagens claras

## Dependencias
- TASK-0071,TASK-0073

## Estimativa
- 5h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Movimentacao bloqueada
- Picking ignora quarentena
- Logs registrados

## How to test
1) Garanta um lote em quarentena (ex: produto com shelf-life minimo e recebimento que dispare quarentena).
2) No DemoMvc: Inventory Movements -> Create, selecione o lote em quarentena e tente movimentar.
   - Esperado: erro "Quarantined inventory cannot be moved."
3) Opcional via API: POST /api/inventory-movements com lotId em quarentena.
   - Esperado: 400 com error code `inventory.movement.quarantine_blocked`.
4) Verifique Inventory Balances: saldo do lote com Status=Blocked deve exibir Available=0.

## Status
DONE
