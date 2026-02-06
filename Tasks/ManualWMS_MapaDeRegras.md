# Manual WMS - Mapa de Regras

Este mapa lista regras confirmadas no codigo e onde elas aparecem.

## Inbound
| Regra | Onde no codigo | Impacto no processo |
| --- | --- | --- |
| Status de Inbound Order | `src/DevcraftWMS.Domain/Enums/InboundOrderStatus.cs` | Define etapas da ordem de entrada. |
| Fluxo de ASN (criar/submeter/aprovar/convert/cancelar) | `src/DevcraftWMS.Api/Controllers/AsnsController.cs` + `src/DevcraftWMS.Application/Features/Asns/*` | Controla previsao de recebimento e conversao para Inbound Order. |
| Receipts (criar/iniciar/finalizar/itens) | `src/DevcraftWMS.Api/Controllers/ReceiptsController.cs` + `src/DevcraftWMS.Application/Features/Receipts/ReceiptService.cs` | Registra recebimento fisico e altera status. |
| Quality Inspection (approve/reject/evidence) | `src/DevcraftWMS.Api/Controllers/QualityInspectionsController.cs` | Inspecao de qualidade com evidencias. |
| Putaway tasks (confirm/reassign) | `src/DevcraftWMS.Api/Controllers/PutawayTasksController.cs` | Enderecamento e transferencia para local final. |
| Unit Loads (create/print/relabel) | `src/DevcraftWMS.Api/Controllers/UnitLoadsController.cs` | Etiquetagem e controle de unidades de carga. |

## Outbound
| Regra | Onde no codigo | Impacto no processo |
| --- | --- | --- |
| Outbound Order status e prioridades | `src/DevcraftWMS.Domain/Enums/OutboundOrderStatus.cs`, `OutboundOrderPriority.cs` | Define pipeline de saida. |
| Criacao de OS (validacoes basicas) | `src/DevcraftWMS.Application/Features/OutboundOrders/OutboundOrderService.cs` | OS exige warehouse, order number unico e itens validos. |
| Release de OS (janela + reserva) | `src/DevcraftWMS.Application/Features/OutboundOrders/OutboundOrderService.cs` | Janela valida; reserva estoque; gera picking. |
| Picking task status | `src/DevcraftWMS.Domain/Enums/PickingTaskStatus.cs` | Estados das tarefas de picking. |
| Confirmacao de picking (quantidade) | `src/DevcraftWMS.Application/Features/PickingTasks/Commands/ConfirmPickingTask/ConfirmPickingTaskCommandHandler.cs` | Quantidade nao pode exceder planejado; completa tarefa. |
| Outbound Check (divergencia + evidencia) | `src/DevcraftWMS.Application/Features/OutboundChecks/OutboundCheckService.cs` | Divergencia exige motivo; evidencia opcional. |
| Start de conferencia exige picking completo | `src/DevcraftWMS.Application/Features/OutboundChecks/OutboundCheckService.cs` | Bloqueia conferencia se picking incompleto. |
| Packing (volumes) | `src/DevcraftWMS.Application/Features/OutboundPacking/OutboundPackingService.cs` | Cria volumes e itens de volume. |
| Shipping (embarque) | `src/DevcraftWMS.Application/Features/OutboundShipping/OutboundShippingService.cs` | Registra doca e horarios; altera status. |
| Outbound notifications | `src/DevcraftWMS.Application/Features/OutboundOrderNotifications/*` | Envia notificacoes de eventos outbound. |

## Inventario e rastreio
| Regra | Onde no codigo | Impacto no processo |
| --- | --- | --- |
| Reserva de estoque no release | `src/DevcraftWMS.Application/Features/OutboundOrders/OutboundOrderService.cs` + `IInventoryBalanceRepository` | Impede liberar OS sem saldo. |
| Status de saldo | `src/DevcraftWMS.Domain/Enums/InventoryBalanceStatus.cs` | Define disponibilidade de estoque. |
| Movimentacoes | `src/DevcraftWMS.Application/Features/InventoryMovements/*` | Auditoria de entradas/saidas. |
| Lotes e validade | `src/DevcraftWMS.Domain/Entities/Lot.cs` + `OutboundOrderService.ValidateTrackingMode` | Rastreio por lote e vencimento. |

## Permissoes e RBAC
| Regra | Onde no codigo | Impacto |
| --- | --- | --- |
| Policies por role | `src/DevcraftWMS.Api/Program.cs` | Controla acesso por papel. |
| Roles e permissions seed | `src/DevcraftWMS.Infrastructure/Auth/RbacSeeder.cs` | Mapeia roles iniciais e permissoes. |

## Contexto de cliente
| Regra | Onde no codigo | Impacto |
| --- | --- | --- |
| Header X-Customer-Id obrigatorio | `src/DevcraftWMS.Api/Middleware/CustomerContextMiddleware.cs` | Filtra todos os dados por cliente. |

## Observacoes
- Itens marcados como “Regra nao confirmada” devem ser validados na implementacao do manual.