# TASK-0110 - DemoMvc: conferencia + divergencias

## Resumo
Tela de conferencia para validar itens separados.

## Objetivo
Registrar divergencias e aprovar conferencia.

## Escopo
- UI com grid de itens e ocorrencias.
- Upload de evidencia quando exigido.

## Dependencias
- TASK-0109

## Estimativa
- 6h

## Entregaveis
- UI DemoMvc para conferencia.

## Criterios de Aceite
- Conferencia concluida e status atualizado.

## Status
DONE

## Progresso
- UI DemoMvc criada para conferencia (fila + detalhes).
- Registro de divergencias com upload de evidencia por item.
- Testes unitarios e de integracao atualizados.

## How to test
```bash
dotnet build DevcraftWMS/DevcraftWMS.sln
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj
```

### UI (DemoMvc)
1) Run `DevcraftWMS` and open DemoMvc.
2) Go to **Outbound Checks**.
3) Open a picking order, adjust checked quantities, add divergence reason, upload evidence, and submit.
4) Confirm success message and order status updates to **Checked**.

### Swagger (API)
1) Open Swagger for `DevcraftWMS.Api`.
2) Call `POST /api/outbound-orders/{id}/check` with a released/picking order id.
3) Verify response contains items and `evidenceCount`, and order status becomes **Checked**.
