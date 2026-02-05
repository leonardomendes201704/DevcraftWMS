# TASK-0103 - Portal Cliente: UI criar OS

## Resumo
Tela no Portal para criar OS com itens e documentos.

## Objetivo
Permitir que o cliente registre pedidos de saida.

## Escopo
- Tela Create/Details no Portal.
- Anexos opcionais (docs).
- Validacoes amigaveis.

## Dependencias
- TASK-0102

## Estimativa
- 6h

## Entregaveis
- UI Portal para OS (create/list/details).

## Criterios de Aceite
- Cliente cria OS com itens e envia para backoffice.

## Como testar
1) Subir o portal: `dotnet run --project src/DevcraftWMS.Portal`
2) Autenticar no portal e selecionar um cliente.
3) Acessar **Outbound Orders** no menu.
4) Criar uma OS em **New Order**, preencher warehouse, order number e ao menos 1 item.
5) Validar criação e abrir **View** para ver detalhes e itens.

## Status
DONE
