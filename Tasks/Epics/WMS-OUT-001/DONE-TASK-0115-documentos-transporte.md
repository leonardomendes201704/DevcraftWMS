# TASK-0115 - Documentos de transporte (romaneio/relatorio)

## Resumo
Gerar documentos de saida para o cliente e transporte.

## Objetivo
Emitir romaneio e relatorio final de expedicao.

## Escopo
- Relatorio de OS (previsto x expedido).
- Download CSV/PDF.

## Dependencias
- TASK-0113

## Estimativa
- 6h

## Entregaveis
- Relatorio e exportacao.

## Criterios de Aceite
- Cliente acessa documentos via portal.

## Status
DONE

## Progresso
- Relatorio de expedicao por OS (previsto x expedido) com resumo e linhas pendentes.
- Exportacao CSV do relatorio via API.
- Portal com lista de reports e detalhe com exportacao.
- Testes unitarios e de integracao cobrindo relatorio e exportacao.

## How to test
```bash
dotnet build DevcraftWMS/DevcraftWMS.sln
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj
```

### UI (Portal)
1) Criar uma outbound order no Portal e concluir conferencia + packing + expedicao (status **Shipped**).
2) No Portal, abrir **Shipping Reports** e localizar a OS.
3) Abrir o report e validar resumo (Expected vs Shipped) e linhas pendentes.
4) Usar **Export CSV** para baixar o arquivo.

### Swagger (API)
1) Abrir Swagger do `DevcraftWMS.Api`.
2) Chamar `GET /api/outbound-orders/{id}/report` e validar summary/lines.
3) Chamar `GET /api/outbound-orders/{id}/report/export` e validar download CSV.
