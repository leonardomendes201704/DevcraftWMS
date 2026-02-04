# TASK-0087 - Portal Cliente: relatorios de recebimento

## Resumo
Portal Cliente: relatorios de recebimento e divergencias.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Tela de relatorios
- Filtros por periodo
- Download/visualizacao

## Dependencias
- TASK-0080,TASK-0045

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Cliente acessa relatorios
- Filtros funcionam
- Download disponivel

## Status
DONE

## Implementacao
- Adicionada tela "Receiving Reports" no Portal Cliente com filtros por periodo.
- Listagem de OEs com acesso ao relatorio detalhado e download CSV.
- Filtros de data adicionados no backend para listar OEs por CreatedAtUtc.
- Help/manual da tela com instrucoes de uso.

## Como testar
1) Acesse Portal > Receiving Reports.
2) Defina Created From/To e filtre.
3) Clique em View para abrir o relatorio com divergencias.
4) Clique em CSV para baixar o arquivo do relatorio.
