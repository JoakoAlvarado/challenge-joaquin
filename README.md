# API REST de Horóscopo

API REST desarrollada en .NET 8

## Tecnolog�as utilizadas

- .NET 8 (LTS)
- ASP.NET Core Web API con controllers clásicos
- Entity Framework Core 8 (Code First)
- SQL Server 2022
- JWT para autenticación
- Serilog para logging estructurado
- IMemoryCache para cache de horoscopo diario
- xUnit + Moq + FluentAssertions para tests unitarios
- Docker + Docker Compose
- Swagger UI como interfaz de prueba

## Arquitectura

El proyecto sigue Clean Architecture con cuatro capas:

- **ChallengeApi.Domain** : Entidades y excepciones de dominio, sin dependencias externas
- **ChallengeApi.Application** : Casos de uso, interfaces, DTOs y servicios
- **ChallengeApi.Infrastructure** : EF Core, repositorios, caché y cliente HTTP externo
- **ChallengeApi.API** : Controllers, middleware y configuración

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
docker compose down -v --remove-orphans
docker rmi challengeapi-api
docker compose build --no-cache
docker compose up -d
```

Esto levanta:
- **SQL Server 2022** en el puerto 1433
- **ChallengeApi** en el puerto 8080

La base de datos se crea y migra autom�ticamente al iniciar.

Acceder a Swagger UI: http://localhost:8080


Reconstruir después de cambios:

```bash
docker compose down
docker compose build --no-cache
docker compose up -d
```


Bajar los servicios:

```bash
docker compose down
```

## Levantar el proyecto sin Docker (desarrollo local)

Requisitos:
- .NET 8 SDK
- SQL Server local instalado y corriendo
- Herramientas de EF Core instaladas globalmente:

```bash
dotnet tool install --global dotnet-ef
```

Configurar el connection string en `ChallengeApi.API/appsettings.json` si es necesario:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=ChallengeApiDb;Trusted_Connection=True;TrustServerCertificate=True;"
}
```

Crear la base de datos y aplicar las migraciones:

```bash
dotnet ef migrations add InitialCreate --project ChallengeApi.Infrastructure --startup-project ChallengeApi.API
dotnet ef database update --project ChallengeApi.Infrastructure --startup-project ChallengeApi.API
```

> Si la migración ya existe en el repositorio, ejecutar solo el segundo comando.

Correr la API:

```bash
cd ChallengeApi.API
dotnet run
```

Acceder a Swagger UI según el método usado:

| Método | URL |
|--------|-----|
| `dotnet run` (http) | http://localhost:5176 |
| `dotnet run` (https) | https://localhost:7043 |
| IIS Express (Visual Studio) | https://localhost:44365 |
| Docker | http://localhost:8080 |


## Endpoints disponibles

### Autenticación (sin token)

| M�todo | Ruta | Descripci�n |
|--------|------|-------------|
| POST | `/api/auth/register` | Registrar nuevo usuario |
| POST | `/api/auth/login` | Login, devuelve JWT |

### Usuario (requiere token)

| M�todo | Ruta | Descripci�n |
|--------|------|-------------|
| GET | `/api/user/me` | Ver perfil del usuario |
| PUT | `/api/user/me` | Actualizar perfil (excepto username) |

### Hor�scopo (requiere token)

| M�todo | Ruta | Descripci�n |
|--------|------|-------------|
| GET | `/api/horoscope` | Horóscopo del día + días al cumpleaños |
| GET | `/api/horoscope/historial` | Historial de consultas del usuario |
| GET | `/api/horoscope/estadisticas` | Signos más consultados |

## C�mo usar Swagger con autenticaci�n

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
- **IMemoryCache** para cachear el hor�scopo del d�a por signo, evitando llamadas repetidas a la API externa. La cach� expira a medianoche UTC
- **BCrypt** para hashing de contrase�as
- **Mismo mensaje de error** para usuario no encontrado y password incorrecta, evitando enumeraci�n de usuarios
- **ClockSkew = TimeSpan.Zero** en JWT para expiraci�n exacta del token
- **Migraciones autom�ticas** al iniciar la aplicaci�n para simplificar el despliegue con Docker

### Configuración Docker

La ejecución mediante Docker utiliza:

```yaml
ASPNETCORE_ENVIRONMENT=Production
```

y la cadena de conexión definida en:

```text
appsettings.Production.json
```