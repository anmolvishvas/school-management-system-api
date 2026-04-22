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

## Period/Hour attendance (`/api/PeriodAttendance`)

This supports attendance per **day + hour + subject** and teacher allocation.

- A **teacher can handle many subjects**
- A subject does **not** store teacher directly; assignment comes from class/section allocation
- Attendance is upserted per tuple: `StudentId + Date + HourNumber + SubjectId`

Endpoints:
- `GET /api/Subjects?activeOnly=true`
- `POST /api/Subjects` (Admin)
- `PUT /api/Subjects/{id}` (Admin)
- `DELETE /api/Subjects/{id}` (Admin)
- `GET /api/PeriodAttendance` (filter by `studentId`, `subjectId`, `className`, `section`, `dateFrom`, `dateTo`, `hourNumber`) — response also includes timetable `startTime`/`endTime` when mapped
- `POST /api/PeriodAttendance/bulk-mark` (Admin, Teacher)
- `POST /api/PeriodAttendance/bulk-mark-timetable` (Admin, Teacher) — uses timetable slot to auto-resolve class/section/subject/teacher and map period number from that day schedule

Example bulk mark:

```json
{
  "date": "2026-04-22",
  "hourNumber": 2,
  "subjectId": 1,
  "class": "MCA",
  "section": "A",
  "lines": [
    { "studentId": 6, "status": "Present", "notes": "" },
    { "studentId": 7, "status": "Late", "notes": "10 min late" }
  ]
}
```

Example timetable-linked bulk mark:

```json
{
  "date": "2026-04-22",
  "timetableEntryId": 5,
  "lines": [
    { "studentId": 6, "status": "Present", "notes": "" },
    { "studentId": 7, "status": "Late", "notes": "10 min late" }
  ]
}
```

## Teachers and allocations (`/api/Teachers`)

- `GET /api/Teachers?activeOnly=true`
- `POST /api/Teachers` (Admin)
- `PUT /api/Teachers/{id}` (Admin)
- `DELETE /api/Teachers/{id}` (Admin)
- `GET /api/Teachers/allocations?page=1&pageSize=30&teacherId=&subjectId=&className=&section=&activeOnly=true`
- `POST /api/Teachers/allocations` (Admin)
- `PUT /api/Teachers/allocations/{id}` (Admin)
- `DELETE /api/Teachers/allocations/{id}` (Admin)
- `GET /api/Teachers/{id}/teaching-plan?fromDate=2026-01-01&toDate=2026-12-31&activeOnly=true`

## Timetable (`/api/Timetable`)

- `GET /api/Timetable?page=1&pageSize=50&className=&section=&dayOfWeek=&teacherId=&activeOnly=`
- `GET /api/Timetable/{id}`
- `GET /api/Timetable/class-section?className=MCA&section=A&activeOnly=true`
- `POST /api/Timetable` (Admin)
- `POST /api/Timetable/bulk-weekly` (Admin)
- `PUT /api/Timetable/{id}` (Admin)
- `DELETE /api/Timetable/{id}` (Admin)

Rules enforced:
- One class/section can have only one subject in an overlapping day+time range
- One teacher can take only one class in an overlapping day+time range
- Teacher and subject must exist; teacher must be active

Bulk weekly upsert example:

```json
{
  "class": "MCA",
  "section": "A",
  "lines": [
    { "dayOfWeek": "Monday", "startTime": "07:30:00", "endTime": "09:30:00", "subjectId": 2, "teacherId": 1, "isActive": true },
    { "dayOfWeek": "Monday", "startTime": "09:30:00", "endTime": "10:30:00", "subjectId": 3, "teacherId": 2, "isActive": true },
    { "dayOfWeek": "Tuesday", "startTime": "07:30:00", "endTime": "08:30:00", "subjectId": 4, "teacherId": 1, "isActive": true }
  ]
}
```

Behavior:
- Upserts by slot key: `class + section + dayOfWeek + startTime + endTime`
- Existing slot in same class-section is updated, missing slot is inserted
- Validates duplicate slots within payload before writing

## Accountants (`/api/Accountants`)

- `GET /api/Accountants?activeOnly=true` (Admin, Accountant)
- `GET /api/Accountants/{id}` (Admin, Accountant)
- `POST /api/Accountants` (Admin)
- `PUT /api/Accountants/{id}` (Admin)
- `DELETE /api/Accountants/{id}` (Admin)

## Fees and reports (`/api/Fees`)

- `GET /api/Fees/invoices?page=1&pageSize=30&studentId=&status=&className=&section=&dueFrom=&dueTo=` (Admin, Accountant)
- `GET /api/Fees/invoices/{id}` (Admin, Accountant)
- `POST /api/Fees/invoices` (Admin, Accountant)
- `PUT /api/Fees/invoices/{id}` (Admin, Accountant)
- `DELETE /api/Fees/invoices/{id}` (Admin, Accountant)
- `GET /api/Fees/invoices/{invoiceId}/payments` (Admin, Accountant)
- `POST /api/Fees/invoices/{invoiceId}/payments` (Admin, Accountant)
- `GET /api/Fees/reports/summary?from=&to=&className=&section=` (Admin, Accountant)

Invoice payment flow:
- Create invoice with `amount`, `discount`, `dueDate`
- Add one or many payments against invoice
- Status auto-updates: `Pending`, `Partial`, `Paid`, `Overdue`
- Summary report returns billed/collected/due and status counts

Teaching plan response is a tree:
- Teacher profile
- Class/section groups
- Subject allocations per class/section
- `weeklyPeriods` derived from observed period attendance in the selected date range

Replace the JWT signing key in production (`Jwt:Key` in configuration).

## Legacy `SchoolDB` / existing tables

Migrations are **idempotent**: if `Students` or `Users` already exist, the first migration skips `CREATE TABLE` and only **adds missing columns** (for example `IsActive`, profile fields). Then `dotnet run` can apply pending migrations without “object already exists” errors.
