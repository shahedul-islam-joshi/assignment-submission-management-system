# Assignment & Submission Management System

A role-based web application for a school/college where Teachers create assignments, Students submit their work, and Teachers grade and give feedback. Admin manages users, classes, subjects, and teacher-subject-class assignments.

## Overview

- **Admin** creates Teachers/Students, manages Classes and Subjects, and assigns Teachers to a Subject + Class.
- **Teacher** creates assignments (Draft/Published), reviews student submissions, gives marks and feedback.
- **Student** views assignments for their class, submits answers before the deadline, and views marks/feedback.

## Main Features

- JWT-based authentication with role-based authorization (Admin / Teacher / Student)
- Admin: manage Users, Classes, Subjects, Teacher-Subject-Class assignment
- Teacher: create/update/delete/publish assignments; review and grade submissions
- Student: view published assignments for their class; submit/update answers before deadline; view marks & feedback
- Business rules enforced on the backend:
  - Teacher can only manage their own assignments and grade only their assigned class
  - Student can only see/submit for their own class, and only their own submissions
  - One submission per student per assignment (updates instead of duplicating)
  - Submission blocked after the deadline
  - Marks must be between 0 and the assignment's max marks
- Swagger/OpenAPI docs for the backend
- Unit tests (xUnit) covering the core business rules above

## Technology Stack

| Layer | Technology |
|---|---|
| Frontend | Next.js, React, TypeScript, Tailwind CSS |
| Backend | ASP.NET Core Web API (.NET 10), C# |
| Database | PostgreSQL |
| ORM | Entity Framework Core |
| Auth | JWT (JSON Web Token) |
| Testing | xUnit |

## Project Structure

```
AssignmentManagement.API/            Backend solution root
├── AssignmentManagement.API/        Controllers, Program.cs, appsettings.json (entry point)
├── AssignmentManagement.Application/DTOs (request/response models)
├── AssignmentManagement.Domain/     Entities (User, Role, Class, Subject, TeacherAssignment, Assignment, Submission)
├── AssignmentManagement.Infrastructure/
│   ├── Data/                        AppDbContext (EF Core)
│   ├── Migrations/                  EF Core migrations
│   └── Services/                    Business logic (AuthService, AssignmentService, SubmissionService, etc.)
├── AssignmentManagement.Tests/      xUnit unit tests
└── assignment-frontend/             Next.js frontend
    └── src/
        ├── app/
        │   ├── page.tsx             Login page
        │   ├── admin/page.tsx       Admin dashboard
        │   ├── teacher/page.tsx     Teacher dashboard
        │   └── student/page.tsx     Student dashboard
        └── lib/api.ts                Axios API client (attaches JWT token)
```

## Setup Instructions

### Prerequisites
- [.NET 10 SDK](https://dotnet.microsoft.com/download)
- [Node.js](https://nodejs.org/) (18+)
- [PostgreSQL](https://www.postgresql.org/download/) installed and running

### 1. Clone the repository
```bash
git clone <your-repo-url>
cd AssignmentManagement.API
```

### 2. Database Setup

Create a `appsettings.Development.json` file inside `AssignmentManagement.API/` (this file is git-ignored and never committed) with your real PostgreSQL password:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=AssignmentManagementDb;Username=postgres;Password=YOUR_PASSWORD_HERE"
  }
}
```

Then run the migrations to create the database and tables:

```bash
cd AssignmentManagement.Infrastructure
dotnet ef database update --startup-project ../AssignmentManagement.API
```

This creates the `AssignmentManagementDb` database with all required tables (Users, Roles, Classes, Subjects, TeacherAssignments, Assignments, Submissions) — no manual table creation needed.

### 3. Running the Backend

```bash
cd AssignmentManagement.API
dotnet run
```

The API will start at `https://localhost:7201` (check your terminal output for the exact port).
Swagger UI: `https://localhost:7201/swagger`

> First time running locally over HTTPS? Trust the local dev certificate:
> `dotnet dev-certs https --trust`

### 4. Running the Frontend

In a separate terminal:

```bash
cd assignment-frontend
npm install
```

Create a `.env.local` file in `assignment-frontend/`:

```
NEXT_PUBLIC_API_URL=https://localhost:7201/api
```

Then run:

```bash
npm run dev
```

Open `http://localhost:3000` in your browser.

### 5. Running the Tests

```bash
cd AssignmentManagement.Tests
dotnet test
```

## Demo Credentials

| Role | Email | Password |
|---|---|---|
| Admin | admin@test.com | Admin@123 |
| Teacher | teacher@test.com | Teacher@123 |
| Student | student@test.com | Student@123 |

> These are seeded by registering through `POST /api/Auth/register` (Admin) and `POST /api/Users` (Teacher/Student created by Admin). If setting up fresh, register an Admin first via Swagger, then use the Admin dashboard/API to create Teacher and Student accounts, a Class, a Subject, and a Teacher-Subject-Class assignment before testing the Teacher/Student flows.

## Assumptions

- A student belongs to only one class.
- A teacher can teach multiple subjects (and multiple classes), one Subject+Class pair per `TeacherAssignment` row.
- An assignment belongs to exactly one class and one subject.
- Only Published assignments are visible to students.
- Marks are always between 0 and the assignment's maximum marks.
- A Teacher must first be assigned (by Admin) to a Subject+Class before they can create assignments for it.
- Role is assigned at account-creation time and does not change afterward.

## Known Limitations

- No password reset / forgot-password flow.
- No pagination or advanced filtering on list endpoints (Users, Assignments, Submissions).
- The Admin dashboard UI does not yet have a form for creating Teacher-Subject-Class assignments (the API endpoint `POST /api/TeacherAssignments` exists and is fully functional; it can be tested via Swagger).
- JWT tokens are valid for 24 hours; there is no refresh-token flow.
- File/attachment uploads are not implemented — submissions are text-based answers only.
- No email notifications for publishing, grading, or deadlines.
- Not yet deployed to a live URL (designed to run locally per the setup instructions above).
