# TASK-0201 - UI Backoffice: Consulta + filtros + drill-down

## Summary
Criar tela Backoffice "Consulta > Estoque por Cliente" com filtros, agrupamento por produto e drill-down por endereco.

## Contexto
Backoffice precisa visualizar rapidamente distribuicao fisica e disponibilidade. Esta task entrega a UI principal com base na API do TASK-0200.

## Objetivo
- Tela unica com filtros e resultados em 3 niveis.
- Drill-down por produto -> localizacoes -> detalhes.

## Escopo
- Nova area de menu (Backoffice).
- Filtros: cliente, armazem, SKU, lote, validade, status, endereco, estrutura, area (staging/doca/picking), tipo de unidade.
- Resultados: resumo por produto + distribuicao por localizacao.

## UI/UX
- Tabs ou accordion para alternar: Resumo, Distribuicao, Rastreio.
- Chips de status: Disponivel, Reservado, Bloqueado, Em Processo.
- Alertas visuais (validade proxima, quarentena).

## API Integration
- GET /api/inventory-visibility (summary + locations).

## Acceptance Criteria
- Usuario aplica filtros e visualiza resumo por SKU.
- Drill-down mostra enderecos e unidade logistica.

## How to Test
- UI: testar filtros e drill-down.
- Swagger: validar payloads.

## Status
PENDING
