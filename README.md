# ChallengeApi — API REST de Horóscopo

API REST desarrollada en .NET 8 como solución al challenge técnico de Recursiva S.R.L.

## Tecnologías utilizadas

- .NET 8 (LTS)
- ASP.NET Core Web API con controllers clásicos
- Entity Framework Core 8 (Code First)
- SQL Server 2022
- JWT para autenticación
- Serilog para logging estructurado
- IMemoryCache para caché de horóscopo diario
- xUnit + Moq + FluentAssertions para tests unitarios
- Docker + Docker Compose
- Swagger UI como interfaz de prueba

## Arquitectura

El proyecto sigue Clean Architecture con cuatro capas:

- **ChallengeApi.Domain** — Entidades y excepciones de dominio, sin dependencias externas
- **ChallengeApi.Application** — Casos de uso, interfaces, DTOs y servicios
- **ChallengeApi.Infrastructure** — EF Core, repositorios, caché y cliente HTTP externo
- **ChallengeApi.API** — Controllers, middleware y configuración

## Requisitos previos

- Docker Desktop instalado y corriendo
- Puerto 8080 disponible
- Puerto 1433 disponible

## Levantar el proyecto con Docker

Clonar el repositorio:

```bash
git clone <url-del-repositorio>
cd ChallengeApi
```

Levantar todos los servicios:

```bash
docker compose up --build
```

Esto levanta:
- **SQL Server 2022** en el puerto 1433
- **ChallengeApi** en el puerto 8080

La base de datos se crea y migra automáticamente al iniciar.

Acceder a Swagger UI: http://localhost:8080

Bajar los servicios:

```bash
docker compose down
```

## Levantar el proyecto sin Docker (desarrollo local)

Requisitos:
- .NET 8 SDK
- SQL Server local

Configurar el connection string en `ChallengeApi.API/appsettings.json` si es necesario:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ChallengeApiDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Correr las migraciones:

```bash
dotnet ef database update --project ChallengeApi.Infrastructure --startup-project ChallengeApi.API
```

Correr la API:

```bash
cd ChallengeApi.API
dotnet run
```

## Endpoints disponibles

### Autenticación (sin token)

| Método | Ruta | Descripción |
|--------|------|-------------|
| POST | `/api/auth/register` | Registrar nuevo usuario |
| POST | `/api/auth/login` | Login, devuelve JWT |

### Usuario (requiere token)

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/user/me` | Ver perfil del usuario |
| PUT | `/api/user/me` | Actualizar perfil (excepto username) |

### Horóscopo (requiere token)

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/api/horoscope` | Horóscopo del día + días al cumpleaños |
| GET | `/api/horoscope/historial` | Historial de consultas del usuario |
| GET | `/api/horoscope/estadisticas` | Signos más consultados |

## Cómo usar Swagger con autenticación

1. Ejecutar `POST /api/auth/register` con los datos del usuario
2. Ejecutar `POST /api/auth/login` y copiar el `token` de la respuesta
3. Click en el botón **Authorize** (candado) en la esquina superior derecha
4. Ingresar: `Bearer {token}`
5. Click en **Authorize** y ya podés usar los endpoints protegidos

## Correr los tests

```bash
dotnet test
```

## Decisiones técnicas destacadas

- **Clean Architecture** para separar responsabilidades y facilitar el testeo unitario
- **IMemoryCache** para cachear el horóscopo del día por signo, evitando llamadas repetidas a la API externa. La caché expira a medianoche UTC
- **BCrypt** para hashing de contraseñas
- **Mismo mensaje de error** para usuario no encontrado y password incorrecta, evitando enumeración de usuarios
- **ClockSkew = TimeSpan.Zero** en JWT para expiración exacta del token
- **Migraciones automáticas** al iniciar la aplicación para simplificar el despliegue con Docker