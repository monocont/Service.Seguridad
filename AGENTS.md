# Service.Seguridad

.NET 10.0 ASP.NET Core web API implementing authentication (local + Google federated) using Clean Architecture / CQRS with MediatR.

## Quick Commands

```powershell
# Build
dotnet build Service.Seguridad.slnx

# Run (API project)
dotnet run --project Service.Seguridad.API

# Swagger available at https://localhost:54515/swagger
```

## Architecture

```
Service.Seguridad.API          -- Entry point. Controllers, DTOs, middleware, Program.cs
Service.Seguridad.Application  -- CQRS commands + handlers via MediatR
Service.Seguridad.Domain       -- Entities, enums, repository interfaces
Service.Seguridad.Infrastructure -- EF Core DbContext, repositories, external services
```

Dependency direction: **API -> Application -> Domain**. Infrastructure depends on both Application and Domain. Domain has zero external dependencies.

### Key patterns

- **CQRS via MediatR**: Commands in `Application/Commands/`, handlers in `Application/Handlers/`. API controllers delegate to `IMediator.Send()`.
- **Soft delete**: All entities inherit `EntidadAuditoria` with `Activo` flag. EF Core global query filter `e => e.Activo` hides deleted rows automatically.
- **Repository interfaces** live in Domain; implementations in Infrastructure. Register in `Program.cs` as scoped.
- **Refresh tokens** are stored as httpOnly secure cookies (`refreshToken`), not in the response body.

### Roles

- **`seguridad.rol`** — `id_rol` (CHAR(36)), `codigo_rol` (VARCHAR(10), unique), `nombre_rol` (VARCHAR(50)). Seed: `USER` and `ADMIN`.
- **`seguridad.usuario_rol`** — composite PK `(id_usuario, id_rol)`. FKs cascade to `usuario` and `rol`.
- Roles are loaded during login/refresh and embedded as `ClaimTypes.Role` in the JWT.
- Use `[Authorize(Roles = "ADMIN")]` on controllers to restrict access.
- New users (Google registration) automatically receive the `USER` role.
- SQL script to create tables: `Infrastructure/Script/crear_tablas_rol.sql` (run manually against PostgreSQL).

### Database

- PostgreSQL via EF Core 10 with Npgsql provider.
- Schema: `seguridad` (set in `SecurityDbContext.OnModelCreating`).
- SQL script at `Infrastructure/Script/query.sql` creates base tables (usuario, token_refresco).
- Connection string key: `SecurityDb` in `appsettings.json`.

### Auth flow

| Endpoint | Method | Description |
|---|---|---|
| `POST /api/v1/seguridad/auth/login` | Local credentials | Returns JWT + sets refresh cookie |
| `POST /api/v1/seguridad/auth/federated-login` | Google identity token | Existing user login |
| `POST /api/v1/seguridad/auth/registro-google` | Google identity token | New user registration via Google |
| `POST /api/v1/seguridad/auth/refresh` | Cookie-based | Refreshes access token |

JWT validation params configured in `Program.cs`: `Jwt:Issuer`, `Jwt:Audience`, `Jwt:Key` (HMAC-SHA256, 32+ chars).

## Estándares de Desarrollo

### 1. Nomenclatura (Naming Conventions)
- **Idioma:** Español (excepto términos técnicos: `domain`, `api`, `token`, `service`, `infrastructure`, `context`, etc.).
- **Clases/Objetos (Domain/App):** `PascalCase` (ej: `UsuarioDetalle`).
- **Interfaces:** `PascalCase` con prefijo `I` (ej: `IUsuarioService`).
- **Métodos/Funciones:** `camelCase` (ej: `obtenerUsuario()`).
- **Variables/Atributos:** `camelCase` (ej: `nombreUsuario`).
- **Tablas DB:** `snake_case` en minúsculas (ej: `usuario_detalle`).
- **Columnas DB:** `snake_case` (ej: `fecha_creacion`).
- **Carpetas/Archivos:** `snake_case` en minúsculas (ej: `reglas_negocio/`).

### 2. Estándar de Respuesta API
Todas las respuestas deben seguir el patrón `ApiResponse<T>` para garantizar la consistencia:

| Campo | Tipo | Descripción |
|---|---|---|
| **`data`** | `T` \| `null` | El objeto o colección solicitado. `null` si `success` es `false`. |
| **`success`** | `boolean` | `true` si la operación fue exitosa. `false` si hubo error. |
| **`errors`** | `string[]` | Lista de mensajes de error (de validación o de sistema). |
| **`messages`** | `string[]` | Lista de mensajes informativos (opcional). |

**Comportamiento esperado:**
- **Éxito (2xx):** `{ "data": <T>, "success": true, "errors": [], "messages": [] }`
- **Error Controlado (4xx):** `{ "data": null, "success": false, "errors": ["Mensaje"], "messages": [] }`
- **Error No Controlado (500):** `{ "data": null, "success": false, "errors": ["Error en el proceso"], "messages": [] }`

## Important details

- **Target framework**: `net10.0` -- all 4 projects use the same target.
- **ImplicitUsings + Nullable** enabled project-wide.
- **No migrations**: schema is created via raw SQL scripts. Do not run `dotnet ef migrations add` -- it will conflict with the hand-maintained schema.
- **ErrorHandlingMiddleware** and **ResponseFormattingMiddleware** are registered in `Program.cs` before `UseAuthentication()`. The error middleware handles mapped exceptions (UnauthorizedAccessException, KeyNotFoundException, ArgumentException) with their messages, and unmapped exceptions with "Error en el proceso".
- **Google config**: `Google:ClientId` in appsettings. `IGoogleAuthService` uses `HttpClient` for Google token verification.
- **BCrypt** (`BCrypt.Net-Next`) handles password hashing in `LoginLocalCommandHandler`.
- **NuGet versions** use wildcard ranges (`10.*`, `12.*`, `7.*`, `8.*`) -- pin exact versions for production.
