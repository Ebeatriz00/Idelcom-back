# Flujo de Inventario Físico (Toma de Inventario)

## Estados del inventario

| Código | Descripción | Transiciones permitidas |
|--------|-------------|------------------------|
| `DRAFT` | Borrador | → COUNTING, → CANCELLED |
| `COUNTING` | En conteo | → CLOSED, → CANCELLED |
| `CLOSED` | Cerrado | → ADJUSTED, → CANCELLED |
| `ADJUSTED` | Ajustado | (terminal) |
| `CANCELLED` | Anulado | (terminal) |

---

## Endpoints

| Método | Ruta | Acción |
|--------|------|--------|
| `POST` | `/api/inventory-count/create` | Crear toma en estado DRAFT |
| `POST` | `/api/inventory-count/start/{id}` | Iniciar conteo → COUNTING |
| `PUT` | `/api/inventory-count/update-details` | Registrar cantidades contadas |
| `POST` | `/api/inventory-count/close/{id}` | Cerrar toma → CLOSED |
| `POST` | `/api/inventory-count/generate-adjustments/{id}` | Generar ajustes → ADJUSTED |
| `POST` | `/api/inventory-count/cancel/{id}` | Anular toma → CANCELLED |
| `GET` | `/api/inventory-count/list` | Listar tomas con filtros |
| `GET` | `/api/inventory-count/get/{id}` | Obtener cabecera + detalle |

---

## Paso 1 — Crear toma de inventario

**Endpoint:** `POST /api/inventory-count/create`

**Body:**
```json
{
  "warehouseId": 1,
  "countDate": "2026-05-18",
  "observation": "Toma mensual de mayo"
}
```

**Qué hace:**
- Genera un `COUNT_NUMBER` único por empresa (ej. `IC-000001`)
- Guarda la cabecera en `INVENTORY_COUNT` con estado `DRAFT`
- No toca stock

**Respuesta:**
```json
{ "id": 1, "status": 1, "message": "Toma de inventario creada correctamente." }
```

---

## Paso 2 — Iniciar conteo

**Endpoint:** `POST /api/inventory-count/start/{inventoryCountId}`

**Qué hace:**
- Valida que el estado sea `DRAFT`
- Carga todos los productos activos y stockeables del almacén desde `INVENTORY_STOCK`
- Crea filas en `INVENTORY_COUNT_DETAIL` con:
  - `SYSTEM_QUANTITY` = stock actual del sistema
  - `COUNTED_QUANTITY` = 0 (a rellenar manualmente)
  - `UNIT_COST` = costo promedio actual
- Cambia estado a `COUNTING`
- **No modifica stock**

**Regla:** Solo se puede iniciar desde `DRAFT`. Si ya está en `COUNTING`, `CLOSED`, `ADJUSTED` o `CANCELLED`, devuelve error.

---

## Paso 3 — Registrar cantidades contadas

**Endpoint:** `PUT /api/inventory-count/update-details`

**Body:**
```json
{
  "inventoryCountId": 1,
  "details": [
    {
      "inventoryCountDetailId": 10,
      "countedQuantity": 25.00,
      "lotNumber": null,
      "serialNumber": null,
      "expirationDate": null,
      "observation": "Contado por almacenero"
    },
    {
      "inventoryCountDetailId": 11,
      "countedQuantity": 8.00
    }
  ]
}
```

**Qué hace:**
- Valida que el estado sea `COUNTING`
- Actualiza `COUNTED_QUANTITY`, `LOT_NUMBER`, `SERIAL_NUMBER`, `EXPIRATION_DATE`, `OBSERVATION` por producto
- La columna `DIFFERENCE_QUANTITY` es calculada automáticamente por SQL: `COUNTED_QUANTITY - SYSTEM_QUANTITY`
- La columna `TOTAL_DIFFERENCE_COST` es calculada automáticamente: `DIFFERENCE_QUANTITY * UNIT_COST`
- **No modifica stock**
- Puede llamarse múltiples veces (parcialmente o completo)

**Regla:** No se puede editar si el estado no es `COUNTING`.

---

## Paso 4 — Cerrar toma

