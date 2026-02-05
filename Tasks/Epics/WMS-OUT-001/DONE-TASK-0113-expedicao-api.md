# TASK-0113 - Expedicao: doca/carregamento + API

## Resumo
API para carregar volumes e finalizar expedicao.

## Objetivo
Registrar embarque e documentos de saida.

## Escopo
- Endpoints /api/outbound-orders/{id}/ship.
- Registro de doca, horarios e volumes.
- Status para Expedido/Parcial.

## Dependencias
- TASK-0111

## Estimativa
- 6h

## Entregaveis
- API de expedicao + migration.

## Criterios de Aceite
- Expedicao valida volumes embarcados.

## Status
DONE

## Progresso
- Modelo de expedicao com OutboundShipment/OutboundShipmentItem + relacionamentos.
- Endpoint `POST /api/outbound-orders/{id}/ship` para registrar doca, horarios e volumes.
- Regras de negocio para validar status e volumes embarcados.
- Migration aplicada para novas tabelas de expedicao.
- Testes unitarios e de integracao cobrindo envio e status.

## How to test
```bash
dotnet build DevcraftWMS/DevcraftWMS.sln
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj
```

### UI (DemoMvc)
1) Criar uma outbound order no Portal/DemoMvc e liberar a OS (status **Released**).
2) Realizar conferencia e packing nas telas existentes para chegar ao status **Packed**.
3) Chamar o endpoint de expedicao via Swagger (passo abaixo).
4) Voltar para a tela de Outbound Orders no DemoMvc e validar a OS como **Shipped** ou **Partially shipped**.

### Swagger (API)
1) Abrir Swagger do `DevcraftWMS.Api`.
2) Chamar `POST /api/outbound-orders/{id}/ship` informando `dockCode`, horarios e `packages` (IDs de pacotes).
3) Validar retorno com itens de expedicao e status atualizado para **Shipped** ou **Partially shipped**.
