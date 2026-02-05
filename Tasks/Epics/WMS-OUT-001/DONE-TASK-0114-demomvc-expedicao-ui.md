# TASK-0114 - DemoMvc: expedicao (carregar, finalizar)

## Resumo
Tela de expedicao para carregar e finalizar OS.

## Objetivo
Permitir operador concluir carregamento.

## Escopo
- UI de doca/carregamento.
- Validacao de volumes.

## Dependencias
- TASK-0113

## Estimativa
- 6h

## Entregaveis
- UI DemoMvc para expedicao.

## Criterios de Aceite
- OS passa para Expedido/Parcial.

## Status
DONE

## Progresso
- UI DemoMvc para fila de expedicao com filtros e acao de embarque.
- Tela de detalhes para doca, horarios e selecao de volumes.
- Integracao com API de pacotes e embarque, com feedback de sucesso.
- Testes unitarios e de integracao atualizados.

## How to test
```bash
dotnet build DevcraftWMS/DevcraftWMS.sln
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj
```

### UI (DemoMvc)
1) Criar uma outbound order e liberar (status **Released**).
2) Realizar conferencia e packing para status **Packed**.
3) Abrir **Outbound Shipping** no DemoMvc e entrar nos detalhes da OS.
4) Informar doca, horarios e selecionar os pacotes a embarcar.
5) Confirmar que a OS fica **Shipped** ou **Partially shipped**.

### Swagger (API)
1) Abrir Swagger do `DevcraftWMS.Api`.
2) Chamar `GET /api/outbound-orders/{id}/packages` e validar os pacotes do pedido.
3) Chamar `POST /api/outbound-orders/{id}/ship` com `dockCode` e `packages`.
4) Validar retorno com itens embarcados e status atualizado.
