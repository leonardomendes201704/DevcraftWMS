# TASK-0111 - Packing: modelo + API (volumes, peso, etiqueta)

## Resumo
Registrar packing com volumes, pesos e etiquetas.

## Objetivo
Gerar volumes e preparar expedicao.

## Escopo
- Entidade OutboundPackage/Volume.
- Endpoints /api/outbound-orders/{id}/pack.
- Etiquetas e pesos/dimensoes.

## Dependencias
- TASK-0109

## Estimativa
- 6h

## Entregaveis
- API de packing + migration.

## Criterios de Aceite
- Packing gera volumes e atualiza status.

## Status
DONE

## Progresso
- Modelo de packing (OutboundPackage/OutboundPackageItem) com configuracoes EF e migration.
- Endpoint /api/outbound-orders/{id}/pack com validacoes de quantidade por item e status.
- Testes unitarios e de integracao adicionados.

## How to test
```bash
dotnet build DevcraftWMS/DevcraftWMS.sln
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj
```

### UI (DemoMvc)
1) Criar e liberar uma outbound order no DemoMvc.
2) Conferir a OS em **Outbound Checks** para status **Checked**.
3) Voltar em **Outbound Orders** e confirmar status **Packed** apos o packing via API.

### Swagger (API)
1) Abrir Swagger do `DevcraftWMS.Api`.
2) Chamar `POST /api/outbound-orders/{id}/pack` com `packages` e itens.
3) Validar resposta com volumes criados e status da OS em **Packed**.
