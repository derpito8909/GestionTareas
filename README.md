# gestionTareas ✅

Sistema interno para **gestión de tareas**: registrar usuarios, crear/asignar tareas y hacer seguimiento del estado.

* **Backend:** .NET 8 (Web API) + EF Core + FluentValidation + SQL Server
* **Frontend:** Angular **19.2.19** (standalone) + Reactive Forms + Http Interceptors
* **BD:** SQL Server con columna JSON (`AdditionalInfo`) y consultas con `JSON_VALUE` / `OPENJSON` / `JSON_QUERY`.

---

## 1) Requisitos

### Backend

* .NET SDK **8.x**
* SQL Server

### Frontend

* Node.js (LTS)
* Angular CLI **19.2.19**

---

## 2) Ejecución del proyecto

### 2.1 Base de datos

1. Crear la base de datos y tablas con este script: 
[Create_SQL.sql](./Create_SQL.sql).
2. Ejecutar los **INSERT** y consultas de prueba (ver sección **7**).
---

### 2.2 Backend (.NET 8)

1. Clone el repositorio:

```shell
https://github.com/derpito8909/GestionTareas.git
```

2. Ve a la carpeta del API:

```bash
cd GestionTareas/API
```

3. Configura tu connection string en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "Default": "Server=localhost;Database=gestionTareas;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

4. Ejecuta el API:

```bash
dotnet restore
dotnet build
dotnet run --project GestionTareas.Api/GestionTareas.Api.csproj
```

4. Verifica el puerto en la consola:

* `Now listening on: http://localhost:5239`

5. Abre Swagger:

* `http://localhost:5239/swagger`
---

### 2.3 Frontend (Angular 19.2.19)

1. Instalar dependencias:

```bash
cd GestionTareas/gestion-tareas-web/
npm install
```

3. Ejecutar:

```bash
npm start
# o
ng serve --proxy-config proxy.conf.json
```

4. El front consumirá:

* `GET /api/users`
* `GET /api/tasks`

---

## 3) Decisiones técnicas

### 3.1 Backend

**Arquitectura por capas (mínima y clara):**

* **Domain**

  * Entidades (encapsuladas)
  * Enums (`TaskStatus`)
  * Reglas de negocio que deben vivir cerca del dominio (p.ej. transición de estados)
  * Excepciones y códigos de error (para mensajes humanos)

* **Application**

  * DTOs (Request/Response)
  * Interfaces de servicios y repositorios
  * Casos de uso (services)
  * Validaciones con FluentValidation

* **Infrastructure**

  * EF Core: `DbContext`, configuraciones `IEntityTypeConfiguration<>`
  * Repositorios EF

* **Api**

  * Controllers (delgados)
  * Middleware de excepciones (fuente de verdad del formato de error)

**Por qué así:**

* Controladores sin lógica: solo HTTP.
* Reglas de negocio en Service/Domain (no en Controllers).
* DTOs evitan exponer entidades EF.
* Un solo punto de salida de errores (middleware) → mensajes humanos consistentes.

**Validaciones:**

* FluentValidation valida estructura (required, rangos, JSON válido si aplica).
* El Service/Domain valida reglas de negocio (por ejemplo: *no Pending → Done directo*).

**Persistencia:**

* EF Core con configuraciones explícitas:

  * `Tasks.UserId` (FK)
  * `Tasks.AdditionalInfo` (JSON)

---

### 3.2 Frontend

**Estructura por features (práctica y mantenible):**

* `features/tasks` y `features/users`
* `domain` (modelos/contratos)
* `data-access` (API services + facades)
* `ui` (componentes)

**Interceptors:**

* `api-base-url.interceptor`: antepone baseURL si aplica (con proxy puede quedar vacío).
* `error.interceptor`: convierte errores en un modelo estándar para mostrar mensajes humanos.

**Reactive Forms:**

* Formularios tipados + validación.

**Facade pattern (simple):**

* Centraliza loading/error/datos (evita lógica dispersa en componentes).

