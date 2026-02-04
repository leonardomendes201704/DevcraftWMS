# TASK-0076 - Putaway: motor de sugestao de endereco

## Resumo
Motor de sugestao de endereco (zona/capacidade/compatibilidade).

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Regras de sugestao
- Filtro por compatibilidade
- Ranking de enderecos

## Dependencias
- TASK-0075

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Sugestoes respeitam regras
- Enderecos incompativeis excluidos
- Explicacao de escolha

## How to test
1) Gere uma Putaway Task (crie UL e Print Label).
2) DemoMvc -> Putaway Tasks -> Details.
   - Esperado: lista de sugestoes com zonas nao-quarentena e justificativa.
3) API: GET /api/putaway-tasks/{id}/suggestions?limit=5 (com X-Customer-Id).
   - Esperado: lista sem zonas Quarantine e sem locais incompativeis.

## Status
DONE
