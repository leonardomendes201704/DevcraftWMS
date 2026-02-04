# TASK-0079 - OE: validacoes de encerramento

## Resumo
Validacoes de encerramento de OE.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Checagem UL em destino valido
- Status parcial/completo
- Mensagens de bloqueio

## Dependencias
- TASK-0077,TASK-0074

## Estimativa
- 5h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- OE nao fecha com UL solta
- Encerramento parcial suportado
- Auditoria registrada

## Status
DONE

## Como testar
1) API: criar ASN, aprovar, converter para OE.
2) Iniciar recebimento pela OE (`POST /api/inbound-orders/{id}/receipts/start`).
3) Adicionar item e completar o recebimento (`POST /api/receipts/{id}/items` + `/complete`).
4) Criar Unit Load, imprimir e confirmar putaway (`/api/unit-loads`, `/print`, `/api/putaway-tasks/{id}/confirm`).
5) Encerrar OE (`POST /api/inbound-orders/{id}/complete`) com `allowPartial=false`.
6) Verificar status `Completed` e evento de status na resposta.

## Observacoes
- Encerramento parcial retorna status `PartiallyCompleted` quando ha recebimentos ou putaway pendentes e `allowPartial=true`.
