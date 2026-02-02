# TASK-0057 - ASN: workflow de status + auditoria

## Resumo
Workflow de status ASN com auditoria.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Transicoes Registrado/Pendente/Aprovado/Convertido/Cancelado
- Registro de eventos
- Historico visivel

## Dependencias
- TASK-0053

## Estimativa
- 4h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Transicoes validas apenas
- Auditoria registrada
- UI mostra historico

## Status
PENDENTE
