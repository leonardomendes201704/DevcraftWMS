# TASK-0086 - Cubagem/Pesagem real no recebimento

## Resumo
Capturar cubagem/pesagem real no recebimento.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Campos de peso/volume real
- Comparacao com cadastro
- Bloqueio opcional

## Dependencias
- TASK-0047

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Valores reais salvos
- Diferenca sinalizada
- Regra configuravel

## Status
DONE

## Implementacao
- Adicionados campos de peso/volume esperado e real em ReceiptItem.
- Comparacao com cadastro (produto) com calculo de desvio e sinalizacao.
- Regra configuravel para bloquear quando desvio excede limites.
- UI do Recebimento atualizada para capturar peso/volume real e exibir comparacao.
- Tests e migration AddReceiptItemMeasurements.

## Como testar
1) Em Receipts > Details, adicionar item com Actual Weight/Volume preenchidos.
2) Verificar colunas Expected/Actual e o badge de desvio na grid.
3) Ajustar ReceiptMeasurements:BlockOnDeviation=true e MaxWeight/VolumeDeviationPercent baixo para validar bloqueio.
4) Confirmar que a API retorna erro "receipts.item.measurement_out_of_range" quando excede o limite.
