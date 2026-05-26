# 📘 IDELCOM - Documentación del Proyecto

Este proyecto implementa una arquitectura Clean Architecture para **IDELCOM**, con una separación clara entre dominio, aplicación, infraestructura y API.

## 🏗️ Estructura del proyecto

```text
├── Application            # Casos de uso, DTOs, validaciones, perfiles de mapeo, servicios y contextos
│   ├── Contexts           # Objetos internos de proceso, auditoría u orquestación
│   ├── DTOs               # Data Transfer Objects de entrada y salida
│   ├── MappingProfiles    # Perfiles de AutoMapper
│   ├── Services           # Servicios de aplicación
│   ├── UseCases           # Casos de uso por módulo y acción
│   └── Validators         # Validaciones con FluentValidation
├── Core                   # Núcleo del dominio y contratos
│   ├── Entities           # Entidades del dominio
│   ├── Interfaces         # Interfaces de repositorios y servicios
│   ├── Projections        # Tipos de lectura o consulta
│   └── Results            # Resultados compuestos de flujos de negocio
├── DependencyInjection    # Registro de módulos y composición de dependencias
├── GlueMark               # API ASP.NET Core
│   ├── Controllers        # Controladores HTTP
│   ├── Extensions         # Extensiones y configuración de la API
│   └── Middleware         # Middlewares personalizados
├── Infrastructure         # Persistencia, seguridad, notificaciones y repositorios
│   ├── Persistence        # Conexión SQL, helpers de Dapper y soporte de persistencia
│   ├── Repositories       # Implementaciones de repositorios
│   ├── Notifications      # Componentes de notificaciones
│   └── Security           # Componentes de autenticación y seguridad
├── SharedKernel           # Respuestas globales, constantes, helpers y abstracciones compartidas
└── Tests                  # Pruebas unitarias e integración
```

## 🔄 Flujo general

1. La solicitud llega a un controlador en `GlueMark.Controllers`.
2. El controlador delega la operación a un caso de uso o servicio de `Application`.
3. La capa `Application` valida datos, orquesta la lógica y consume interfaces definidas en `Core`.
4. `Infrastructure` implementa esas interfaces y ejecuta la persistencia contra base de datos.
5. La persistencia usa `ISqlConnectionFactory` y, cuando aplica, utilidades compartidas como `IDapperHelper`, `DapperParams` y `DbParam`.
6. Los datos se mapean entre entidades, resultados, proyecciones y DTOs con AutoMapper cuando corresponde.
7. La respuesta vuelve al controlador y se entrega al cliente.

La composición principal de dependencias se registra desde `AddApplicationModules`, que centraliza módulos funcionales, persistencia y servicios de seguridad.

### 🔐 Flujo de autenticación App Móvil

La aplicación móvil usa un flujo separado del ERP/Web. No consume `/api/Auth/login`, no depende de cookies `HttpOnly` y no usa refresh token. El flujo móvil vive bajo `/api/app/auth/*` y trabaja con un JWT Bearer de larga duración, pensado para escenarios de conectividad limitada.

#### Login móvil

```http
POST /api/app/auth/login
Content-Type: application/json
```

Body:

```json
{
  "username": "usuario",
  "password": "clave",
  "deviceId": "uuid-dispositivo",
  "deviceName": "Samsung Galaxy S23"
}
```

El endpoint es público (`AllowAnonymous`) y delega en `AppLoginUseCase`. El caso de uso valida el request con `AppLoginRequestValidator`, normaliza el usuario, aplica control de intentos fallidos con `ILoginAttemptService`, valida la contraseña con `IPasswordService` y registra una sesión persistente del dispositivo en `SECURITY_APP_DEVICE_SESSION`.

Respuesta exitosa:

```json
{
  "status": 1,
  "message": "Autenticación móvil exitosa.",
  "accessToken": "...",
  "userId": 1,
  "fullName": "Nombre Apellido",
  "userName": "",
  "roleName": "Perfil",
  "businessId": 1
}
```

El `accessToken` móvil expira en 30 días y debe enviarse en las llamadas protegidas de la App:

```http
Authorization: Bearer <accessToken>
```

Claims principales del token móvil:

