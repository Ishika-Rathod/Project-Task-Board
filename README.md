# Project Task Board (React + ASP.NET Core)

This repository contains a full-stack task board app:

- Backend: `TaskBoard.Api` (`.NET 9`, ASP.NET Core Web API, EF Core, SQLite)
- Frontend: `task-board-ui` (React + Vite + Axios + React Router)

## Features

- Projects CRUD (`/api/projects`)
- Tasks CRUD with filtering/sorting/pagination (`/api/projects/{projectId}/tasks`, `/api/tasks/{id}`)
- Comments add/list/delete (`/api/tasks/{taskId}/comments`, `/api/comments/{id}`)
- Dashboard summary (`/api/dashboard`)
- SQLite persistence with EF migrations and startup seed data
- Global API exception middleware and validation responses
- User-friendly frontend error messages for validation failures (while backend still returns proper 400/404/409/500 codes)
- Bonus: debounced title search on task board
- Bonus: task status audit trail shown in task detail

## Prerequisites
Install these tools on the target machine before running:
- Git (to clone repository)
- .NET SDK 9.x (backend target framework is net9.0)
- Node.js 20.x (recommended) and npm
- Modern browser (Chrome/Edge/Firefox)
Quick version checks:
bash
git --version
dotnet --version
node -v
npm -v

Notes:
- No separate database server installation is required.
- App uses SQLite file database managed through EF Core migrations.

## Clone And Run (Fresh Machine)

1. Clone the repository and go inside it:

```bash
git clone <your-repo-url>
cd <your-repo-folder>
```

2. Start backend API (Terminal 1):

```bash
cd TaskBoard.Api
dotnet restore
dotnet tool restore
dotnet ef database update
dotnet run
```

3. Start frontend UI (Terminal 2):

```bash
cd task-board-ui
# .env is optional, but if it exists ensure it contains:
# VITE_API_BASE_URL=http://localhost:5201/api
npm install
npm run dev
```

4. Open the frontend URL shown in terminal (usually `http://localhost:5173`).

Notes:
- Backend usually runs at `http://localhost:5201` when started with `dotnet run`
- Frontend uses `VITE_API_BASE_URL` from `.env` (default points to backend above)

## Backend Setup

1. Open terminal in `TaskBoard.Api`
2. Run:

```bash
dotnet restore
dotnet tool restore
dotnet ef database update
dotnet run
```

The API runs locally (check terminal output, typically `http://localhost:5201` with this setup).

## Frontend Setup

1. Open terminal in `task-board-ui`
2. Ensure `task-board-ui/.env` exists and contains:

```bash
VITE_API_BASE_URL=http://localhost:5201/api
```

3. Install and run:

```bash
npm install
npm run dev
```

Frontend defaults to `http://localhost:5201/api`.

## Troubleshooting

- **Frontend loads but API calls fail**
  - Ensure backend is running (`dotnet run` in `TaskBoard.Api`).
  - Ensure `task-board-ui/.env` has:
    - `VITE_API_BASE_URL=http://localhost:5201/api`
  - Restart frontend dev server after `.env` changes.

- **`dotnet ef` command not found**
  - Run `dotnet tool restore` inside `TaskBoard.Api`.

## API Endpoints

- `GET /api/projects`
- `POST /api/projects`
- `GET /api/projects/{id}`
- `PUT /api/projects/{id}`
- `DELETE /api/projects/{id}`
- `GET /api/projects/{projectId}/tasks?status=&priority=&title=&sortBy=&sortDir=&page=&pageSize=`
- `POST /api/projects/{projectId}/tasks`
- `GET /api/tasks/{id}`
- `PUT /api/tasks/{id}`
- `DELETE /api/tasks/{id}`
- `GET /api/tasks/{taskId}/comments`
- `POST /api/tasks/{taskId}/comments`
- `DELETE /api/comments/{id}`
- `GET /api/dashboard`

## Task Statuses

Supported task statuses:
- `Todo`
- `InProgress`
- `Review`
- `Done`

## Bonus Features Implemented

- Debounced search:
  - Task board includes title search input with debounce before API call.
  - Uses `title` query parameter in tasks endpoint.

- Audit trail:
  - Status changes are stored in `TaskHistory` table (`old -> new`, timestamp).
  - History is returned with task details and shown on task detail page.
