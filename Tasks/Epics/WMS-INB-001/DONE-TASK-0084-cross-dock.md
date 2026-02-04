# TASK-0084 - Cross-dock: excecao sem putaway

## Resumo
Fluxo de cross-dock sem putaway.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Zona cross-dock
- Tarefas de expedicao imediata
- Excecao no fechamento OE

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
- Cross-dock ignora putaway
- Status correto
- Relatorio indica cross-dock

## Como testar
1) Garanta que existe uma zona Cross-dock e locations CD-01/CD-02 (seed ou cadastro manual).
2) Crie/abra uma OE e inicie um Receipt.
3) Adicione itens do receipt usando uma Location da zona Cross-dock.
4) Imprima a etiqueta do Unit Load (não deve criar Putaway Task).
5) Finalize o Receipt e feche a OE (não deve bloquear por putaway/unit load).
6) Abra o relatório da OE e valide a seção “Cross-dock Lines”.

## Status
DONE
