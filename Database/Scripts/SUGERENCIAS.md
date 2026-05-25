# Sugerencias para el Proyecto

Este documento resume recomendaciones practicas para mantener el proyecto alineado con Clean Architecture, mejorar la mantenibilidad y reducir riesgos al agregar nuevas funcionalidades.

## Flujo Actual

El flujo general de una operacion en la API es:

1. La solicitud llega a un controlador en `GlueMark/Controllers`.
2. El controlador recibe el request, valida el contrato HTTP basico y delega la operacion.
3. El caso de uso o servicio en `Application/` ejecuta la orquestacion principal.
4. Los DTOs de entrada se validan con FluentValidation antes de llamar a persistencia.
5. `Application/` consume interfaces definidas en `Core/Interfaces`.
6. `Infrastructure/` implementa esas interfaces y ejecuta la persistencia con Dapper, helpers compartidos o stored procedures.
7. Si el flujo requiere auditoria, se usan los componentes relacionados con `[AuditField]`, `IAuditLogFactory` y `IAuditService`.
8. Los datos se convierten entre entidades, proyecciones, resultados y DTOs usando mapeos explicitos o perfiles de AutoMapper.
9. El resultado vuelve al controlador y se entrega al cliente con el formato de respuesta correspondiente.

El motor de sugerencias logisticas vive en `QuotationLogisticsSuggestion` y se apoya en reglas configuradas en `LOGISTICS_SUGGESTION_RULE`. La API no calcula las coincidencias en memoria; valida el request, arma comandos/filtros y delega la logica de generacion a procedimientos almacenados.

## Flujo del Motor de Sugerencias

El controlador principal es `QuotationLogisticsSuggestionController` y expone estos endpoints:

- `POST api/QuotationLogisticsSuggestion/Generate`: genera sugerencias para una cotizacion.
- `GET api/QuotationLogisticsSuggestion/List`: lista sugerencias generadas con filtros.
- `PUT api/QuotationLogisticsSuggestion/Update`: selecciona o actualiza cantidad aprobada y observacion.
- `POST api/QuotationLogisticsSuggestion/AddManual`: agrega una sugerencia manual.
- `PATCH api/QuotationLogisticsSuggestion/Disable/{id}`: desactiva una sugerencia.

### Generacion Automatica

El flujo de generacion automatica es:

1. El cliente envia `QuotationId` y opcionalmente `QuotationVerId`.
2. El controller obtiene `userId` y `businessId` desde el contexto autenticado.
3. `GenerateQuotationLogisticsSuggestionsUseCase` ejecuta `GenerateQuotationLogisticsSuggestionValidator`.
4. El caso de uso normaliza `QuotationVerId`: valores nulos o menores/iguales a cero se tratan como `null`.
5. Se llama a `IQuotationLogisticsSuggestionRepository.GenerateAsync`.
6. El repositorio ejecuta `SP_WS_GENERATE_QUOTATION_LOGISTICS_SUGGESTIONS`.
7. El procedimiento valida que la cotizacion pertenezca a la empresa.
8. Si no llega version, resuelve la version seleccionada o activa de la cotizacion.
9. Valida que la version tenga lineas registradas.
10. Cruza las lineas de cotizacion con reglas activas de `LOGISTICS_SUGGESTION_RULE`.
11. Genera candidatos cuando la regla coincide por condiciones como palabra clave, tipo de producto, sistema o tipo de linea.
12. Calcula la cantidad sugerida usando `DEFAULT_QUANTITY` y `QUANTITY_FACTOR`.
13. Evita duplicados contra `QUOTATION_LOGISTICS_SUGGESTION` usando cotizacion, version, linea, regla, producto o descripcion.
14. Inserta solo las sugerencias nuevas en `QUOTATION_LOGISTICS_SUGGESTION`.
15. Devuelve `CreatedCount`, `ExistingCount`, `TotalActiveCount` y `Message`.

### Logica de Reglas

Cada regla en `LOGISTICS_SUGGESTION_RULE` define que recurso logistico se debe sugerir y bajo que condiciones aplica:

- `BUSINESS_ID`: limita la regla a una empresa.
- `RULE_NAME`: nombre funcional de la regla.
- `KEYWORD`: texto usado para comparar contra la descripcion de la linea.
- `PRODUCTS_TYPE_ID`: restringe la regla por tipo de producto.
- `SYSTEM_ID`: restringe la regla por sistema.
- `LINE_TYPE`: restringe la regla por tipo de linea.
- `LOGISTICS_RESOURCE_TYPE_ID`: tipo de recurso sugerido, por ejemplo equipo, herramienta o personal.
- `SUGGESTED_PRODUCTS_ID`: producto sugerido cuando existe un producto maestro.
- `SUGGESTED_DESCRIPTION`: descripcion sugerida cuando no depende de un producto.
- `DEFAULT_QUANTITY`: cantidad base.
- `QUANTITY_FACTOR`: factor para calcular cantidad segun la cantidad de la linea.
- `IS_REQUIRED`: marca si la sugerencia es obligatoria.
- `REQUIRES_REVIEW`: indica si necesita revision de oficina.

### Listado

El listado usa `ListQuotationLogisticsSuggestionsUseCase` y `SP_WS_LIST_QUOTATION_LOGISTICS_SUGGESTIONS`.

Filtros disponibles:

- `QuotationId`: obligatorio.
- `QuotationVerId`: opcional.
- `ResourceTypeId`: opcional.
- `OnlySelected`: opcional.
- `Search`: opcional, maximo 100 caracteres.

El resultado incluye datos de la sugerencia, linea de cotizacion, tipo de recurso, producto, cantidad sugerida, cantidad aprobada, seleccion, origen manual, duplicado, motivo, observacion y estado.

### Actualizacion de Sugerencias

La actualizacion usa `UpdateQuotationLogisticsSuggestionUseCase` y `SP_WS_UPDATE_QUOTATION_LOGISTICS_SUGGESTION`.

Reglas principales:

- `QuotationLogisticsSuggestionId` es obligatorio.
- `ApprovedQuantity` no puede ser negativa.
- Si `IsSelected` es `true`, `ApprovedQuantity` debe ser mayor a cero.
- `OfficeObservation` no debe exceder 500 caracteres.
- El procedimiento valida que la sugerencia exista, pertenezca a la empresa y este activa.
- Al actualizar, guarda seleccion, cantidad aprobada, observacion, usuario revisor y fecha de revision.

### Sugerencias Manuales

La creacion manual usa `AddManualQuotationLogisticsSuggestionUseCase` y `SP_WS_ADD_MANUAL_QUOTATION_LOGISTICS_SUGGESTION`.

Reglas principales:

- `QuotationId`, `QuotationVerId` y `LogisticsResourceTypeId` son obligatorios.
- `ProductsId` es opcional, pero si no se informa producto, `Description` es obligatoria.
- `SuggestedQuantity` debe ser mayor a cero.
- `ApprovedQuantity` no puede ser negativa.
- La sugerencia se inserta con `IS_MANUAL = 1`.
- La respuesta devuelve el `Id` creado, `Status` y `Message`.

### Desactivacion

La desactivacion usa `DisableQuotationLogisticsSuggestionUseCase` y `SP_WS_DISABLE_QUOTATION_LOGISTICS_SUGGESTION`.

Reglas principales:

- `businessId` y `suggestionId` deben ser mayores a cero.
- El procedimiento valida que la sugerencia exista, pertenezca a la empresa y este activa.
- La sugerencia se marca con `STATUS = 0` e `IS_SELECTED = 0`.
- Se actualizan `UPDATE_USER` y `UPDATE_DATE`.

### Tablas y Procedimientos

Tablas principales:

- `LOGISTICS_RESOURCE_TYPE`: catalogo de tipos de recursos logisticos.
- `LOGISTICS_SUGGESTION_RULE`: reglas que disparan sugerencias.
- `QUOTATION_LOGISTICS_SUGGESTION`: sugerencias generadas o manuales para una cotizacion.

Procedimientos principales:

- `SP_WS_GENERATE_QUOTATION_LOGISTICS_SUGGESTIONS`
- `SP_WS_LIST_QUOTATION_LOGISTICS_SUGGESTIONS`
- `SP_WS_UPDATE_QUOTATION_LOGISTICS_SUGGESTION`
- `SP_WS_ADD_MANUAL_QUOTATION_LOGISTICS_SUGGESTION`
- `SP_WS_DISABLE_QUOTATION_LOGISTICS_SUGGESTION`

## Saldo Pendiente por Solicitar

### Problema que resuelve

Cuando una cotizacion tiene una cantidad calculada por regla y ya se creo una solicitud logistica por parte de esa cantidad, volver a generar sugerencias no debe repetir el total original. Debe mostrar solo el saldo pendiente.

