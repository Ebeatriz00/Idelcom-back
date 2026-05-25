# Flujo de WarehousesMovement, InventoryStock e InventoryKardex

## Objetivo

Este flujo registra movimientos de almacén y mantiene actualizado el stock por producto y almacén.

La relación funcional principal es:

- `WarehousesMovement` registra la cabecera del movimiento.
- `WarehouseMovementDetail` registra los productos, cantidades y costos del movimiento.
- `InventoryStock` mantiene el stock acumulado por `BusinessId`, `WarehouseId` y `ProductsId`.
- `InventoryKardex` registra la trazabilidad historica de cada entrada, salida o transferencia de stock.

Todo el registro del movimiento y la actualización de inventario se ejecutan dentro de una misma transacción. Si falla el registro de cabecera, detalle, auditoría o actualización de stock, se revierte toda la operación.

## Modelo de Stock

`InventoryStock` representa el stock actual de un producto dentro de un almacén específico.

Clave lógica:

```text
BusinessId + WarehouseId + ProductsId
```

Campos principales:

- `BusinessId`: empresa propietaria del stock.
- `WarehouseId`: almacén donde se encuentra el stock.
- `ProductsId`: producto controlado.
- `StockQuantity`: cantidad actual.
- `AverageCost`: costo promedio.
- `LastCost`: último costo registrado.
- `LastEnteryDate`: fecha de última entrada.
- `LastOutputDate`: fecha de última salida.

## Modelo de Kardex

`InventoryKardex` representa el historial de movimientos que afectaron el stock. A diferencia de `InventoryStock`, que guarda el saldo actual, el Kardex guarda una linea por cada afectacion de inventario.

Campos principales:

- `BusinessId`: empresa propietaria del movimiento.
- `WarehouseId`: almacen afectado.
- `ProductsId`: producto afectado.
- `WareHouseMovementId`: cabecera del movimiento que origino el registro.
- `WareHouseMovementDetailId`: detalle del movimiento que origino el registro.
- `MovementTypesId`: tipo de movimiento.
- `MovementDate`: fecha del movimiento.
- `EntryQuantity`: cantidad de entrada. Es mayor a `0` solo en entradas.
- `ExitQuantity`: cantidad de salida. Es mayor a `0` solo en salidas.
- `PreviousStock`: stock antes de aplicar el movimiento.
- `FinalStock`: stock despues de aplicar el movimiento.
- `UnitCost`: costo unitario del detalle.
- `AverageCost`: costo promedio final del stock.
- `TotalCost`: costo total del detalle.
- `ReferenceDocumentType`: referencia de origen, actualmente `WAREHOUSES_MOVEMENT`.
- `ReferenceDocumentNumber`: documento de referencia, o serie-numero del movimiento.
- `Observation`: observacion del detalle o de la cabecera.

## Endpoints

### Registrar stock inicial

```http
POST api/InventoryStock/InventoryStockCreate
```

DTO:

```json
{
  "productsId": 10,
  "warehouseId": 2,
  "stockQuantity": 100,
  "averageCost": 12.5,
  "lastCost": 12.5,
  "lastEntryDate": "2026-05-12T10:00:00",
  "lastOutputDate": null
}
```

Este endpoint crea un registro inicial de stock para un producto en un almacén.

### Registrar movimiento de almacén

```http
POST api/WarehousesMovement/WarehousesMovementCreate
```

DTO:

```json
{
  "movementTypeId": 1,
  "warehouseId": 2,
  "warehouseDestinationId": 3,
  "suppliersId": null,
  "clientsId": null,
  "series": "F001",
  "numberDocument": "000123",
  "referenceDocument": "OC-456",
  "movementDate": "2026-05-12T10:00:00",
  "observation": "Traslado interno",
  "igv": 18,
  "details": [
    {
      "productsId": 10,
      "quantity": 5,
      "unitCost": 12.5,
      "lotNumber": "L001",
      "serialNumber": null,
      "expirationDate": "2027-01-01T00:00:00",
      "observation": null
    }
  ]
}
```

## Capas y Responsabilidades

### Controller

Archivos:

- `GlueMark/Controllers/Logistic/InventoryStockController.cs`
- `GlueMark/Controllers/Logistic/WarehousesMovementController.cs`

Responsabilidades:

- Recibir el request HTTP.
- Obtener `userId` y `businessId` desde claims.
- Delegar al caso de uso correspondiente.
- Retornar el resultado.

No contiene reglas de negocio.

### DTOs

Archivos:

- `Application/DTOs/InventoryStock/InventoryStockCreateDto.cs`
- `Application/DTOs/WarehousesMovement/WarehousesMovementCreateDto.cs`
- `Application/DTOs/WarehousesMovement/WarehousesMovementDetailCreateDto.cs`

Responsabilidades:

