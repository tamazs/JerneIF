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
    "Audience": "your token audience"
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
  However, the previous board’s repeat count also decreases, which makes the data look inconsistent in the database — though functionality is correct.

---


## Summary

The Jerne IF Lotto System is a stable, full-stack application with automated CI/CD, testing, and linting.  
Both Admin and Player workflows are fully implemented, with only minor display quirks in repeating board logic.  
