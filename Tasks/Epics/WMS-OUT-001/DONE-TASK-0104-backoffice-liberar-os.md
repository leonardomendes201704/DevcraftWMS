# TASK-0104 - Backoffice: liberar OS + parametros

## Resumo
Backoffice libera OS e define parametros de picking e prioridade.

## Objetivo
Habilitar workflow de liberacao e definicao de metodo de separacao.

## Escopo
- Endpoint /api/outbound-orders/{id}/release.
- Campos: prioridade, metodo picking, janela de expedicao.
- UI DemoMvc para liberar.

## Dependencias
- TASK-0102

## Estimativa
- 6h

## Entregaveis
- Release API + UI Backoffice.

## Criterios de Aceite
- OS muda para Liberada com parametros persistidos.

## Como testar
1) Subir API: `dotnet run --project src/DevcraftWMS.Api`
2) Subir DemoMvc: `dotnet run --project src/DevcraftWMS.DemoMvc`
3) Selecionar cliente no topo do DemoMvc.
4) Acessar **Outbound Orders**.
5) Abrir **Details** de uma OS.
6) Informar prioridade, metodo de separacao e janela de expedicao (inicio/fim).
7) Clicar **Release Order** e validar status = Released.
8) Opcional: testar via API:
   - `POST /api/outbound-orders/{id}/release`
   - Body com `priority`, `pickingMethod`, `shippingWindowStartUtc`, `shippingWindowEndUtc`.

## Status
DONE