- Definir la forma del request de entrada.
- Mantener separados los datos de API respecto a las entidades de dominio.

### Validators

Archivos:

- `Application/Validators/InventoryStock/InventoryStockCreateValidator.cs`
- `Application/Validators/WarehousesMovement/WarehousesMovementCreateValidator.cs`

Responsabilidades:

- Validaciones de forma y obligatoriedad.
- Validaciones simples de rango, longitud y estructura.

Ejemplos:

- `ProductsId > 0`
- `WarehouseId > 0`
- `StockQuantity >= 0`
- `Quantity > 0`
- `UnitCost >= 0`
- longitud máxima de serie, documento, lote y observación.

Los validators no consultan base de datos y no contienen reglas de negocio.

### Business Rules

Archivos:

- `Application/UseCases/InventoryStock/InventoryStockBusinessRules.cs`
- `Application/UseCases/WarehousesMovement/WarehousesMovementBusinessRules.cs`
- `Application/UseCases/WarehousesMovement/StockOperation.cs`

Responsabilidades:

- Validar reglas que requieren repositorios o conocimiento del negocio.
- Evitar que los casos de uso mezclen lógica de negocio con orquestación.

#### Reglas de InventoryStock

`InventoryStockBusinessRules.ValidateForCreateAsync` valida:

1. El producto existe y pertenece al negocio.
2. El producto está activo.
3. El producto es stockable.
4. El almacén existe y pertenece al negocio.
5. No existe un stock previo para el mismo `BusinessId + WarehouseId + ProductsId`.
6. Si `StockQuantity` es `0`, no se permite registrar costos positivos.

#### Reglas de WarehousesMovement

`WarehousesMovementBusinessRules.ValidateForCreateAsync` valida:

1. El tipo de movimiento existe y pertenece al negocio.
2. El tipo de movimiento está activo.
3. El tipo de movimiento afecta stock.
4. Se determina si el movimiento es:
   - `Entry`
   - `Output`
   - `Transfer`
5. Si el tipo requiere almacén destino, `WarehouseDestinationId` debe venir informado.
6. Si la operación es transferencia, debe existir almacén destino.
7. El almacén origen y destino no pueden ser el mismo.
8. El almacén origen existe y pertenece al negocio.
9. El almacén destino existe y pertenece al negocio, cuando aplica.
10. No se permiten productos repetidos en el detalle.
11. Cada producto del detalle existe y pertenece al negocio.
12. Cada producto está activo.
13. Cada producto es stockable.
14. Si el producto gestiona lotes, el detalle debe incluir `LotNumber`.
15. Si el producto gestiona series, el detalle debe incluir `SerialNumber`.
16. Si el producto tiene control de vencimiento, el detalle debe incluir `ExpirationDate`.

### Use Cases

Archivos:

- `Application/UseCases/InventoryStock/CreateInventoryStock.cs`
- `Application/UseCases/WarehousesMovement/CreateWarehousesMovement.cs`

Responsabilidades:

- Ejecutar el validator.
- Ejecutar reglas de negocio.
- Abrir conexión y transacción.
- Mapear DTO a entidad.
- Persistir datos mediante repositorios.
- Registrar auditoría.
- Confirmar o revertir la transacción.

Los casos de uso no contienen reglas de negocio; delegan esa responsabilidad en las clases `BusinessRules`.

### Repositories

Archivos:

- `Infrastructure/Repositories/Logistic/InventoryStockRepository.cs`
- `Infrastructure/Repositories/Logistic/InventoryKardexRepository.cs`
- `Infrastructure/Repositories/Logistic/WarehousesMovementRepository.cs`

Responsabilidades:

- Ejecutar procedimientos almacenados o queries SQL.
- Traducir errores SQL a excepciones de infraestructura o negocio.
- Mantener las operaciones dentro de la transacción recibida.

## Flujo de Registro de InventoryStock

1. El cliente llama a:

   ```http
   POST api/InventoryStock/InventoryStockCreate
   ```

2. `InventoryStockController` obtiene:

   - `userId`
   - `businessId`

3. `CreateInventoryStock.ExecuteAsync` ejecuta `InventoryStockCreateValidator`.

4. `InventoryStockBusinessRules.ValidateForCreateAsync` valida:

   - producto;
   - almacén;
   - duplicidad de stock;
   - coherencia de costos.

5. Se abre una transacción.

6. Se mapea el DTO a `InventoryStock`.

7. Si hay stock inicial y no se envía fecha de entrada, se asigna `DateTime.Now` a `LastEnteryDate`.

8. `InventoryStockRepository.AddAsync` ejecuta:

   ```text
   SP_WS_REGISTER_INVENTORY_STOCK
   ```

9. Se registra auditoría con:

   ```text
   TableNames.InventoryStock = INVENTORY_STOCK
   ```

