# TASK-0122 - Seed outbound check flow (picking + checks)

## Resumo
Adicionar seed no SampleDataSeeder para gerar ordens/picking tasks e checks pendentes/andamento, facilitando testes no mobile.

## Objetivo
- Criar OutboundChecks automaticamente para ordens cujo picking ja esteja completo na seed.
- Permitir validar a fila de conferencia via /api/outbound-checks sem precisar executar todo o fluxo manualmente.

## Escopo
- Atualizar SampleDataSeeder para criar OutboundChecks (Pending/InProgress) quando houver picking tasks completas.
- Nao criar itens de conferencia (OutboundCheckItems) na seed.

## Fora do escopo
- Alterar APIs ou regras de negocio.
- Criar seed de evidencias ou divergencias.

## Dependencias
- TASK-0121

## Entregaveis
- SampleDataSeeder com criacao de OutboundChecks para ordens com picking completo.

## Criterios de aceite
- Com seed habilitado e picking tasks geradas, a fila /api/outbound-checks retorna registros.
- Pelo menos um registro com Status=InProgress para testar a tela mobile.

## How to test
- Unit tests: `dotnet test DevcraftWMS.sln --filter FullyQualifiedName~SampleDataSeeder`
- Integration tests: `dotnet test DevcraftWMS.sln --filter FullyQualifiedName~OutboundCheckQueueEndpointsTests`
- UI (DemoMvc): abrir a fila de conferencia e validar que existem registros seed.
- Swagger/API: `GET /api/outbound-checks?pageNumber=1&pageSize=10&orderBy=CreatedAtUtc&orderDir=desc`.

## Status
PENDING