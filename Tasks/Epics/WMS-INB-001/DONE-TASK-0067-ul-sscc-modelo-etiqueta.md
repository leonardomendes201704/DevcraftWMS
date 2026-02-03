# TASK-0067 - UL/SSCC: modelo + etiqueta interna

## Resumo
Modelo de UL/SSCC e geracao de etiqueta interna.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- UL entity + status
- Geracao SSCC interno
- Endpoint impressao

## Dependencias
- TASK-0066

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- UL criada sem SSCC externo
- Etiqueta gerada
- UL vinculada ao recebimento

## Status
DONE

## Implementacao
- UnitLoad entity + status, com SSCC interno gerado e opcional externo.
- Endpoints de criacao, listagem, detalhe e impressao de etiqueta.
- UI DemoMvc (Index/Create/Details) com modal de help e impressao.
- Tests unitarios e integracao para criacao + etiqueta.
