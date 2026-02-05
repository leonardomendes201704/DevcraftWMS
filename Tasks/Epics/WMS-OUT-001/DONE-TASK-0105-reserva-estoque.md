# TASK-0105 - Reserva de estoque e validacao disponibilidade

## Resumo
Reservar estoque ao liberar OS e validar disponibilidade.

## Objetivo
Evitar conflito entre OS e garantir saldo reservado.

## Escopo
- Reserva de InventoryBalances.
- Validacao de saldo disponivel.
- Regras de bloqueio/quarentena.

## Dependencias
- TASK-0102

## Estimativa
- 6h

## Entregaveis
- Reserva automatica e validacao.

## Criterios de Aceite
- OS nao libera sem saldo disponivel.

## Como testar
- Criar um produto e um saldo de inventario (Inventory Balance) com quantidade disponivel.
- Criar uma OS com item para esse produto.
- Acessar a OS e clicar em Release:
  - Deve liberar quando houver saldo suficiente.
  - Deve retornar erro amigavel quando o saldo disponivel for insuficiente.

## Status
DONE