10. Se confirma la transacción.

## Flujo de Registro de WarehousesMovement

1. El cliente llama a:

   ```http
   POST api/WarehousesMovement/WarehousesMovementCreate
   ```

2. `WarehousesMovementController` obtiene:

   - `userId`
   - `businessId`

3. `CreateWarehousesMovement.ExecuteAsync` ejecuta `WarehousesMovementCreateValidator`.

4. `WarehousesMovementBusinessRules.ValidateForCreateAsync` valida reglas de negocio y devuelve un `StockOperation`.

5. Se abre una transacción.

6. Se mapea el DTO a `WarehousesMovement`.

7. Se calcula:

   ```text
   SubTotal = SUM(Quantity * UnitCost)
   Total = SubTotal + Igv
   ```

8. `WarehousesMovementRepository.AddAsync` ejecuta:

   ```text
   SP_WS_REGISTER_WAREHOUSES_MOVEMENT
   ```

9. Se registra auditoría de cabecera con:

   ```text
   TableNames.WarehousesMovement = WAREHOUSES_MOVEMENT
   ```

10. Por cada detalle:

    - se mapea a `WarehouseMovementDetail`;
    - se calcula `TotalCost = Quantity * UnitCost`;
    - se registra el detalle;
    - se audita el detalle;
    - se aplica stock;
    - se registra `InventoryKardex`.

11. `WarehousesMovementRepository.AddDetailAsync` ejecuta:

    ```text
    SP_WS_REGISTER_WAREHOUSES_MOVEMENT_DETAIL
    ```

12. Se registra auditoría del detalle con:

    ```text
    TableNames.WarehousesMovementDetail = WAREHOUSES_MOVEMENT_DETAIL
    ```

13. Se actualiza `InventoryStock` según el tipo de operación.

14. Se registra `InventoryKardex` con stock previo, entrada/salida y stock final.

15. Se confirma la transacción.

## Actualización de Stock

La actualización de stock ocurre en `WarehousesMovementStockService.ApplyAsync`.

### Entrada

Operación:

```text
StockOperation.Entry
```

Acción:

- Incrementa stock en `movement.WarehouseId`.
- Si no existe stock para el producto en ese almacén, lo crea.
- Actualiza:
  - `StockQuantity`
  - `AverageCost`
  - `LastCost`
  - `LastEnteryDate`

Repositorio:

```csharp
IInventoryStockRepository.IncreaseAsync(...)
```

### Salida

Operación:

```text
StockOperation.Output
```

Acción:

- Disminuye stock en `movement.WarehouseId`.
- Valida que exista stock.
- Valida stock suficiente.
- Actualiza:
  - `StockQuantity`
  - `LastOutputDate`

Repositorio:

```csharp
IInventoryStockRepository.DecreaseAsync(...)
```

Actualmente `allowNegative` se envía como `false`, por lo que no se permiten salidas con stock insuficiente.

### Transferencia

Operación:

```text
StockOperation.Transfer
```

Acción:

1. Disminuye stock en `movement.WarehouseId`.
2. Incrementa stock en `movement.WarehouseDestinationId`.

Ambas operaciones ocurren dentro de la misma transacción.

Si falla la salida del almacén origen o la entrada al almacén destino, se revierte todo el movimiento.

## Registro de InventoryKardex

El Kardex se registra desde `WarehousesMovementStockService`, despues de aplicar la actualizacion de `InventoryStock`.

Para cada afectacion de stock se ejecuta este flujo:

1. Se lee el stock actual antes del movimiento con `GetByProductAsync`.
2. Se ejecuta `IncreaseAsync` o `DecreaseAsync`.
3. Se vuelve a leer el stock final con `GetByProductAsync`.
4. Se crea `InventoryKardex` con:

   ```text
   PreviousStock = stock antes del movimiento
   EntryQuantity = cantidad de entrada, si aplica
   ExitQuantity = cantidad de salida, si aplica
   FinalStock = stock despues del movimiento
   AverageCost = costo promedio final del stock
   ```

5. `InventoryKardexRepository.AddAsync` ejecuta:

   ```text
   SP_WS_REGISTER_INVENTORY_KARDEX
   ```

6. Se registra auditoria con:

   ```text
   TableNames.InventoryKardex = INVENTORY_KARDEX
   ```

### Kardex en Entrada

Genera una linea de Kardex en el almacen origen del movimiento:

```text
EntryQuantity = detail.Quantity
ExitQuantity = 0
PreviousStock = stock previo del almacen
FinalStock = stock final del almacen
```

### Kardex en Salida

Genera una linea de Kardex en el almacen origen del movimiento:

```text
EntryQuantity = 0
ExitQuantity = detail.Quantity
PreviousStock = stock previo del almacen
FinalStock = stock final del almacen
```

