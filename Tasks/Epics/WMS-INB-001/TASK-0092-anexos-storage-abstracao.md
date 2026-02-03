# TASK-0092 - ASN anexos: storage real (abstracao + config)

## Resumo
Substituir upload "fake" (blob no banco) por armazenamento externo real com metadados no banco.

## Objetivo
Introduzir arquitetura de storage (S3/Azure Blob/FileShare) com configuracao via appsettings e validacao em startup.

## Escopo
- Criar abstrações de storage (IFileStorage, IFileStorageClient).
- Implementar provider (inicialmente FileSystem ou MinIO/S3 para dev).
- Configuracoes em appsettings (endpoint, bucket/container, base path).
- Persistir apenas metadados no banco (nome, tamanho, hash, url).
- Atualizar entidade AsnAttachment para armazenar URL/Key e remover blob.

## Dependencias
- DONE-TASK-0055

## Estimativa
- 6h

## Entregaveis
- Storage abstraction + provider default configurado.
- Persistencia de metadados de anexos.
- Migrations necessarias.

## Criterios de Aceite
- Upload nao grava bytes no banco.
- Config valida no startup (fail-fast).
- README atualizado (ENVs).

## Status
PENDENTE
