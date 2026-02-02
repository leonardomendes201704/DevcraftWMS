# TASK-0017 - Validacao de Paginacao (sem fallbacks)

## Resumo
Garantir que todas as consultas paginadas recusem PageNumber/PageSize invalidos, sem ajustes silenciosos.

## Contexto
Parametros de paginacao chegavam como 0 em alguns fluxos (Swagger e DemoMvc), causando listas vazias com total maior que zero.

## Problema
Sem validacao, a API aceitava pageNumber/pageSize = 0 e retornava resultados inconsistentes.

## Objetivos
- Validar pageNumber/pageSize em todas as queries paginadas.
- Remover fallbacks silenciosos no backend.
- Ajustar defaults no DemoMvc para evitar envio de 0.

## Nao Objetivos
- Alterar limites maximos alem dos existentes.
- Mudancas de schema.

## Requisitos Funcionais
- PageNumber > 0
- PageSize > 0
- Manter limites maximos existentes (ex: logs e setores)

## Requisitos Nao Funcionais
- Sem regressao em endpoints existentes.

## Mudancas de API
- Validation errors (400) quando paginacao invalida.

## Modelo de Dados / Migracoes
- Nenhuma.

## Observabilidade / Logging
- Sem impacto.

## Plano de Testes
- dotnet build
- dotnet test

## Status
Concluida (DONE)

## Progresso / Notas
- Validators adicionados para todas as queries paginadas.
- Fallbacks removidos do backend.
- Defaults corrigidos no DemoMvc para evitar PageNumber/PageSize = 0.
