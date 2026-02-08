# TASK-0132 - DemoMvc: Alinhar botoes Help/Filters/New no header

## Summary
Padronizar os botoes de acoes (Help, Filters, New) para ficarem na mesma linha do titulo, alinhados a direita, em todas as telas de index.

## Context
Atualmente os botoes aparecem em linhas separadas e desalinhadas, criando inconsistencias visuais.

## Objective
- Agrupar Help, Filters e New no header da grid.
- Manter alinhamento a direita na linha do titulo.

## Scope
- DemoMvc _Grid, _FilterDrawer, JS e CSS de suporte.

## Out of Scope
- Mudancas de layout fora das telas index.

## Acceptance Criteria
- Help, Filters e New aparecem na mesma linha do titulo.
- Alinhamento consistente em todas as telas index.

## How to Test
- UI: abrir /Products, /Customers, /Lots e verificar alinhamento dos botoes.

## Status
DONE

## Implementation Notes
- Centralized actions in grid header and moved Help/Filters via JS.
