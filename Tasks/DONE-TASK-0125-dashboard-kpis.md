# TASK-0125 - Dashboard KPIs reorganizados por contexto (DemoMvc)

## Summary
Reorganizar os KPIs do topo do Dashboard do DemoMvc por contexto (System & Inventory, Inbound, Outbound, Operations).

## Objetivo
- Melhorar leitura e tomada de decisao do Dashboard.
- Agrupar KPIs por contexto sem alterar as regras atuais.

## Escopo
- Ajustar layout e agrupamento dos KPIs no Dashboard.
- Manter dados e calculos existentes.

## Fora de Escopo
- Alterar calculos ou regras de KPI.
- Criar novos endpoints ou alterar APIs.

## Requisitos de UI/UX
- Cada contexto com titulo e cards agrupados.
- Priorizar legibilidade (sem excesso de cards por linha).
- Responsivo (desktop e mobile).

## Areas e arquivos
- `src/DevcraftWMS.DemoMvc/Views/Home/Index.cshtml`

## How to Test
- UI: carregar `/` e validar distribuicao dos KPIs por contexto.
- UI: validar responsividade mobile.

## Status
DONE