**Endpoint:** `POST /api/inventory-count/close/{inventoryCountId}`

**Qué hace:**
- Valida que el estado sea `COUNTING`
- Cambia estado a `CLOSED`
- A partir de este punto ya no se pueden modificar cantidades
- **No genera ajustes ni modifica stock**

---

## Paso 5 — Generar ajustes

**Endpoint:** `POST /api/inventory-count/generate-adjustments/{inventoryCountId}`

**Qué hace (todo en una sola transacción):**

1. Valida que el estado sea `CLOSED`
2. Obtiene todos los detalles con `DIFFERENCE_QUANTITY <> 0`
3. Si no hay diferencias → error: *"No existen diferencias para ajustar"*
4. Si ya hay `ADJUSTMENT_MOVEMENT_ID` en algún detalle → error: *"La toma ya fue ajustada"*
5. Separa los detalles en dos grupos:

| Grupo | Condición | Tipo de movimiento | Operación |
|-------|-----------|-------------------|-----------|
| Sobrantes | `DIFFERENCE_QUANTITY > 0` | Código `009` — Ajuste sobrante | Entry (suma stock) |
| Faltantes | `DIFFERENCE_QUANTITY < 0` | Código `015` — Ajuste faltante | Output (resta stock) |

6. Por cada grupo crea **un** `WAREHOUSE_MOVEMENT` con todos los productos del grupo como detalles
7. Por cada detalle del movimiento:
   - Crea `WAREHOUSE_MOVEMENT_DETAIL`
   - Actualiza `INVENTORY_STOCK` (aumento o disminución)
   - Registra `INVENTORY_KARDEX`
8. Guarda el `WAREHOUSE_MOVEMENT_ID` en `INVENTORY_COUNT_DETAIL.ADJUSTMENT_MOVEMENT_ID`
9. Cambia estado a `ADJUSTED`

> **Nota:** La cantidad usada en los movimientos de ajuste es siempre positiva (`ABS(DIFFERENCE_QUANTITY)`). El tipo de movimiento determina si suma o resta stock.

**Reglas:**
- Si el stock disponible es insuficiente para un faltante y `ALLOW_NEGATIVE = 0`, el SP lanza error y se hace **rollback completo**.
- Si falla cualquier producto, se revierte **toda** la transacción.

---

## Paso 6 — Anular toma

**Endpoint:** `POST /api/inventory-count/cancel/{inventoryCountId}`

**Qué hace:**
- Cambia el estado a `CANCELLED`
- **No revierte movimientos de ajuste** (si ya fue ajustada no se puede anular)

**Reglas:**
- Permitido desde: `DRAFT`, `COUNTING`, `CLOSED`
- **No permitido** si ya está `ADJUSTED`
- **No permitido** si ya está `CANCELLED`

---

## Diagrama de estados

```
         create
[inicio] ──────────> [DRAFT]
                        │
                     start│
                        ▼
                   [COUNTING]
                        │
                      close│
                        ▼
                    [CLOSED]
                        │
            generate-adjustments│
                        ▼
                   [ADJUSTED] (terminal)

[DRAFT] ──cancel──> [CANCELLED]
[COUNTING] ─cancel─> [CANCELLED]
[CLOSED] ──cancel──> [CANCELLED]
```

---

## Estructura de tablas

### INVENTORY_COUNT (cabecera)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `INVENTORY_COUNT_ID` | BIGINT PK | Identificador |
| `BUSINESS_ID` | BIGINT | Empresa |
| `WAREHOUSE_ID` | BIGINT | Almacén |
| `COUNT_NUMBER` | VARCHAR(30) | Número único ej. `IC-000001` |
| `COUNT_DATE` | DATE | Fecha del conteo |
| `INVENTORY_COUNT_STATUS_ID` | BIGINT FK | Estado actual |
| `OBSERVATION` | NVARCHAR(500) | Observación general |

