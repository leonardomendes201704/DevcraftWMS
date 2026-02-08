# TASK-0126 - Melhorias no cadastro de Armazens (Warehouses)

## Summary
Ajustar a tela de criacao/edicao de Armazens com geracao automatica de codigo, dropdown de Cost Center, UX de busca por CEP e mascaras/calculos.

## Objetivos
- Gerar automaticamente o codigo do armazem (padrao de mercado) e tornar o campo somente leitura.
- Criar cadastro auxiliar de Cost Center com CRUD completo e dropdown no Warehouse.
- Melhorar UX do lookup de CEP com loading e campos bloqueados.
- Adicionar mascara de celular BR no Contact Phone.
- Calcular automaticamente Total Area (m2) e deixar read-only.

## Escopo
### 1) Code (Warehouse)
- Geracao automatica do campo Code no backend.
- Campo read-only na UI, mas enviado no POST.
- Padrao aplicado: `WH-YYYY-0001` (sequencial por ano).

### 2) Cost Center Code (cadastro auxiliar)
- Criar entidade/CRUD: CostCenters.
- API endpoints: list, get, create, update, delete.
- Manter Warehouse com CostCenterCode/CostCenterName e popular a partir do dropdown.
- Dropdown no Warehouse form com ordenacao por Code.

### 3) CEP (address lookup)
- Mostrar indicador visual de carregamento ao consultar CEP.
- Campos read-only durante/apos lookup: AddressLine1, City, State.
- Adicionar campo Numero (AddressNumber) no formulario.

### 4) Contact Phone
- Mascara para celular BR (ex: (11) 9 9999-9999).

### 5) Total Area m2
- Campo calculado automaticamente (Length x Width) e read-only.

## Alteracoes tecnicas sugeridas
- Domain:
  - Nova entidade `CostCenter`.
  - Ajuste em `WarehouseAddress` para AddressNumber.
- Infrastructure:
  - Migration para nova tabela + coluna AddressNumber.
- Application:
  - Features de CostCenters (Commands/Queries + Validators).
  - Services para geracao de codigo automatico.
- Api:
  - Controller de CostCenters.
- DemoMvc:
  - Views de CostCenters (CRUD).
  - Warehouse form atualizado.

## Tests
- Unit: geracao de codigo do Warehouse.
- Integration: CRUD CostCenters.
- UI: form Warehouse com CEP/mascaras/read-only/calculo area.

## How to Test
- Unit: `dotnet test tests/DevcraftWMS.Tests.Unit/DevcraftWMS.Tests.Unit.csproj`.
- Integration: `dotnet test tests/DevcraftWMS.Tests.Integration/DevcraftWMS.Tests.Integration.csproj`.
- UI: DemoMvc > Cost Centers: criar/editar/inativar.
- UI: DemoMvc > Warehouses: criar com Code = AUTO, selecionar Cost Center, validar nome preenchido, CEP com "aguarde", AddressLine1/City/State read-only apos lookup, Total Area calculado, Phone com mascara BR.
- Swagger: `POST /api/cost-centers`, `GET /api/cost-centers`, `POST /api/warehouses`.

## Status
DONE
