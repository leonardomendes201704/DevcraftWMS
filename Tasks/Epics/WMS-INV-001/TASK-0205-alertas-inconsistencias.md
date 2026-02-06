# TASK-0205 - Alertas operacionais e inconsistencias

## Summary
Detectar alertas (validade proxima, quarentena, divergencias, fragmentacao e restricoes de endereco) e exibir na consulta.

## Objetivo
- Alertar riscos e inconsistencias para o Backoffice/Supervisor.

## Escopo
- Regras de alerta e severidade.
- Indicadores no resumo e detalhes.

## Regras
- FEFO: alertar validade <= 15 dias (configuravel).
- Quarentena/bloqueio sempre sinalizados.
- Fragmentacao: produto em N enderecos acima de limite.

## Tests
- Unit: regras de alerta.

## Status
PENDING
