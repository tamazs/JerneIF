# Jerne IF Lotto System

## Overview

This project is a full-stack lottery management system built for **Jerne IF**.

It allows:

- **Players** to buy lottery boards, view transactions, and participate in games.  
- **Admins** to manage users, games, and transactions.

### Tech Stack

| Layer | Technology |
|-------|-------------|
| Frontend | React + TypeScript + TailwindCSS |
| Backend | .NET 9 (C#), Entity Framework Core |
| Database | Neon (PostgreSQL) |
| Deployment | Fly.io using Docker |
| CI/CD | GitHub Actions for build, test, lint, and deploy |

---

## Security & Authorization

The system uses **role-based authorization** both on the frontend and backend, with **JWT authentication** for secure access.

### Roles

| Role | Description |
|------|--------------|
| **Player** | Can buy boards, view their own boards and transactions. |
| **Admin** | Can manage users, transactions, and games. |

### Roles and permissions

Roles are stored as a string in `Users.Role` and must match the `UserRole` enum (`Admin` or `Player`).
New users are always created as `Player`, and an admin has to activate the account before it can log in.

**How a request is authorized**

1. At login the server puts the user id and role into a JWT that is valid for 30 minutes.
2. Controllers use `[Authorize]` (any logged-in user) or `[Authorize(Roles = "Admin")]` (admins only).
3. Endpoints that return "my" data take the user id from the token (`CurrentUserId`), never from the request.
4. Endpoints that take a user id in the request (`GetUserById`, `UpdateUser`) check ownership in `UserService`:
   a player can only read or change their own account, an admin can manage anyone.
5. `Role` and `IsActive` can only be changed by an admin. From a player they are ignored.
6. Approving or denying a transaction only works on a `Pending` transaction, and buying a board is serialized per user.

| Action | Player | Admin |
|--------|--------|-------|
| Read or edit own profile | yes | yes |
| Read or edit another user | no (403) | yes |
| Change a role or activate a user | no | yes |
| Buy boards, create a deposit, see own boards and transactions | yes | yes |
| List users, boards and transactions, approve or deny deposits, create games | no | yes |

The React route guards (`ProtectedRoute`, role checks in `Layout.tsx`) only hide pages. The server is what enforces access.

**Known trade-offs**

- The role is read from the JWT, so a role change or deactivation reaches an existing access token only when it expires (up to 30 minutes).
  Deactivated users cannot refresh their token.
- Access and refresh tokens are stored in `localStorage`, so an XSS bug on the site could steal them.
  Moving the refresh token to an httpOnly cookie would fix that, but the client and API are on different domains,
  so it needs cross-site cookie settings (`SameSite=None`, CORS with credentials) and CSRF handling. This has not been done yet.
- Login lockout is kept in server memory, so it is per instance and resets on restart.
- CORS allows only the origins in `AppOptions:AllowedOrigins` (default: local Vite dev server and the deployed client).
- `server/DataAccess/sql/001_users_role_check.sql` adds a database CHECK on `Users.Role`. It has to be run manually.

### Frontend Access Control

All routes are protected through the `ProtectedRoute` component.  
Unauthorized users are redirected to `/login`.

### Test Users

https://jerneif-frontend.fly.dev

| Role | Email | Password |
|------|--------|----------|
| Player | test@player.com | Playertest123 |
| Player | test@player2.com | Player2test123 |
| Admin | test@admin.com | Admintest123 |

---

## Environment & Configuration

### Backend

**Framework:** .NET 9  
**Database:** Neon (PostgreSQL)  
**ORM:** Entity Framework Core

#### Example `appsettings.json`

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AppOptions": {
    "DbConnectionString": "your connection string",
    "Token": "your JWT token key",
    "Issuer": "your token issuer",
    "Audience": "your token audience",
    "AllowedOrigins": ["https://your-client-origin"]
  },
  "AllowedHosts": "*"
}
```

#### Run Locally

```bash
dotnet restore
dotnet build
dotnet run
```

---

### Frontend

**Framework:** React + TypeScript + Vite  
**Styling:** TailwindCSS, DaisyUI  

#### Run Locally

```bash
npm ci
npm run dev
```

Then open the URL shown in the terminal (usually `http://localhost:5173`).

---

## Linting

The project uses **ESLint** for TypeScript + React (`eslint.config.js`).

Key rules:
- Based on official recommended rules.
- Accessibility rules are relaxed.

### Run Linter

```bash
npm run lint
```

Linting also runs automatically during the CI build.

---

## CI/CD

### Continuous Integration (GitHub Actions)

Triggered on:
- Push to `main`
- Pull requests to `main`

Jobs:
1. **Client build & lint**
   - Installs dependencies
   - Lints code
   - Builds production React app
2. **Server test**
   - Builds .NET solution
   - Runs xUnit tests

### Continuous Deployment (Fly.io)

Triggered after CI passes.  
Both the server and client have separate `fly.toml` files and Dockerfiles.

- **Server:** Deploys from `server/fly.toml`  
- **Client:** Deploys from `client/fly.toml`  
- Uses `--remote-only` so builds happen on Fly’s infrastructure.

---

## Current State of the Project

### Working Features

- User authentication via JWT  
- Role-based authorization  
- Player and Admin dashboards  
- Game creation and management  
- Automatic handling of repeating boards  
- Transaction approval flow  
- Integration with PostgreSQL (Neon)  
- CI/CD pipeline with build, test, and deploy automation  

### Known Issues

- **Repeating Boards Logic:**  
  When creating a new game, repeating boards are correctly reused.  
  However, the previous board’s repeat count also decreases, which makes the data look inconsistent in the database, though functionality is correct.

---


## Summary

The Jerne IF Lotto System is a stable, full-stack application with automated CI/CD, testing, and linting.  
Both Admin and Player workflows are fully implemented, with only minor display quirks in repeating board logic.  
