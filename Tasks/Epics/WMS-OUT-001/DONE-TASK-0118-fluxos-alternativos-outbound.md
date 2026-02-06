# TASK-0118 - Fluxos alternativos (parcial, cancelamento, cross-dock)

## Resumo
Implementar fluxos alternativos do outbound com cancelamento e controle de reservas.

## Objetivo
Tratar cenarios de excecao com auditoria e liberar reservas conforme a expedição.

## Escopo
- Cancelamento de OS apos separacao com devolucao de reserva.
- Expedicao parcial com liberacao proporcional das reservas.
- Suporte a cross-dock na reserva (zona cross-dock).

## Dependencias
- TASK-0106

## Estimativa
- 6h

## Entregaveis
- Endpoint de cancelamento de OS.
- Reservas persistidas por OS e liberadas em cancelamento/expedicao.
- Suporte a filtro de reserva por zona cross-dock.

## Criterios de Aceite
- Cancelamento libera reserva e cancela tarefas de picking.
- Expedicao parcial libera somente o reservado expedido.
- Cross-dock usa saldos da zona cross-dock quando aplicavel.

## Status
DONE

## Progresso
- Adicionado modelo de reservas de outbound por OS e balance (OutboundOrderReservations).
- Release cria reservas persistidas e cross-dock filtra balances por zona.
- Cancelamento libera reservas e marca picking tasks como canceladas.
- Expedicao libera reservas conforme pacotes embarcados.
- Novos testes unitarios e integracao cobrindo cancelamento e liberacao de reservas.

## How to test
```bash
dotnet build DevcraftWMS/DevcraftWMS.sln
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj
```

### UI (DemoMvc/Portal)
1) Criar uma OS no Portal e liberar pelo Backoffice (status **Released**).
2) Atualizar a lista da OS e confirmar que o status muda para **Canceled** apos o cancelamento via API.
3) Verificar no Backoffice que as tarefas de picking associadas ficam com status **Canceled**.

### Swagger (API)
1) Abrir Swagger do `DevcraftWMS.Api`.
2) Criar OS em `POST /api/outbound-orders` e liberar em `POST /api/outbound-orders/{id}/release`.
3) Cancelar em `POST /api/outbound-orders/{id}/cancel` com `reason`.
4) Expedir parcialmente em `POST /api/outbound-orders/{id}/ship` com subset de pacotes.
5) Validar status **Canceled** ou **Partially shipped** e reservas liberadas no banco.
