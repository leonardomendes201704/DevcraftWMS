# TASK-0080 - OE: relatorio final de recebimento

## Resumo
Relatorio final de recebimento (previsto x recebido).

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Relatorio por OE
- Exportacao basica
- Visivel para cliente/backoffice

## Dependencias
- TASK-0079

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Relatorio disponivel
- Dados de divergencia incluidos
- Exportacao funcional

## Status
CONCLUIDO

## Como testar
1. Inicie a API e o Portal/DemoMvc.
2. No Portal, abra uma OE (Inbound Order) e clique em "Receipt Report".
3. Verifique o resumo (expected/received/variance) e as linhas do relatorio.
4. Clique em "Export CSV" e valide o download do arquivo.
5. No Backoffice (DemoMvc), abra "Inbound Orders" e clique em "Report" para a mesma OE.
