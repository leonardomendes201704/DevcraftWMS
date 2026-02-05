# TASK-0094 - ASN anexos: Portal (download/preview)

## Resumo
Atualizar Portal para download real e preview de anexos.

## Objetivo
Exibir links de download e preview quando disponivel, com status e tamanho.

## Escopo
- Botao Download em Asn Details.
- Preview para PDF/imagem (quando suportado).
- Mensagens amigaveis e status de upload.

## Dependencias
- TASK-0093

## Estimativa
- 4h

## Entregaveis
- UI atualizada no Portal.
- Help/manual atualizado.

## Criterios de Aceite
- Download funciona para anexos.
- Preview disponivel para tipos suportados.

## Como testar
1) Subir a API e o Portal.
2) Abrir um ASN com anexos.
3) Validar os botoes "Download" e "Preview" na tabela de anexos.
4) Para PDF/imagem, clicar em "Preview" e confirmar abertura inline.
5) Para outros tipos, validar que o preview nao aparece.

## Status
DONE
