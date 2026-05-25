DATOS DEL PROYECTO

  ┌───────────────────┬───────────────────────────────────────────────────┐
  │       Item        │                      Detalle                      │
  ├───────────────────┼───────────────────────────────────────────────────┤
  │ Backend           │ ASP.NET Core 8 Web API                            │
  ├───────────────────┼───────────────────────────────────────────────────┤
  │ Frontend          │ React 19 + Vite (archivos estáticos)              │
  ├───────────────────┼───────────────────────────────────────────────────┤
  │ Base de datos     │ SQL Server en SRV-ERP-SAPI\SQLEXPRESS             │
  ├───────────────────┼───────────────────────────────────────────────────┤
  │ Servidor local    │ IP 192.168.1.29, ya funciona en red local con IIS │
  ├───────────────────┼───────────────────────────────────────────────────┤
  │ IP pública actual │ 200.115.22.84                                     │
  ├───────────────────┼───────────────────────────────────────────────────┤
  │ Dominio           │ idelcom.pe (ya existe, se usa para SMTP)          │
  └───────────────────┴───────────────────────────────────────────────────┘

  ---
  TAREAS REQUERIDAS

  1. DNS — Crear subdominios

  En el panel del dominio idelcom.pe, crear dos registros tipo A:
  - erp.idelcom.pe → 200.115.22.84
  - api.idelcom.pe → 200.115.22.84

  ---
  2. Router/Firewall — Port Forwarding

  Redirigir al servidor 192.168.1.29:

  ┌────────────────┬────────────────┬───────────┐
  │ Puerto externo │ Puerto interno │ Protocolo │
  ├────────────────┼────────────────┼───────────┤
  │ 80             │ 80             │ TCP       │
  ├────────────────┼────────────────┼───────────┤
  │ 443            │ 443            │ TCP       │
  └────────────────┴────────────────┴───────────┘

  ---
  3. Servidor Windows — Instalar certificado SSL

  - Instalar win-acme (win-acme.com)
  - Generar certificado Let's Encrypt para:
    - erp.idelcom.pe
    - api.idelcom.pe
  - Configurar renovación automática

  ---
  4. IIS — Configurar dos sitios web

  Sitio 1 — Frontend
  - Nombre: IDELCOM-ERP-FRONT
  - Ruta física: carpeta dist/ del build de React (equipo de desarrollo entrega esta carpeta)
  - Binding HTTP: erp.idelcom.pe:80 → redirigir a HTTPS
  - Binding HTTPS: erp.idelcom.pe:443 con certificado SSL
  - Agregar archivo web.config para SPA (equipo de desarrollo entrega este archivo)

  Sitio 2 — Backend API
  - Nombre: IDELCOM-ERP-API
  - Ruta física: carpeta de publicación del .NET 8 (equipo de desarrollo entrega esta carpeta)
  - Binding HTTP: api.idelcom.pe:80 → redirigir a HTTPS
  - Binding HTTPS: api.idelcom.pe:443 con certificado SSL
  - Application Pool: sin código administrado (No Managed Code), 64-bit

  ---
  5. Servidor — Instalar .NET 8 Runtime

  Si no está instalado:
  - Descargar .NET 8 Hosting Bundle desde dotnet.microsoft.com
  - Instalar en el servidor
  - Reiniciar IIS después de instalar (iisreset)

  ---
  6. Firewall de Windows — Abrir puertos

  Verificar que el firewall de Windows permita tráfico entrante en los puertos 80 y 443.

  ---
  LO QUE ENTREGA EL EQUIPO DE DESARROLLO

  ┌─────────────────────────────────────┬────────────────────────────────────────────────┐
  │             Entregable              │                  Descripción                   │
  ├─────────────────────────────────────┼────────────────────────────────────────────────┤
  │ Carpeta dist/                       │ Build del frontend listo para IIS              │
  ├─────────────────────────────────────┼────────────────────────────────────────────────┤
  │ Archivo web.config                  │ Configuración SPA para IIS (routing de React)  │
  ├─────────────────────────────────────┼────────────────────────────────────────────────┤
  │ Carpeta de publicación API          │ Build del backend .NET 8 listo para IIS        │
  ├─────────────────────────────────────┼────────────────────────────────────────────────┤
  │ Archivo appsettings.Production.json │ Config con dominios y parámetros de producción │
  └─────────────────────────────────────┴────────────────────────────────────────────────┘

  ---
  VERIFICACIÓN FINAL

  - https://erp.idelcom.pe carga el sistema sin errores
  - https://api.idelcom.pe/swagger responde (solo para prueba inicial)
  - Login funciona correctamente
  - Certificado SSL muestra candado verde en el navegador
