# TASK-0007 - Cadastro de Produtos (SKU) e Unidades de Medida (UoM)

## Resumo
Implementar o cadastro completo de produtos (SKU) e unidades de medida (UoM) como base para operacoes de estoque, incluindo codigos, dimensoes, peso e conversoes.

## Contexto
O WMS depende de produtos bem definidos para receber, armazenar, separar e expedir com precisao. UoM padroniza quantidade, volume e peso por unidade.

## Problema
Nao existe um cadastro centralizado de produtos e UoM, impossibilitando estoque real e operacoes futuras (recebimento, picking, inventario).

## Objetivos
- CRUD E2E de Produtos (SKU) com dados completos.
- CRUD E2E de Unidades de Medida (UoM).
- Permitir UoM base e conversoes por produto.
- Garantir integridade e validacoes.

## Nao Objetivos
- Nao implementar precificacao comercial.
- Nao integrar com ERP externo nesta fase.

## Stakeholders
- Operacoes
- TI / Arquitetura
- Planejamento de Estoque

## Premissas
- Produto pode ter multiplas UoM, mas deve ter uma UoM base.
- Codigos (SKU, EAN/GTIN, Codigo ERP) devem ser unicos quando informados.

## User Stories / Casos de Uso
- Como operador, quero cadastrar um produto com dimensoes e peso para alocacao correta.
- Como gestor, quero definir UoM e conversoes (ex.: 1 CX = 12 UN).
- Como supervisor, quero desativar produtos obsoletos sem perder historico.

## Requisitos Funcionais
### Produto (SKU)
- Campos obrigatorios:
  - Code (SKU interno)
  - Name
  - BaseUomId
- Campos opcionais:
  - Description
  - Ean (GTIN)
  - ErpCode
  - Category
  - Brand
  - Status (IsActive)
- Dados fisicos:
  - WeightKg
  - LengthCm
  - WidthCm
  - HeightCm
  - VolumeCm3 (derivado ou armazenado)
- Relacionamentos:
  - 1:N com ProductUom (conversoes)

### Unidade de Medida (UoM)
- Campos:
  - Code (ex.: UN, CX, KG, L)
  - Name (ex.: Unidade, Caixa, Quilograma)
  - Type (enum: Unit, Weight, Volume, Length)
  - IsBase (bool) [apenas para o produto]

### Conversao de UoM por Produto
- ProductUom:
  - ProductId
  - UomId
  - ConversionFactor (ex.: 1 CX = 12 UN)
  - IsBase (true apenas para UoM base)

### CRUD E2E
- Produtos: create, update, list paginado, details, deactivate.
- UoM: create, update, list, details, deactivate.

### Filtros e ordenacao
- Produtos: code, name, category, brand, ean, isActive.
- UoM: code, name, type, isActive.
- Ordenacao por qualquer campo.

## Requisitos Nao Funcionais
- Paginacao obrigatoria em listas.
- Logs e auditoria via TransactionLogs.
- Validacoes consistentes via FluentValidation.

## Criterios de Aceitacao
- CRUD completo de Produtos e UoM em API e DemoMvc.
- Conversoes por produto funcionando.
- Migrations criadas e aplicadas.
- Testes unitarios e de integracao (cenarios felizes e tristes).

## Mudancas de API
- /api/products (POST/GET)
- /api/products/{id} (GET/PUT/DELETE)
- /api/uoms (POST/GET)
- /api/uoms/{id} (GET/PUT/DELETE)
- /api/products/{id}/uoms (GET/POST)

## Modelo de Dados / Migracoes
- Tabelas:
  - Products
  - Uoms
  - ProductUoms (join + conversion factor)

## Observabilidade / Logging
- TransactionLogs para CRUD e alteracoes de conversao.

## Plano de Testes
- Unit tests:
  - Validacao de SKU duplicado.
  - Validacao de UoM duplicada.
  - Conversao com fator invalido.
- Integration tests:
  - CRUD Products.
  - CRUD Uoms.
  - Criar UoM base + conversao por produto.

## Rollout / Deploy
- Aplicar migration AddProductsAndUoms.

## Riscos / Questões em Aberto
- Definir padrao de codigo SKU (formato).
- Definir se Volume sera calculado ou armazenado.

## Status
Concluida (DONE)

## Progresso / Notas
- Entidades Product, Uom e ProductUom criadas com relacionamentos 1:N.
- CRUD API implementado com CQRS + MediatR e RequestResult.
- Telas DemoMvc (Index/Details/Create/Edit/Delete) para Produtos e UoM.
- Conversoes de UoM por produto adicionadas via endpoint dedicado.
- Migrations criadas e aplicadas (AddProductsAndUoms).
- Testes unitarios e de integracao adicionados.

## Checklist de Aceitacao
- [x] API + DemoMvc completos.
- [x] Conversoes por produto funcionando.
- [x] Migrations criadas/aplicadas.
- [x] Testes unitarios e integracao.
