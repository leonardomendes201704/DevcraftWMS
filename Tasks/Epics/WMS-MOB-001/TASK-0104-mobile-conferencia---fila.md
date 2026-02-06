# TASK-0104 - Mobile: Outbound Check Queue

## Resumo
Mostrar a fila de conferencia outbound com filtros e acao de iniciar tarefa.

## Contexto
Apos o picking, existem tarefas de conferencia (OutboundCheck) com status e prioridade no backend. Esta tela lista as tarefas e permite iniciar uma delas.

## Objetivo
- Listar tarefas de conferencia com status/prioridade.
- Filtrar por status, prioridade, armazem e OS.
- Iniciar uma tarefa (Start) e navegar para a execucao.

## Escopo
- Tela de fila (lista paginada).
- Filtros e ordenacao.
- Acao Start (iniciar conferencia).

## Dependencias
- API: `GET /api/outbound-checks`.
- API: `POST /api/outbound-checks/{id}/start`.
- Execucao (TASK-0105): `POST /api/outbound-orders/{id}/check`.

## Requisitos de UI/UX
- Filtro padrao: Status = Pending.
- Ordenacao padrao: CreatedAtUtc desc.
- Exibir prioridade com badge e status com cor.

## Campos (lista)
- orderNumber
- warehouseName
- status (Pending/InProgress/Completed/Canceled)
- priority (Low/Normal/High/Urgent)
- itemsCount
- createdAtUtc

## Headers obrigatorios
- `X-Customer-Id: <customer-guid>`

## API Integration

### 1) Listar fila
`GET /api/outbound-checks`

**Query params:**
- warehouseId (guid)
- outboundOrderId (guid)
- status (int enum)
- priority (int enum)
- isActive (bool)
- includeInactive (bool)
- pageNumber (int)
- pageSize (int)
- orderBy (string)
- orderDir (string)

**Ordenacao suportada (orderBy):**
- status
- priority
- checkedAtUtc
- createdAtUtc

**Observacoes:**
- `includeInactive=false` por padrao (nao retorna IsActive=false).
- Se `isActive` for informado, ele tem prioridade sobre `includeInactive`.

**Response (PagedResult):**
```json
{
  "items": [
    {
      "id": "b8f1c9d4-4b4b-4d10-9d7c-97b7b1ce05b1",
      "outboundOrderId": "a3a7b9d2-21c1-4c07-90e1-5d2dc2fb2bb1",
      "warehouseId": "c4b85775-83f6-417f-af87-50ff5682de20",
      "orderNumber": "OS-2026-000781",
      "warehouseName": "Demo Warehouse",
      "status": 0,
      "priority": 1,
      "itemsCount": 6,
      "createdAtUtc": "2026-02-06T12:00:00Z",
      "isActive": true
    }
  ],
  "totalCount": 1,
  "pageNumber": 1,
  "pageSize": 20,
  "orderBy": "CreatedAtUtc",
  "orderDir": "desc"
}
```

### 2) Iniciar tarefa
`POST /api/outbound-checks/{id}/start`

**Request body:** vazio.

**Response (OutboundCheckDto):**
```json
{
  "id": "b8f1c9d4-4b4b-4d10-9d7c-97b7b1ce05b1",
  "outboundOrderId": "a3a7b9d2-21c1-4c07-90e1-5d2dc2fb2bb1",
  "warehouseId": "c4b85775-83f6-417f-af87-50ff5682de20",
  "orderNumber": "OS-2026-000781",
  "warehouseName": "Demo Warehouse",
  "status": 1,
  "priority": 1,
  "startedByUserId": "00000000-0000-0000-0000-000000000002",
  "startedAtUtc": "2026-02-06T12:05:00Z",
  "checkedByUserId": null,
  "checkedAtUtc": null,
  "notes": null,
  "items": [
    {
      "id": "4a3d2b18-3e5f-4c2d-8d5d-92c4b7ef1234",
      "outboundOrderItemId": "0b8a1f53-7d8c-4b41-a2cb-2cfa4a0f9fd1",
      "productId": "3c3aa2d0-0f20-4f2a-b3e7-5b28b0d0a4ae",
      "uomId": "f1d6d0d0-1d7a-4efb-b3f2-5fd1f9b77b2c",
      "productCode": "SKU-001",
      "productName": "Sample Product",
      "uomCode": "EA",
      "quantityExpected": 5,
      "quantityChecked": 0,
      "divergenceReason": null,
      "evidenceCount": 0
    }
  ]
}
```

**Falhas esperadas:**
- `outbound_checks.check.not_found` quando o id nao existir.
- `outbound_checks.check.status_locked` quando Status for Completed/Canceled.
- `outbound_checks.check.picking_incomplete` quando ainda houver picking tasks pendentes.
- `customers.context.required` quando o header `X-Customer-Id` nao for enviado.

**Exemplo de erro (RequestResult -> ProblemDetails/Erro padrao):**
```json
{
  "errorCode": "outbound_checks.check.picking_incomplete",
  "message": "Picking tasks must be completed before starting the check."
}
```

### 3) Finalizar conferencia (execucao)
`POST /api/outbound-orders/{id}/check`

**Request:**
```json
{
  "items": [
    {
      "outboundOrderItemId": "0b8a1f53-7d8c-4b41-a2cb-2cfa4a0f9fd1",
      "quantityChecked": 5,
      "divergenceReason": null,
      "evidence": [
        {
          "fileName": "photo1.jpg",
          "contentType": "image/jpeg",
          "sizeBytes": 12345,
          "content": "<base64>"
        }
      ]
    }
  ],
  "notes": "Checked ok"
}
```

## Regras
- Status enum: Pending=0, InProgress=1, Completed=2, Canceled=3.
- Priority enum: Low=0, Normal=1, High=2, Urgent=3.
- Start deve ser bloqueado se status != Pending.
- Start so pode ocorrer quando todas as picking tasks da OS estiverem Completed.

## Como testar
- UI: abrir fila, filtrar por Pending e iniciar uma tarefa.
- Swagger: `GET /api/outbound-checks` + `POST /api/outbound-checks/{id}/start`.

## Status
PENDING