### INVENTORY_COUNT_DETAIL (detalle por producto)
| Campo | Tipo | Descripción |
|-------|------|-------------|
| `INVENTORY_COUNT_DETAIL_ID` | BIGINT PK | Identificador |
| `PRODUCTS_ID` | BIGINT | Producto |
| `SYSTEM_QUANTITY` | DECIMAL(18,4) | Stock del sistema al iniciar |
| `COUNTED_QUANTITY` | DECIMAL(18,4) | Cantidad contada físicamente |
| `DIFFERENCE_QUANTITY` | DECIMAL computed | `COUNTED - SYSTEM` |
| `UNIT_COST` | DECIMAL(18,4) | Costo unitario (costo promedio) |
| `TOTAL_DIFFERENCE_COST` | DECIMAL computed | `DIFFERENCE * UNIT_COST` |
| `LOT_NUMBER` | NVARCHAR(100) | Lote (si aplica) |
| `SERIAL_NUMBER` | NVARCHAR(100) | Serie (si aplica) |
| `EXPIRATION_DATE` | DATE | Vencimiento (si aplica) |
| `ADJUSTMENT_MOVEMENT_ID` | BIGINT FK | Movimiento de ajuste generado |

---

## Tipos de movimiento requeridos en BD

Deben existir en `MOVEMENT_TYPES` para la empresa correspondiente:

| Código | Descripción | MOV_OPER_ID | IS_ADJUSTMENT | ALLOW_NEGATIVE |
|--------|-------------|:-----------:|:-------------:|:--------------:|
| `009` | AJUSTE POR INVENTARIO FÍSICO (SOBRANTE) | 1 (Entry) | 1 | 0 |
| `015` | AJUSTE POR INVENTARIO FÍSICO (FALTANTE) | 2 (Output) | 1 | 0 |

> Script de inserción: `Database/Scripts/AdjustmentMovementTypes.sql`

---

## Scripts SQL a ejecutar (en orden)

```
1. Database/Scripts/Inventario Físico.SQL    → Crea tablas e inserta estados
2. Database/Scripts/AdjustmentMovementTypes.sql → Inserta tipos de movimiento '009' y '015'
3. Database/Scripts/InventoryCount.sql       → Crea TVPs y todos los Stored Procedures
```

---

## Stored Procedures

| SP | Acción |
|----|--------|
| `SP_WS_GET_NEXT_INVENTORY_COUNT_NUMBER` | Genera número correlativo `IC-XXXXXX` |
| `SP_WS_GET_INVENTORY_COUNT_STATUS_ID` | Busca ID de estado por código |
| `SP_WS_CREATE_INVENTORY_COUNT` | Crea cabecera en DRAFT |
| `SP_WS_START_INVENTORY_COUNT` | Carga stock y pasa a COUNTING |
| `SP_WS_UPDATE_INVENTORY_COUNT_DETAILS` | Actualiza cantidades (TVP) |
| `SP_WS_CLOSE_INVENTORY_COUNT` | Cierra toma → CLOSED |
| `SP_WS_CANCEL_INVENTORY_COUNT` | Anula toma → CANCELLED |
| `SP_WS_MARK_INVENTORY_COUNT_AS_ADJUSTED` | Guarda movimientos y pasa a ADJUSTED (TVP) |
| `SP_WS_LIST_INVENTORY_COUNT` | Listado paginado con filtros |
| `SP_WS_GET_INVENTORY_COUNT_BY_ID` | Cabecera + detalle completo |

---

## Validaciones de negocio

| Condición | Mensaje |
|-----------|---------|
| WarehouseId = 0 | *"Debe seleccionar un almacén."* |
| Registro no encontrado | *"La toma de inventario no existe."* |
| Iniciar desde estado no DRAFT | *"La toma de inventario no se encuentra en estado válido para iniciar conteo."* |
| Editar desde estado no COUNTING | *"La toma de inventario no se encuentra en estado válido para editar."* |
| Generar ajuste sin estar CLOSED | *"La toma de inventario debe estar cerrada para generar ajustes."* |
| Sin diferencias para ajustar | *"No existen diferencias para ajustar."* |
| Ya tiene adjustment_movement_id | *"La toma de inventario ya fue ajustada."* |
| Anular una toma ADJUSTED | *"No se puede anular una toma de inventario ya ajustada."* |
| Stock insuficiente (ALLOW_NEGATIVE=0) | Error en SP → rollback completo |
