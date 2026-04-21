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
2. Set `ConnectionStrings:DefaultConnection` in `src/SchoolManagement.Api/appsettings.Development.json`
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
- `GET /api/Dashboard/kpis` — total students; attendance/fee KPIs reserved for upcoming modules

Replace the JWT signing key in production (`Jwt:Key` in configuration).
