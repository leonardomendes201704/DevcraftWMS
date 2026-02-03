# TASK-0065 - Portaria: atribuicao de doca e status

## Resumo
Atribuir doca e atualizar status OE/veiculo.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Endpoint de assign dock
- Atualizacao de status
- Audit trail

## Dependencias
- TASK-0063

## Estimativa
- 4h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Status muda para Em doca
- Timestamp registrado
- UI reflete mudanca

## Status
DONE