---

## 4) Contrato de errores (API)

El API devuelve errores en un formato único (para mostrar en UI):

```json
{
  "traceId": "...",
  "code": "...",
  "message": "Mensaje humano",
  "errors": {
    "campo": ["mensaje"]
  }
}
```

* `traceId`: útil para cruzar con logs.
* `code`: código estable para manejo en UI.
* `message`: texto humano.
* `errors`: errores por campo (validaciones).

---

## 5) Documentación técnica de endpoints

### 5.1 Users

#### POST `/api/users`

Crea un usuario.

**Request**

```json
{
  "name": "Ana Gómez",
  "email": "ana.gomez@empresa.com"
}
```

**Response 201**

```json
{
  "id": 1,
  "name": "Ana Gómez",
  "email": "ana.gomez@empresa.com",
  "createdAt": "2026-01-31T00:00:00"
}
```

**Errores**

* 400: validación (name/email requeridos, email formato)

---

#### GET `/api/users`

Lista usuarios (normalmente ordenados por fecha de creación desc).

**Response 200**

```json
[
  {
    "id": 5,
    "name": "Laura Rojas",
    "email": "laura.rojas@empresa.com",
    "createdAt": "2026-01-31T00:00:00"
  }
]
```

---

### 5.2 Tasks

#### POST `/api/tasks`

Crea una tarea nueva.

**Reglas**

* `title` obligatorio
* debe tener `assignedUserId`

**Request**

```json
{
  "title": "Actualizar documentación API",
  "description": "Actualizar endpoints de Users/Tasks",
  "assignedUserId": 1,
  "additionalInfoJson": "{\"Prioridad\":\"Media\",\"FechaEstimada\":\"2026-02-15\",\"Etiquetas\":[\"Backend\",\"Docs\"],\"Meta\":{\"origen\":\"TechLead\"}}"
}
```

**Response 201**

```json
{
  "id": 10,
  "title": "Actualizar documentación API",
  "description": "Actualizar endpoints de Users/Tasks",
  "status": "Pending",
  "createdAt": "2026-01-31T00:00:00",
  "assignedUserId": 1,
  "assignedUserName": "Ana Gómez",
  "additionalInfoJson": "{...}"
}
```

---

#### GET `/api/tasks`

Lista tareas con filtros.

**Query params (opcionales)**

* `userId` (int)
* `status` (`Pending|InProgress|Done`)
* `orderByCreatedAtDesc` (bool)

**Filtros por JSON (opcionales)**

* `priority` → filtra por `$.Prioridad`
* `tag` → filtra si `$.Etiquetas` contiene el valor
* `dueDateFrom`, `dueDateTo` → filtra por `$.FechaEstimada`

**Ejemplos**

* `/api/tasks?status=Pending`
* `/api/tasks?userId=1&status=Done`
* `/api/tasks?priority=Alta`
* `/api/tasks?tag=Urgente`

---

#### GET `/api/tasks/{id}`

Obtiene una tarea por id.

---

#### PUT `/api/tasks/{id}/assign`

Asigna una tarea a un usuario.

**Request**

```json
{ "userId": 2 }
```

---

#### PUT `/api/tasks/{id}/status`

Cambia el estado.

**Regla**

* No se permite `Pending → Done` directo.

**Request**

```json
{ "status": "InProgress" }
```

---

## 6) JSON en SQL Server (resumen práctico)

* `AdditionalInfo` almacena JSON.
* Validación de JSON: `ISJSON(AdditionalInfo) = 1`.
* Lectura de campos: `JSON_VALUE(AdditionalInfo, '$.Prioridad')`.
* Lectura de objetos/arrays: `JSON_QUERY(AdditionalInfo, '$.Meta')`.
* Arrays: `OPENJSON(AdditionalInfo, '$.Etiquetas')`.
* Update específico: `JSON_MODIFY(AdditionalInfo, '$.Prioridad', 'Media')`.

---