Ejemplo: cotizacion con 13 switches, solicitud creada por 10 → al regenerar, la sugerencia debe ser 3, no 13.

### Campos agregados en QUOTATION_LOGISTICS_SUGGESTION

| Columna | Tipo | Descripcion |
|---|---|---|
| `SUGGESTED_QUANTITY_BASE` | DECIMAL(18,4) NULL | Cantidad calculada por regla (referencia invariable) |
| `ALREADY_REQUESTED_QUANTITY` | DECIMAL(18,4) DEFAULT 0 | Suma de lo ya enviado a solicitudes logisticas activas |
| `PENDING_TO_REQUEST_QUANTITY` | DECIMAL(18,4) DEFAULT 0 | Saldo pendiente = base - ya solicitado |
| `EXCESS_REQUESTED_QUANTITY` | DECIMAL(18,4) DEFAULT 0 | Excedente si ya se solicito mas de lo calculado |
| `IS_FULLY_REQUESTED` | BIT DEFAULT 0 | 1 cuando el saldo pendiente es <= 0 |

El script de migracion esta en `Database/Scripts/SugerenciasLogisticasSaldoPendiente.sql`.

### Logica de calculo

```
PENDING_TO_REQUEST = SUGGESTED_QUANTITY_BASE - ALREADY_REQUESTED_QUANTITY
EXCESS             = ALREADY_REQUESTED_QUANTITY - SUGGESTED_QUANTITY_BASE  (si > 0, si no 0)
IS_FULLY_REQUESTED = PENDING_TO_REQUEST <= 0
```

`ALREADY_REQUESTED_QUANTITY` suma `APPROVED_QUANTITY` (o `REQUESTED_QUANTITY` si no existe) de `LOGISTICS_REQUEST_DETAIL` con estados activos. No se usa `ATTENDED_QUANTITY`.

Estados que NO suman: `CANCELLED`, `REJECTED`, `ANULADA`, `RECHAZADA`.

### Comportamiento al generar (SP_WS_GENERATE_QUOTATION_LOGISTICS_SUGGESTIONS)

1. Calcula `SUGGESTED_QUANTITY_BASE` por regla (factor × QTY o DEFAULT_QUANTITY).
2. Calcula `ALREADY_REQUESTED_QUANTITY` desde `LOGISTICS_REQUEST_DETAIL` activo vinculado por `SOURCE_SUGGESTION_ID` o `SOURCE_QUOTATION_VER_LIN_ID`.
3. Si no existe sugerencia y `PENDING > 0`: inserta nueva con `SUGGESTED_QUANTITY = PENDING`.
4. Si existe sugerencia y `PENDING > 0`: actualiza cantidades y deja `IS_SELECTED = 1`.
5. Si existe sugerencia y `PENDING <= 0`: marca `IS_FULLY_REQUESTED = 1`, `IS_SELECTED = 0`. No crea duplicado.
6. Devuelve `FullyRequestedCount` y `PendingTotalCount` ademas de los campos previos.

### Campos nuevos en LOGISTICS_REQUEST_DETAIL

Se agregan condicionalmente si no existen:

- `SOURCE_SUGGESTION_ID`: FK a `QUOTATION_LOGISTICS_SUGGESTION` para rastrear de que sugerencia viene el detalle.
- `SOURCE_QUOTATION_VER_LIN_ID`: referencia a la linea de cotizacion origen.

### Validacion en actualizacion (SP_WS_UPDATE_QUOTATION_LOGISTICS_SUGGESTION)

- No se puede seleccionar una sugerencia con `IS_FULLY_REQUESTED = 1`.
- `APPROVED_QUANTITY` no puede superar `PENDING_TO_REQUEST_QUANTITY`.
- La misma validacion existe en `UpdateQuotationLogisticsSuggestionValidator` a nivel C#.

### DTOs actualizados

`QuotationLogisticsSuggestionResponseDto` incluye:

- `SuggestedQuantityBase`
- `AlreadyRequestedQuantity`
- `PendingToRequestQuantity`
- `ExcessRequestedQuantity`
- `IsFullyRequested`

`QuotationLogisticsSuggestionGenerateResponseDto` incluye:

- `FullyRequestedCount`
- `PendingTotalCount`

`UpdateQuotationLogisticsSuggestionDto` incluye:

- `PendingToRequestQuantity` (para validacion en FluentValidation)

