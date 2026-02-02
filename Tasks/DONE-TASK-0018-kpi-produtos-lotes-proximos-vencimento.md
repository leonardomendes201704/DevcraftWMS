# TASK-0018 - KPI de Produtos/Lotes Proximos ao Vencimento no Dashboard

## Resumo
Adicionar um KPI no dashboard que indique a quantidade de produtos/lotes proximos ao vencimento, com filtros por periodo e status.

## Contexto
O acompanhamento de vencimentos e critico para operacoes FIFO/FEFO e para reduzir perdas. O dashboard deve destacar lotes proximos ao vencimento para acao rapida.

## Problema
Nao existe indicador visual no dashboard para alertar sobre lotes/produtos proximos ao vencimento.

## Objetivos
- KPI no dashboard com contagem de lotes proximos ao vencimento.
- Possibilidade de configurar janela de dias (ex: 7/15/30).
- Link para tela de Lots com filtro aplicado.

## Nao Objetivos
- Bloqueio automatico de lotes vencidos.
- Alertas via email/SMS.

## Stakeholders
- Operacoes
- Qualidade
- TI / Arquitetura

## Premissas
- Lotes ja estao cadastrados com ExpirationDate.
- Contexto de cliente ativo no front e na API.

## User Stories / Casos de Uso
- Como gestor, quero ver no dashboard quantos lotes estao proximos do vencimento.
- Como operador, quero clicar no KPI e ver os lotes filtrados.

## Requisitos Funcionais
- KPI exibindo total de lotes com ExpirationDate dentro da janela configurada.
- Filtro por status (ex: Available, Quarantined, Blocked, Expired).
- Link direto para tela de Lots com filtros pre-aplicados.

## Requisitos Nao Funcionais
- Consulta performatica (index em ExpirationDate).
- Respeitar contexto de cliente.
- Nao impactar carregamento do dashboard.

## Mudancas de API
- Endpoint de consulta agregada (ex: GET /api/dashboard/expiring-lots?days=30&status=Available).

## Modelo de Dados / Migracoes
- Nenhuma nova tabela.
- Requer index por ExpirationDate (ja previsto em Lots).

## Observabilidade / Logging
- RequestLogs para endpoint de dashboard.

## Plano de Testes
- Unit: regra de janela de vencimento.
- Integration: endpoint retorna contagem correta para dados seed.
- UI: KPI renderiza e redireciona com filtros.

## Rollout / Deploy
- Sem alteracoes de infraestrutura.

## Riscos / Questoes em Aberto
- Definir janela padrao (ex: 30 dias).
- Definir se lotes sem expiracao devem ser excluidos (padrao: sim).

## Status
Concluida (DONE)
