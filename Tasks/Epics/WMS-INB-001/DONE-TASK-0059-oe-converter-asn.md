# TASK-0059 - OE: converter ASN -> OE

## Resumo
Converter ASN em OE mantendo itens e dados.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Command de conversao
- Validacoes
- Auditoria de conversao

## Dependencias
- TASK-0058,TASK-0053

## Estimativa
- 5h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- OE gerada a partir do ASN
- ASN marcado como Convertido
- Logs registrados

## Status
DONE
