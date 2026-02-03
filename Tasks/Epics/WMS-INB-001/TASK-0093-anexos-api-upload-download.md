# TASK-0093 - ASN anexos: API upload/download real

## Resumo
Atualizar endpoints de anexos para usar storage real e fornecer download seguro.

## Objetivo
Permitir upload real para storage e download via URL assinada ou stream controlado.

## Escopo
- Atualizar POST /asns/{id}/attachments para enviar arquivo ao storage.
- Adicionar GET /asns/{id}/attachments/{attachmentId}/download.
- Validar tamanho/tipo de arquivo.
- Registrar hash e tamanho para integridade.

## Dependencias
- TASK-0092

## Estimativa
- 6h

## Entregaveis
- Endpoints atualizados + CQRS.
- Logs/telemetria de uploads.
- Tests (unit + integration).

## Criterios de Aceite
- Upload e download funcionais.
- Autorizacao e contexto de cliente respeitados.
- Problemdetails amigavel.

## Status
PENDENTE
