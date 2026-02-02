# TASK-0016 - Seed de Armazem com Produtos (Demo)

## Resumo
Criar um seed controlado por appsettings para popular um armazem e um cliente com 10 produtos, incluindo toda a hierarquia fisica e mapeamentos de visibilidade.

## Contexto
Precisamos de um dataset consistente para demonstrar a operacao do WMS, com armazem, estrutura fisica e catalogo de produtos de um cliente.

## Problema
Sem seed, a aplicacao sobe vazia e dificulta testes manuais e apresentacoes.

## Objetivos
- Permitir habilitar/desabilitar o seed por configuracao.
- Criar um armazem demo e um cliente demo.
- Criar hierarquia fisica completa (setor, secao, estrutura, corredor, enderecos).
- Criar 10 produtos com UoM e conversoes.
- Garantir visibilidade por cliente via mapeamentos *Customers.

## Nao Objetivos
- Criar estoque/saldo por endereco.
- Criar movimentacoes ou lotes.

## Stakeholders
- Operacoes
- QA
- Comercial
- TI / Arquitetura

## Premissas
- CustomerContext via header X-Customer-Id continua obrigatorio.
- Produtos seguem com ownership direto por CustomerId.

## User Stories / Casos de Uso
- Como analista, quero subir a API e ver dados demo prontos.
- Como vendedor, quero demonstrar a navegacao do WMS sem cadastrar tudo manualmente.

## Requisitos Funcionais
- Seed ativavel por `Seed:SampleData:Enabled`.
- Criar Customer (id configuravel).
- Criar Warehouse com endereco, contato e capacidade.
- Criar Sector, Section, Structure, Aisle e Locations.
- Criar mapeamentos de visibilidade para o cliente em todas as entidades fisicas.
- Criar 10 produtos com base UoM e UoM alternativa (BOX).

## Requisitos Nao Funcionais
- Seed deve ser idempotente (nao duplicar se ja existir).
- Seed nao deve rodar se estiver desabilitado.

## Mudancas de API
- Nenhuma.

## Modelo de Dados / Migracoes
- Nenhuma nova migracao.

## Observabilidade / Logging
- Logs informativos ao executar o seed.

## Plano de Testes
- Build e test suite completa.
- Validar manualmente se dados aparecem ao habilitar o seed.

## Rollout / Deploy
- Atualizar appsettings.
- Habilitar somente em Development/Testing quando necessario.

## Riscos / Questoes em Aberto
- Se o CustomerId configurado nao existir e o seed estiver ativo, a API deve criar o cliente no seed.

## Status
Concluida (DONE)

## Progresso / Notas
- SampleDataSeeder e Options com validacao adicionados.
- Program.cs e DependencyInjection atualizados para executar o seed.
- appsettings e README (ENVs) atualizados.
- Seed agora completa entidades faltantes quando o warehouse ja existe.
