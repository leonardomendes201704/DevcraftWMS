# TASK-0081 - Notificacao ao cliente (portal/email/webhook)

## Resumo
Notificar cliente sobre encerramento/pendencias de OE.

## Objetivo
Entregar escopo pequeno e testavel, mantendo padrao Clean Architecture e UI/UX consistente com DemoMvc.

## Escopo
- Email/webhook/portal
- Template de mensagem
- Registro de envio

## Dependencias
- TASK-0080,TASK-0045

## Estimativa
- 6h

## Entregaveis
- CQRS + endpoints (quando aplicavel).
- UI com layout padrao e help/manual da tela.
- Tests (unit + integration) quando houver regra de negocio.
- README/ENVs atualizados se appsettings mudar.

## Criterios de Aceite
- Notificacao enviada
- Registro de entrega
- Reenvio manual

## Status
CONCLUIDO

## Como testar
1. Execute a API e o Portal.
2. No Portal, abra uma OE e finalize (Complete Order).
3. Na mesma tela, verifique a secao "Notifications" com registros Email/Portal (Webhook se habilitado).
4. Clique em "Resend" e confirme o novo envio (Attempts incrementa).
5. Opcional: habilite `Notifications:InboundOrders:WebhookEnabled=true` e configure `WebhookUrl` para validar webhook.