### Reglas para futura integracion con CreateFromSelectedSuggestions

Cuando se implemente la conversion de sugerencias a solicitudes logisticas:

- Solo tomar sugerencias con `IS_SELECTED = 1`, `STATUS = 1`, `APPROVED_QUANTITY > 0`, `PENDING_TO_REQUEST_QUANTITY > 0`, `IS_FULLY_REQUESTED = 0`.
- Validar que `APPROVED_QUANTITY <= PENDING_TO_REQUEST_QUANTITY`. Bloquear con mensaje si supera.
- Al insertar en `LOGISTICS_REQUEST_DETAIL`, guardar `SOURCE_SUGGESTION_ID` y `SOURCE_QUOTATION_VER_LIN_ID`.
- Despues de crear la solicitud, recalcular y actualizar los campos de saldo en la sugerencia.

## Arquitectura

- Mantener las nuevas funcionalidades como vertical slices: entidad, interfaz, DTOs, validador, perfil de mapeo, caso de uso o servicio, repositorio, registro DI y controlador.
- Evitar que `GlueMark/` contenga logica de negocio. Los controladores deben validar el contrato HTTP, delegar y devolver respuestas.
- Mantener las reglas de negocio en `Application/` o `Core/` segun corresponda, dejando `Infrastructure/` solo para detalles externos como base de datos, seguridad, notificaciones o integraciones.
- Revisar que las interfaces vivan en `Core/` cuando representen contratos del dominio o de persistencia consumidos por la aplicacion.

## DTOs, Validaciones y Mapeos

- Usar nombres especificos para DTOs y resultados, por ejemplo `CreateWarehouseMovementRequestDto`, `WarehouseMovementResult` o `OperationSnapshot`.
- Evitar nombres genericos como `Model`, `Data`, `Info` o `Response` cuando no expresen el proposito real.
- Agregar validadores con FluentValidation para todo request que llegue desde API antes de llamar repositorios o servicios externos.
- Centralizar conversiones repetidas en perfiles de AutoMapper cuando el mapeo sea estable y compartido.

## Persistencia

- Preferir `IDapperHelper`, `DapperParams` y `DbParam` cuando apliquen, para mantener consistencia en llamadas Dapper.
- Mantener los stored procedures con el prefijo `SP_WS_*`.
- Documentar en el repositorio el nombre del stored procedure usado por cada metodo cuando el flujo no sea evidente.
- Evitar construir SQL dinamico con concatenacion de strings; usar parametros para reducir riesgos de inyeccion y errores de formato.

## Auditoria y Seguridad

- Preservar los flujos que dependen de `[AuditField]`, `IAuditLogFactory` y `IAuditService`.
- Validar permisos y datos de entrada antes de ejecutar cambios persistentes.
- No incluir secretos, connection strings locales ni tokens en commits.
- Mantener configuraciones por ambiente en `appsettings.*` y revisar que los valores sensibles lleguen por variables de entorno o mecanismos seguros.

## Pruebas

- Agregar pruebas cuando se cambien validaciones, orquestacion, mapeos de repositorio o comportamiento de auditoria.
- Nombrar las pruebas con el nombre de la clase o flujo productivo que cubren, por ejemplo `UpdateOperationsTeamSsomaTests`.
- Cubrir al menos casos exitosos, validaciones principales y fallos relevantes de repositorio o dependencia externa.
- Ejecutar `dotnet test Idelcom.WebApi.sln` antes de cerrar cambios con impacto funcional.

## Pull Requests

- Incluir resumen claro, modulos afectados, validacion realizada y ejemplos de request/response cuando cambie la API.
- Usar commits convencionales como `feat(operations): add status audit` o `fix(auth): validate mobile session`.
- Mantener los PRs acotados a una funcionalidad o correccion para facilitar revision y pruebas.

## Checklist para Nuevas Funcionalidades

- Definir contrato de entrada y salida en `Application/DTOs`.
- Agregar validador del request.
- Crear o actualizar entidad, resultado o proyeccion en `Core/` si corresponde.
- Crear interfaz en `Core/Interfaces`.
- Implementar caso de uso o servicio en `Application/`.
- Implementar repositorio en `Infrastructure/`.
- Registrar dependencias en `DependencyInjection/`.
- Exponer endpoint en `GlueMark/Controllers`.
- Agregar pruebas relevantes en `Tests/`.
- Ejecutar build y pruebas antes del PR.
