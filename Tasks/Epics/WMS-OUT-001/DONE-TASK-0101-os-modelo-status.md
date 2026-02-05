# TASK-0101 - OS: modelo de dados e status

## Resumo
Criar modelo de Ordem de Saida (OS) e status do fluxo outbound.

## Objetivo
Persistir OS com itens, dados de transporte, prioridade e status.

## Escopo
- Entidade OutboundOrder e OutboundOrderItem.
- Enum de status (Registrada, Liberada, EmSeparacao, Conferida, Embalada, EmExpedicao, Expedida, Parcial, Cancelada).
- EF config e migration.

## Dependencias
- TASK-0100

## Estimativa
- 6h

## Entregaveis
- Modelo + migration.

## Criterios de Aceite
- OS e itens persistidos com status e auditoria.

## Como testar
1) Executar migrations da MainDb:
   - `dotnet ef database update --project src/DevcraftWMS.Infrastructure --startup-project src/DevcraftWMS.Api`
2) Verificar tabelas `OutboundOrders` e `OutboundOrderItems` no SQLite.
3) Confirmar enums `OutboundOrderStatus` e `OutboundOrderPriority` com DisplayName.

## Status
DONE
