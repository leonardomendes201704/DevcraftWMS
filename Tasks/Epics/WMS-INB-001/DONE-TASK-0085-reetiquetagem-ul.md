# TASK-0085 - Reetiquetagem UL/SSCC

## Resumo
Reetiquetagem de UL/SSCC com rastreio.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Novo SSCC interno
- Historico de troca
- UI simples

## Dependencias
- TASK-0067

## Estimativa
- 4h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Nova etiqueta gerada
- Historico preservado
- Auditoria completa

## Status
DONE

## Implementacao
- Criada entidade UnitLoadRelabelEvent com historico de reetiquetagem.
- API: endpoint POST /api/unit-loads/{id}/relabel com CQRS e validacao.
- DemoMvc: tela de detalhes com form de reetiquetagem e tabela de historico.
- Tests: unit + integration cobrindo reetiquetagem.
- Migration: AddUnitLoadRelabelEvents.

## Como testar
1) Abrir Unit Loads > escolher um UL > Details.
2) Preencher Reason e (opcional) Notes e clicar em "Generate New SSCC".
3) Confirmar que o SSCC interno mudou e que a tabela "Re-label History" mostra o evento.
4) Via API: POST /api/unit-loads/{id}/relabel com JSON { "reason": "...", "notes": "..." } e verificar retorno com novo SSCC.
