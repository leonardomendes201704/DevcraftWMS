# TASK-0116 - Notificacoes outbound (portal/email/webhook)

## Resumo
Notificar cliente ao concluir expedicao e em caso de pendencias.

## Objetivo
Enviar comunicacoes automaticas para o cliente.

## Escopo
- Email/portal/webhook.
- Templates de notificacao.

## Dependencias
- TASK-0115

## Estimativa
- 6h

## Entregaveis
- Notificacoes operacionais.

## Criterios de Aceite
- Cliente recebe evento de OS expedida.

## Status
DONE

## Progresso
- Notificacoes outbound por Email/Webhook/Portal ao expedir OS.
- Endpoints para listar e reenviar notificacoes.
- Portal exibe notificacoes com opcao de reenvio.
- Migration adicionada para OutboundOrderNotifications.
- Testes unitarios e de integracao cobrindo notificacoes.

## How to test
```bash
dotnet build DevcraftWMS/DevcraftWMS.sln
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj
dotnet test DevcraftWMS/tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj
```

### UI (Portal)
1) Criar uma outbound order, liberar, conferir, pack e expedir.
2) Abrir a OS no Portal e validar a tabela **Notifications** com entradas Email/Portal/Webhook.
3) Usar **Resend** para reenviar uma notificacao.

### Swagger (API)
1) Abrir Swagger do `DevcraftWMS.Api`.
2) Chamar `GET /api/outbound-orders/{id}/notifications` e validar retorno.
3) Chamar `POST /api/outbound-orders/{id}/notifications/{notificationId}/resend` e validar sucesso.