### Kardex en Transferencia

Genera dos lineas de Kardex:

1. Salida del almacen origen.
2. Entrada al almacen destino.

Ambas lineas quedan asociadas al mismo `WareHouseMovementId` y `WareHouseMovementDetailId`, pero con diferente `WarehouseId`.

## Cómo se determina Entry, Output o Transfer

La clase `WarehousesMovementBusinessRules` resuelve el tipo de operación así:

1. Si `MovementTypes.RequiresDestWare` es `true`, se considera `Transfer`.
2. Si la descripción de `MovOper` contiene `transfer` o `traslado`, se considera `Transfer`.
3. Si la descripción contiene `entrada`, `ingreso` o `compra`, se considera `Entry`.
4. Si la descripción contiene `salida`, `egreso`, `venta` o `consumo`, se considera `Output`.
5. Si no se puede determinar la operación, se rechaza el movimiento.

Esto evita modificar inventario cuando el tipo de movimiento es ambiguo.

## Procedimientos Almacenados Esperados

### InventoryStock

```text
SP_WS_REGISTER_INVENTORY_STOCK
SP_WS_INVENTORY_STOCK_BY_ID
SP_WS_INVENTORY_STOCK_BY_PRODUCT
SP_WS_INCREASE_INVENTORY_STOCK
SP_WS_DECREASE_INVENTORY_STOCK
```

### WarehousesMovement

```text
SP_WS_REGISTER_WAREHOUSES_MOVEMENT
SP_WS_REGISTER_WAREHOUSES_MOVEMENT_DETAIL
```

### InventoryKardex

```text
SP_WS_REGISTER_INVENTORY_KARDEX
```

Los SPs de incremento y disminucion de stock deben controlar concurrencia internamente, por ejemplo con bloqueos sobre el registro de `INVENTORY_STOCK` afectado. Si el SP usa `RAISERROR`, el repository captura errores de negocio con numeros `50000`, `51000`, `51001` y `51002`.

## Auditoría

Tablas auditadas:

```text
INVENTORY_STOCK
INVENTORY_KARDEX
WAREHOUSES_MOVEMENT
WAREHOUSES_MOVEMENT_DETAIL
```

Constantes:

```csharp
TableNames.InventoryStock
TableNames.InventoryKardex
TableNames.WarehousesMovement
TableNames.WarehousesMovementDetail
```

La auditoría se registra dentro de la misma transacción del flujo.

## Inyección de Dependencias

### WarehousesMovement

Archivo:

```text
DependencyInjection/Dependency/Modules/WarehousesInjection.cs
```

Registra:

- `CreateWarehousesMovement`
- `WarehousesMovementBusinessRules`
- `IValidator<WarehousesMovementCreateDto>`
- `IWarehousesMovement`

### InventoryStock

Archivo:

```text
DependencyInjection/Dependency/Modules/Logistic/InventoryStockInjection.cs
```

Registra:

- `CreateInventoryStock`
- `InventoryStockBusinessRules`
- `IValidator<InventoryStockCreateDto>`
- `IInventoryStockRepository`
- `IInventoryKardexRepository`

El módulo se agrega en:

```text
DependencyInjection/Dependency/ServiceExtensions/ModuleRegistrationDI.cs
```

## Consideraciones Importantes

1. `InventoryStock` debe tener columna `WAREHOUSE_ID` en base de datos.
2. La combinación `BUSINESS_ID + WAREHOUSE_ID + PRODUCTS_ID` debería tener restricción única.
3. Las salidas y transferencias no permiten stock negativo.
4. La clasificación del movimiento depende de `MovementTypes.RequiresDestWare` y de la descripción de `MovOper`.
5. Si se requiere una clasificación más robusta, conviene agregar un campo explícito en `MovementTypes`, por ejemplo:

   ```text
   STOCK_OPERATION = ENTRY | OUTPUT | TRANSFER
   ```

6. La actualización de stock ocurre después de registrar y auditar el detalle, pero antes del `Commit`.
7. El Kardex se registra despues de actualizar stock, porque necesita conocer `PreviousStock` y `FinalStock`.
8. En transferencias se generan dos registros de Kardex: uno por la salida del almacen origen y otro por la entrada al almacen destino.
9. Si cualquier paso falla, el `Rollback` revierte cabecera, detalle, auditoría, stock y Kardex.

## Resumen de Transacción

```text
BEGIN TRANSACTION
  Validar DTO
  Validar reglas de negocio
  Insertar WarehousesMovement
  Auditar cabecera
  Por cada detalle:
    Insertar WarehouseMovementDetail
    Auditar detalle
    Actualizar InventoryStock
    Insertar InventoryKardex
    Auditar Kardex
COMMIT
```

Si ocurre un error:

```text
ROLLBACK
```
