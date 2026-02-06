# WMS-TASK-0003 - Agenda de doca outbound

## Resumo
Implementar agenda de doca para embarques outbound, com controle de conflitos.

## Contexto
O modulo de shipping nao possui agenda formal de doca; isso gera conflitos de horario e baixa previsibilidade.

## Objetivo
- Criar DockSchedule e DockScheduleStatus.
- Criar slots de doca e associar embarques.
- Bloquear conflitos de horario na mesma doca.

## Escopo
- API + Application + Domain + Infrastructure + DemoMvc.
- Listagem paginada, detalhe, criar, reagendar, cancelar.

## Fora de escopo
- Integracao com TMS.

## Regras de negocio
- Slots nao podem se sobrepor na mesma doca.
- Reagendamento exige motivo.
- Status: Scheduled -> InProgress -> Completed (ou Canceled).

## Solucao tecnica sugerida
- Domain:
  - Enums: DockScheduleStatus.
  - Entity: DockSchedule.
- Application:
  - Features/DockSchedules (commands/queries/validators/service/mapping/dtos).
  - Commands: CreateDockSchedule, RescheduleDockSchedule, CancelDockSchedule, AssignDockSchedule.
  - Queries: ListDockSchedulesPaged, GetDockScheduleById.
- Infrastructure:
  - EF configs + repositories + migration.
- API:
  - DockSchedulesController + Contracts.
- DemoMvc:
  - DockSchedulesController + ApiClient + ViewModels + Views.
- Tests:
  - Unit: validacao de conflito e transicoes.
  - Integration: create/list/reschedule.

## Telas/rotas
- /DockSchedules (lista)
- /DockSchedules/{id} (detalhe + acoes)

## Endpoints
- GET /api/dock-schedules
- GET /api/dock-schedules/{id}
- POST /api/dock-schedules
- POST /api/dock-schedules/{id}/reschedule
- POST /api/dock-schedules/{id}/cancel
- POST /api/dock-schedules/{id}/assign

## Criterios de aceite
- Dado um slot ocupado, quando tentar criar outro no mesmo intervalo, entao retorna erro de conflito.
- Dado um slot criado, quando reagendar, entao atualiza datas e registra motivo.

## Logs/Auditoria
- Registrar alteracoes de agenda.

## Testes necessarios
- Unit e integration conforme acima.

## Dependencias/Riscos
- Definir docas disponiveis por warehouse.

## Status
PENDING