## 7) Seed + consultas de ejemplo (copiar/pegar)

```sql
USE gestionTareas;
GO

INSERT INTO dbo.Users (Name, Email)
VALUES
('Ana Gómez',    'ana.gomez@empresa.com'),
('Carlos Pérez', 'carlos.perez@empresa.com'),
('Diana Ruiz',   'diana.ruiz@empresa.com'),
('Felipe Mora',  'felipe.mora@empresa.com'),
('Laura Rojas',  'laura.rojas@empresa.com');
GO

INSERT INTO dbo.Tasks (Title, Description, Status, UserId, CreatedAt, AdditionalInfo)
VALUES
('Preparar informe semanal', 'Consolidar avances del equipo', 'Pending', 1, DATEADD(DAY, -12, GETDATE()),
 N'{"Prioridad":"Alta","FechaEstimada":"2026-02-10","Etiquetas":["Urgente","Reportes"],"Meta":{"origen":"PMO"}}'),

('Actualizar documentación API', 'Actualizar endpoints de Users/Tasks', 'InProgress', 1, DATEADD(DAY, -10, GETDATE()),
 N'{"Prioridad":"Media","FechaEstimada":"2026-02-15","Etiquetas":["Backend","Docs"],"Meta":{"origen":"TechLead"}}'),

('Revisar PR #342', 'Validar cambios de performance', 'Done', 1, DATEADD(DAY, -8, GETDATE()),
 N'{"Prioridad":"Baja","FechaEstimada":"2026-01-25","Etiquetas":["CodeReview"],"Meta":{"repo":"core-api"}}');
GO

INSERT INTO dbo.Tasks (Title, Description, Status, UserId, CreatedAt, AdditionalInfo)
VALUES
('Configurar pipeline CI', 'Build + tests automáticos', 'Pending', 2, DATEADD(DAY, -11, GETDATE()),
 N'{"Prioridad":"Alta","FechaEstimada":"2026-02-05","Etiquetas":["DevOps","Urgente"],"Meta":{"tool":"AzureDevOps"}}'),

('Corregir bug login', 'Token expirado en refresh', 'InProgress', 2, DATEADD(DAY, -7, GETDATE()),
 N'{"Prioridad":"Alta","FechaEstimada":"2026-02-02","Etiquetas":["Backend","Seguridad"],"Meta":{"area":"Auth"}}'),

('Optimizar consulta de tareas', 'Mejorar índice y plan', 'Done', 2, DATEADD(DAY, -3, GETDATE()),
 N'{"Prioridad":"Media","FechaEstimada":"2026-01-29","Etiquetas":["SQL","Performance"],"Meta":{"db":"gestionTareas"}}');
GO

INSERT INTO dbo.Tasks (Title, Description, Status, UserId, CreatedAt, AdditionalInfo)
VALUES
('Diseñar pantalla listado tareas', 'Filtro por estado y acciones', 'Pending', 3, DATEADD(DAY, -9, GETDATE()),
 N'{"Prioridad":"Media","FechaEstimada":"2026-02-12","Etiquetas":["Frontend","Angular"],"Meta":{"ui":"Tabler"}}'),

('Implementar formulario crear tarea', 'Reactive forms + validaciones', 'InProgress', 3, DATEADD(DAY, -6, GETDATE()),
 N'{"Prioridad":"Alta","FechaEstimada":"2026-02-04","Etiquetas":["Frontend","Urgente","Angular"],"Meta":{"forms":"reactive"}}'),

('Manejo básico de errores UI', 'Toasts y mensajes', 'Done', 3, DATEADD(DAY, -2, GETDATE()),
 N'{"Prioridad":"Baja","FechaEstimada":"2026-01-30","Etiquetas":["Frontend","UX"],"Meta":{"pattern":"Interceptor"}}');
GO

INSERT INTO dbo.Tasks (Title, Description, Status, UserId, CreatedAt, AdditionalInfo)
VALUES
('Crear endpoints Tasks', 'POST/GET/PUT status', 'Pending', 4, DATEADD(DAY, -13, GETDATE()),
 N'{"Prioridad":"Alta","FechaEstimada":"2026-02-01","Etiquetas":["Backend","API"],"Meta":{"framework":".NET"}}'),

('Implementar validaciones negocio', 'No Pending -> Done directo', 'InProgress', 4, DATEADD(DAY, -5, GETDATE()),
 N'{"Prioridad":"Alta","FechaEstimada":"2026-02-03","Etiquetas":["Backend","Reglas","Urgente"],"Meta":{"layer":"Service"}}'),

('DTOs y mapping', 'Separar request/response', 'Done', 4, DATEADD(DAY, -1, GETDATE()),
 N'{"Prioridad":"Media","FechaEstimada":"2026-01-30","Etiquetas":["Backend","CleanCode"],"Meta":{"mapping":"Manual"}}');
GO

INSERT INTO dbo.Tasks (Title, Description, Status, UserId, CreatedAt, AdditionalInfo)
VALUES
('Crear índice adicional JSON', 'Filtrar por Prioridad', 'Pending', 5, DATEADD(DAY, -4, GETDATE()),
 N'{"Prioridad":"Media","FechaEstimada":"2026-02-08","Etiquetas":["SQL","JSON"],"Meta":{"idea":"ComputedColumn"}}'),

('Pruebas manuales de endpoints', 'Postman collection', 'InProgress', 5, DATEADD(DAY, -3, GETDATE()),
 N'{"Prioridad":"Baja","FechaEstimada":"2026-02-20","Etiquetas":["QA","API"],"Meta":{"tool":"Postman"}}'),

('Actualizar etiquetas de tareas', 'Ajustar etiquetas para filtros', 'Done', 5, DATEADD(DAY, -2, GETDATE()),
 N'{"Prioridad":"Media","FechaEstimada":"2026-01-31","Etiquetas":["JSON","Mantenimiento"],"Meta":{"scope":"tasks"}}');
GO

-- Consulta: Tareas por usuario + estado + orden por creación
SELECT t.Id, t.Title, t.Status, t.CreatedAt, u.Name AS Usuario
FROM dbo.Tasks t
JOIN dbo.Users u ON t.UserId = u.Id
WHERE t.UserId = 1
  AND t.Status = 'Done'
ORDER BY t.CreatedAt DESC;

-- Consultar un campo dentro del JSON con JSON_VALUE
SELECT
    t.Id, t.Title,
    JSON_VALUE(t.AdditionalInfo, '$.Prioridad') AS Prioridad
FROM dbo.Tasks t
ORDER BY t.CreatedAt DESC;

-- Filtrar tareas por un valor dentro del JSON
SELECT t.Id, t.Title, t.Status, t.CreatedAt
FROM dbo.Tasks t
WHERE JSON_VALUE(t.AdditionalInfo, '$.Prioridad') = 'Alta'
ORDER BY t.CreatedAt DESC;

-- Filtrar por etiqueta dentro del JSON con OPENJSON
SELECT DISTINCT t.Id, t.Title, t.Status, t.CreatedAt
FROM dbo.Tasks t
CROSS APPLY OPENJSON(t.AdditionalInfo, '$.Etiquetas') AS tags
WHERE tags.value = 'Urgente'
ORDER BY t.CreatedAt DESC;

-- Mostrar un objeto del JSON con JSON_QUERY
SELECT
  t.Id,
  t.Title,
  JSON_QUERY(t.AdditionalInfo, '$.Meta') AS MetaJson
FROM dbo.Tasks t;

-- Actualizar un campo específico en el JSON
UPDATE dbo.Tasks
SET AdditionalInfo = JSON_MODIFY(AdditionalInfo, '$.Prioridad', 'Media')
WHERE Id = 10;
```

---

## 8) Funcionalidades Pendientes

* el cambio de el estado de las tareas, falta pulir y detectar los errores 