| Claim | Uso |
| ----- | --- |
| `sub` | Identificador del usuario autenticado |
| `jti` | Identificador único del token, usado para revocación |
| `sid` | Identificador de sesión generado para el login |
| `bid` | Identificador de empresa |
| `source` | Valor fijo `mobile`, requerido por la política App |
| `deviceId` | Identificador del dispositivo autenticado |

#### Validación y revocación

Después de validar firma, issuer, audience y expiración del JWT, `Program.cs` ejecuta `OnTokenValidated`. Si el claim `source` es `mobile`, la API consulta `IAppDeviceSessionRepository.IsRevokedAsync(jti)` contra la tabla `SECURITY_APP_DEVICE_SESSION`.

Si la sesión fue revocada o marcada inactiva, la autenticación falla y la API responde `401 Unauthorized`. Los endpoints exclusivos de la App deben protegerse con:

```csharp
[Authorize(Policy = "RequireAppAccess")]
```

La política `RequireAppAccess` exige el claim `source=mobile`, por lo que un token web del ERP no debe consumir endpoints móviles.

#### Logout móvil

```http
POST /api/app/auth/logout
Authorization: Bearer <accessToken>
```

El logout requiere `RequireAppAccess`. El controlador lee el `jti` del token actual y `AppLogoutUseCase` revoca la sesión con `SP_WS_REVOKE_APP_SESSION`, marcando `IS_REVOKED = 1`, `STATUS = 0` y `REVOKED_AT`.

Respuesta:

```json
{
  "status": 1,
  "message": "Sesión móvil cerrada exitosamente."
}
```

Si la sesión ya estaba cerrada o no existe, el endpoint responde con `status: 0` y el mensaje correspondiente.

#### Componentes del flujo

- Controller: `GlueMark/Controllers/App/AppAuthController.cs`
- DTOs: `Application/DTOs/AppAuth`
- Use cases: `Application/UseCases/AppAuth`
- Validator: `Application/Validators/AppAuth/AppLoginRequestValidator.cs`
- Repository: `Infrastructure/Repositories/Security/AppDeviceSessionRepository.cs`
- Registro DI: `DependencyInjection/Dependency/Modules/AppAuth/AppAuthInjection.cs`
- Persistencia SQL: `SQL/APP/*`

#### Diferencia con autenticación ERP/Web

El ERP/Web usa `/api/Auth/login`, emite cookies `HttpOnly` (`accessToken` y `refreshToken`), rota refresh tokens y valida listas negras o sesiones web. La App usa `/api/app/auth/login`, recibe el token en el body, lo envía como Bearer token y se revoca mediante sesiones persistentes por dispositivo.

### 🧱 Orden recomendado para crear un módulo

1. Entidad en `Core.Entities`
2. Interfaces en `Core.Interfaces`
3. DTOs en `Application.DTOs`
4. Validadores en `Application.Validators`
5. Perfiles de mapeo en `Application.MappingProfiles`
6. Casos de uso o servicios en `Application.UseCases` o `Application.Services`
7. Implementación de repositorio en `Infrastructure.Repositories`
8. Registro en `DependencyInjection`
9. Controlador en `GlueMark.Controllers`

## ✏️ Convenciones de nombres

| Tipo | Convención | Ejemplo |
| ---- | ---------- | ------- |
| Clases | PascalCase | `DocumentTypeRepository` |
| Interfaces | Prefijo `I` + PascalCase | `IDocumentTypeRepository` |
| Métodos | PascalCase | `GetAllAsync`, `ExecuteAsync` |
| Variables | camelCase | `businessId`, `documentTypeId` |
| DTOs | Sufijo `Dto`, PascalCase | `DocumentTypeCreateDto` |
| Resultados | Sufijo `Result`, PascalCase | `OperationsTeamSsomaCreateResult` |
| Proyecciones o ítems | Sufijo `Projection` o `Item`, PascalCase | `OperationsTeamSsomaDetailProjection` |
| Contextos internos | Sufijo `Context` o `Snapshot`, PascalCase | `OperationsTeamSsomaAuditSnapshot` |
| Persistencia técnica | Sufijo `Row`, PascalCase | `OperationsTeamSsomaRow` |
| Rutas de API | `api/[controller]` + segmentos por acción | `/api/DocumentTypes/DocumentTypesCreate` |
| Stored Procedures | Prefijo `SP_WS_`, SNAKE_CASE | `SP_WS_LIST_DOCUMENT_TYPE` |

