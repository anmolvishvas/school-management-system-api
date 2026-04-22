# School & Coaching Management System — Backend

## Tech stack

- ASP.NET Core 8 Web API
- Entity Framework Core 8 (SQL Server or PostgreSQL)
- Clean Architecture: Domain → Application → Infrastructure → Api
- JWT authentication, RBAC
- AutoMapper, FluentValidation, Swagger

## Solution layout

- `src/SchoolManagement.Domain` — entities and role constants
- `src/SchoolManagement.Application` — services, DTOs, validators, repository interfaces
- `src/SchoolManagement.Infrastructure` — EF Core `ApplicationDbContext`, repositories, migrations
- `src/SchoolManagement.Api` — HTTP API, auth pipeline, Swagger

## Setup

1. Install the [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) and (if needed) the EF CLI: `dotnet tool install --global dotnet-ef`
2. Set `ConnectionStrings:DefaultConnection` in `src/SchoolManagement.Api/appsettings.Development.json` (defaults to local **SchoolDB** with Windows auth; change `Server=` if you use a named instance, e.g. `Server=.\\SQLEXPRESS` or `Server=YOUR-MACHINE\\INSTANCE`).
3. Choose database provider in `Database:Provider`: `SqlServer` (default) or `PostgreSQL` / `Npgsql`
4. Apply migrations (from repo root):

```bash
dotnet ef database update --project src/SchoolManagement.Infrastructure --startup-project src/SchoolManagement.Api
```

5. Run the API:

```bash
dotnet run --project src/SchoolManagement.Api
```

Open Swagger at `http://localhost:5070/swagger` (see `launchSettings.json` for ports).

## Students list (`GET /api/Students`)

- **No query filter**: returns every student (active and inactive).
- **`activeOnly=true`**: returns only rows where `IsActive` is true. If the table is empty, you will see `total: 0` (this is expected).
- In **Development**, if the `Students` table is empty after startup seeding, two demo students are inserted so local Swagger calls return data.

## Seed users (first run)

After migrations, the API seeds:

| Username   | Password      | Role    |
|------------|---------------|---------|
| `admin`    | `Admin@123`   | Admin   |
| `teacher1` | `Teacher@123` | Teacher |

`POST /api/Auth/register` requires an **Admin** JWT (register additional users after logging in as admin).

## API highlights

- `POST /api/Auth/login` — JWT (public)
- `POST /api/Auth/register` — create user (Admin only)
- `GET/POST/PUT/DELETE /api/Students` — student CRUD & paged list (writes Admin-only; all roles authenticated for reads)
- `GET /api/Students/stats` — counts by class/section (Admin)
- `GET /api/Dashboard/kpis` — total students; **attendance % (last 30 days)** when marks exist; fee KPI reserved

## Attendance (`/api/Attendance`)

- `GET /api/Attendance` — paged list (filters: `studentId`, `className`, `section`, `dateFrom`, `dateTo`, `sortBy`, `order`). Roles: **Admin, Teacher, Accountant**
- `GET /api/Attendance/summary?from=&to=` — counts by status + **attendance rate %** (Present+Late = 1, HalfDay = 0.5, Absent = 0; **Excused** excluded from denominator). Optional `className`, `section`
- `GET /api/Attendance/{id}` — single row
- `POST /api/Attendance` — single mark (student must match `class`/`section`). **Admin, Teacher**
- `POST /api/Attendance/bulk-day` — body: `{ "date", "class", "section", "lines": [{ "studentId", "status", "notes" }] }` upserts one row per student per day. **Admin, Teacher**
- `PUT /api/Attendance/{id}` — update status/notes. **Admin, Teacher**
- `DELETE /api/Attendance/{id}` — **Admin** only

`AttendanceStatus` enum values: `Present` (1), `Absent` (2), `Late` (3), `Excused` (4), `HalfDay` (5).

Replace the JWT signing key in production (`Jwt:Key` in configuration).

## Legacy `SchoolDB` / existing tables

Migrations are **idempotent**: if `Students` or `Users` already exist, the first migration skips `CREATE TABLE` and only **adds missing columns** (for example `IsActive`, profile fields). Then `dotnet run` can apply pending migrations without “object already exists” errors.
