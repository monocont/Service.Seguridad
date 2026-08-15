# Service.Seguridad

Microservicio de autenticación, autorización y gestión de usuarios para la plataforma **Monocont**.

---

## 📌 Características
- Autenticación mediante **JWT (JSON Web Tokens)** con Refresh Tokens.
- Autenticación federada con **Google Sign-In**.
- Gestión de usuarios y roles (**SuperAdmin**, **Admin**, **Usuario**).
- Arquitectura Limpia (**Clean Architecture**) con CQRS y **MediatR**.
- Validación de solicitudes con **FluentValidation**.
- Persistencia en **PostgreSQL** mediante **Entity Framework Core**.

---

## 🏗 Arquitectura y Estructura
```
Service.Seguridad/
├── Service.Seguridad.API/            # Controladores REST, Middleware y Program.cs
├── Service.Seguridad.Application/    # Commands, Queries, DTOs, Interfaces y Behaviors
├── Service.Seguridad.Domain/         # Entidades de Dominio, Enums y Value Objects
└── Service.Seguridad.Infrastructure/ # DbContext, Repositorios, BCrypt, Google Auth y Scripts SQL
```

---

## ⚙️ Configuración y Variables de Entorno

Archivo: `appsettings.json`

```json
{
  "ConnectionStrings": {
    "SeguridadDb": "Host=localhost;Port=5432;Database=monocont_seguridad;Username=postgres;Password=postgres"
  },
  "JwtSettings": {
    "SecretKey": "<clave-secreta-jwt>",
    "Issuer": "Service.Seguridad",
    "Audience": "Monocont",
    "ExpiryMinutes": 60,
    "RefreshTokenExpiryDays": 7
  },
  "GoogleAuth": {
    "ClientId": "<google-client-id>.apps.googleusercontent.com"
  }
}
```

---

## 🚀 Ejecución

### Puerto por defecto: `5000`

```bash
cd Service.Seguridad.API
dotnet run
```
- **Swagger UI:** `http://localhost:5000/swagger`

---

## 🗄 Base de Datos
Ejecutar los scripts ubicados en `Service.Seguridad.Infrastructure/Script/` en orden correlativo en la base de datos `monocont_seguridad`.
