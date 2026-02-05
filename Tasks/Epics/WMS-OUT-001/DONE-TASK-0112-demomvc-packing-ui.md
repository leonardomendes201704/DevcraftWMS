# TASK-0112 - DemoMvc: tela de packing + etiquetas

## Resumo
UI para packing e impressao/preview de etiquetas.

## Objetivo
Permitir embalador registrar volumes e imprimir etiqueta.

## Escopo
- Tela de packing com volumes.
- Botao imprimir/preview etiqueta.

## Dependencias
- TASK-0111

## Estimativa
- 6h

## Entregaveis
- UI DemoMvc para packing.

## Criterios de Aceite
- Packing concluido e etiquetas geradas.

## Status
DONE

## Progresso
- UI DemoMvc criada para packing (fila + detalhes) com pacotes e preview.
- Botao de preview/print de etiqueta por pacote.
- Testes unitarios e de integracao atualizados.

## How to test
```bash
dotnet build DevcraftWMS/DevcraftWMS.sln
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj
```

### UI (DemoMvc)
1) Criar e liberar uma outbound order.
2) Conferir a OS em **Outbound Checks** para status **Checked**.
3) Abrir **Outbound Packing**, selecionar a OS e preencher pacotes/quantidades.
4) Registrar packing e usar **Preview Label** para visualizar o conteudo do pacote.
5) Confirmar status da OS em **Packed**.

### Swagger (API)
1) Abrir Swagger do `DevcraftWMS.Api`.
2) Chamar `POST /api/outbound-orders/{id}/pack` com `packages` e itens.
3) Validar retorno com pacotes criados e status da OS em **Packed**.
