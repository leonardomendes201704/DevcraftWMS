# TASK-0092 - ASN anexos: storage real (abstracao + config)

## Resumo
Substituir upload "fake" (blob no banco) por armazenamento externo real com metadados no banco.

## Objetivo
Introduzir arquitetura de storage (S3/Azure Blob/FileShare/Base64 em db) com configuracao via appsettings e validacao em startup.

## Escopo
- Criar abstracoes de storage (IFileStorage, IFileStorageClient).
- Implementar provider (inicialmente FileSystem e/ou MinIO/S3 e/ou base64 em db para dev).
- Configuracoes em appsettings (endpoint, bucket/container, base path).
- Persistir apenas metadados no banco (nome, tamanho, hash, url e se tiver flag para salvar em base64, persistir o base64).
- Atualizar entidade AsnAttachment para armazenar URL/Key.

## Dependencias
- DONE-TASK-0055

## Estimativa
- 6h

## Entregaveis
- Storage abstraction + provider default configurado.
- Persistencia de metadados de anexos e/ou base64 em db.
- Migrations necessarias.

## Criterios de Aceite
- Upload nao grava bytes no banco se a flag base64 nao tiver true.
- Config valida no startup (fail-fast).
- README atualizado (ENVs).

## Como testar
1) Confirmar `FileStorage` no `appsettings.json` da API:
   - `Provider = FileSystem`, `BasePath = App_Data/files`, `AsnAttachmentsPath = asns`.
2) Subir a API (`dotnet run --project src/DevcraftWMS.Api`).
3) Subir o Portal (`dotnet run --project src/DevcraftWMS.Portal`).
4) Criar um ASN e anexar um PDF.
5) Verificar se o banco nao possui base64 quando `StoreContentBase64 = false`.
6) Confirmar o arquivo salvo em `App_Data/files/asns/...`.

## Status
DONE
