# TASK-0008 - Lotes e Validades (FIFO/FEFO)

## Resumo
Cadastro de lotes por produto com validade e politicas FIFO/FEFO.

## Contexto
Controle de lotes e validade e necessario para rastreabilidade e compliance.

## Problema
Nao existe entidade de lote nem regras FIFO/FEFO.

## Objetivos
- - CRUD E2E de lotes por produto.
- - Suporte a validade e status de lote.
- - Preparar base para FIFO/FEFO.

## Nao Objetivos
- - Nao implementar bloqueio automatico de lotes vencidos.
- - Nao integrar com ERP externo.

## Stakeholders
- Operacoes
- Qualidade
- TI / Arquitetura

## Premissas
- Produto deve existir para criar lote.
- Validade pode ser opcional para produtos sem vencimento.

## User Stories / Casos de Uso
- Como operador, quero cadastrar lote e validade para rastrear itens.
- Como gestor, quero filtrar lotes por status e vencimento.

## Requisitos Funcionais
- - Entidade Lote: ProductId, Code, ManufactureDate?, ExpirationDate?, Status, IsActive.
- - CRUD completo.
- - Filtros: productId, code, status, expiração (range), isActive.
- - Ordenacao por qualquer campo.
- - Regras: codigo de lote unico por produto.

## Requisitos Nao Funcionais
- - Paginacao obrigatoria.
- - Logs e auditoria via TransactionLogs.

## Mudancas de API
- - /api/products/{productId}/lots (POST/GET paginado)
- - /api/lots/{id} (GET/PUT/DELETE)

## Modelo de Dados / Migracoes
- - Tabela Lots (FK -> Products).

## Observabilidade / Logging
- TransactionLogs para CRUD e mudancas de status.

## Plano de Testes
- - Unit: validar produto inexistente, codigo duplicado, datas invalidas.
- - Integration: CRUD e filtros.

## Rollout / Deploy
- - Migration AddLots.

## Riscos / Questoes em Aberto
- - Definir status de lote e expiracao padrao.

## Status
Aberta (TODO)