No crear nuevos tipos genéricos con el sufijo `Model` en `Core`, `Application` o `Infrastructure`. El nombre debe reflejar la responsabilidad real del tipo.

## 🌐 Ejemplo de endpoints

El módulo `DocumentType` sigue un patrón de rutas basado en controlador más segmento de acción.

```http
# Crear un tipo de documento
POST /api/DocumentTypes/DocumentTypesCreate
Body:
{
  "businessId": 1,
  "codeSunat": "01",
  "description": "Factura",
  "createdBy": 100
}

# Obtener lista paginada
GET /api/DocumentTypes/DocumentTypesList?business_id=1&search=&page=1&pageSize=10

# Obtener opciones para select
GET /api/DocumentTypes/DocumentTypeSelect?business_id=1&page=1&pageSize=20

# Obtener por id
GET /api/DocumentTypes/DocumentTypeIdList?documentTypeId=10

# Actualizar
PUT /api/DocumentTypes/DocumentTypesUpdate
Body:
{
  "businessId": 1,
  "documentTypeId": 10,
  "codeSunat": "01",
  "description": "Factura Electrónica",
  "updateUser": 100
}

# Cambiar estado
PATCH /api/DocumentTypes/DocumentTypesStatus
Body:
{
  "businessId": 1,
  "documentTypeId": 10,
  "status": "INACTIVO",
  "updateUser": 100
}
```

## 📌 Notas técnicas

- AutoMapper se utiliza para conversiones entre entidades, proyecciones, resultados y DTOs.
- FluentValidation valida reglas de negocio antes de ejecutar persistencia.
- La persistencia se implementa con Dapper y stored procedures.
- Cuando aplique, usar `IDapperHelper`, `DapperParams` y `DbParam` para mantener consistencia en la ejecución y parametrización de consultas.
- `SharedKernel` concentra tipos compartidos como `GlobalResponse`.

### Auditoría con `AuditLog`

Algunos módulos implementan trazabilidad mediante `AuditLog`. Para que un campo sea auditado, la propiedad debe marcarse con `[AuditField("Alias amigable")]`.

El flujo recomendado es:

1. Marcar en la entidad, proyección o snapshot solo las propiedades que deben auditarse.
2. En el caso de uso, obtener el estado anterior y el nuevo estado cuando se trate de una actualización.
3. Crear la cabecera de auditoría con `IAuditLogFactory`.
4. Registrar la operación con `IAuditService` usando `RegisterCreateAsync`, `RegisterUpdateAsync` o `RegisterDeleteAsync`.
5. Permitir que el servicio persista cabecera y detalle a través del módulo registrado con `AddAuditInjection`.

La auditoría solo considera propiedades marcadas con `[AuditField]`, y el alias definido en el atributo es el texto que se usará como nombre visible del campo auditado.

## 🧾 Convenciones de commits

Usa el formato `type(scope): descripción corta`.

Tipos de commit soportados:

| Tipo | Descripción |
| ---- | ----------- |
| `feat` | Nueva funcionalidad |
| `fix` | Corrección de errores |
| `refactor` | Refactorización interna sin cambiar el comportamiento |
| `docs` | Cambios en la documentación |
| `test` | Pruebas agregadas o actualizadas |
| `hotfix` | Corrección crítica en producción |

El `scope` debe identificar el módulo afectado. Usa el estilo de nombres ya existente en el repositorio:

- `operationsWorkOrder`
- `operationsWorkOrderStatus`
- `operationsStatus`
- `operationsTeam`
- `operationsResponsible`
- `ssomaAssignment`
- `auth`
- `users`

Los mensajes de commit deben escribirse en modo imperativo. Cuando sea útil, agrega un cuerpo corto con los cambios relevantes:

```text
type(scope): descripción corta en imperativo

- cambio 1
- cambio 2
- cambio 3
```

Ejemplos:

```text
feat(operationsWorkOrder): agregar campos de ubicación

- Agrega latitud y longitud a la entidad
- Actualiza el stored procedure SP_WS_CREATE_WORK_ORDER
- Agrega endpoint para actualizaciones de ubicación en tiempo real

fix(operationsStatus): manejar respuesta nula en GetById

- Agrega validación antes de mapear el resultado
- Actualiza la prueba unitaria para el caso negativo

refactor(operationsResponsible): reemplazar validaciones manuales por FluentValidation

docs: actualizar README con las convenciones vigentes
```

