# 🎵 Soundmates API

![.NET](https://img.shields.io/badge/.NET-10.0-512BD4?logo=dotnet&logoColor=white)
![SQL Server](https://img.shields.io/badge/SQL_Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![Docker](https://img.shields.io/badge/Docker-Ready-2496ED?logo=docker&logoColor=white)

A social matching platform backend API designed for musicians and bands to connect. Find your next bandmate or discover bands looking for artists with similar musical interests.

---

## 📋 Table of Contents

- [🎵 Soundmates API](#-soundmates-api)
  - [📋 Table of Contents](#-table-of-contents)
  - [🎯 Introduction](#-introduction)
  - [🛠 Tech Stack](#-tech-stack)
  - [🏗 Architecture](#-architecture)
  - [✨ Key Features](#-key-features)
  - [⚙️ Configuration](#️-configuration)
  - [🤓 Prerequisites](#-prerequisites)
  - [🚀 Running the App](#-running-the-app)
  - [📚 API Documentation](#-api-documentation)
  - [📖 API Tutorial](#-api-tutorial)
    - [Authentication Flow](#authentication-flow)
    - [Profile Setup](#profile-setup)
    - [Matching Workflow](#matching-workflow)
    - [Validation Rules](#validation-rules)

---

## 🎯 Introduction

**Soundmates** is a RESTful API backend for a musician-matching platform. It enables individual artists and bands to create profiles, browse potential matches based on configurable preferences (genre, location, age), and communicate with mutual matches via real-time messaging.

The platform solves the common problem musicians face: finding like-minded collaborators in their area with compatible musical styles and goals.

---

## 🛠 Tech Stack

| Category | Technology |
|----------|------------|
| **Language** | C# on .NET 10 |
| **Framework** | ASP.NET Core (Minimal APIs) |
| **Database** | SQL Server |
| **ORM** | Entity Framework Core |
| **Identity** | ASP.NET Core Identity |
| **Authentication** | JWT Bearer + Cookie (dual policy scheme) |
| **Security** | CSRF protection (antiforgery), rate limiting |
| **Real-Time Communication** | SignalR |
| **API Documentation** | OpenAPI + Scalar UI (dev only) |
| **Email Service** | MailKit |
| **Containerization** | Docker & Docker Compose |

---

## 🏗 Architecture

This project follows **Vertical Slice Architecture** — all code for a feature lives together in `Features/<Domain>/<Feature>/`. There is a single consolidated project with no layer separation.

```
SoundmatesAPI/
├── src/
│   └── Soundmates.Api/           # Single project — all layers consolidated
│       ├── Common/               # Shared entities, filters, helpers, options, services
│       ├── Extensions/           # DI registrations and endpoint mapping
│       ├── Features/             # Vertical slices (Auth, Matching, Messages, Users, ...)
│       ├── Middleware/           # Request logging
│       ├── OpenApiTransformers/  # Dev-only OpenAPI security configuration
│       └── Persistence/          # EF Core DbContext, configurations, migrations, seeding
└── tests/
```

Each feature slice contains a `*Endpoint.cs` (Minimal API handler) and, where applicable, `*Request.cs`, `*Response.cs`, and `*Validator.cs` (FluentValidation).

---

## ✨ Key Features

- **🔐 Authentication** - JWT bearer + cookie auth, access tokens (15 min) and refresh tokens (7 days), token rotation and revocation
- **📧 Email Confirmation** - Account activation via email link; password reset via email
- **🛡 Security** - CSRF protection, rate limiting on auth endpoints, account lockout after failed attempts
- **👥 User Profiles** - Support for both individual artists and bands
- **💘 Smart Matching** - Like/Dislike system with configurable preferences (location, age, gender, band size, tags)
- **💬 Real-Time Messaging** - SignalR-powered chat between matched users
- **🎵 Music Samples** - Upload and manage audio samples (MP3/MP4, up to 100 MB)
- **📸 Profile Pictures** - Image upload support (JPEG, up to 5 MB)
- **🔍 Advanced Filtering** - Filter potential matches by distance, gender, band size, and tags
- **📧 Email Notifications** - SMTP integration via MailKit
- **🚨 Reporting & Moderation** - Users report inappropriate profiles; admins block (deactivate) accounts
- **📖 Data Dictionaries** - Centralized lookups for countries, cities, genders, and tags
- **🏥 Health Checks** - Database health endpoint at `/health`

---

## ⚙️ Configuration

Application configuration lives inside [`appsettings.Development.json`](src/Soundmates.Api/appsettings.Development.json) file as well as [`ApplicationConstants.cs`](src/Soundmates.Api/Common/Constants/ApplicationConstants.cs) and [`SecurityConstants.cs`](src/Soundmates.Api/Common/Constants/SecurityConstants.cs).

| Setting | Description |
|---------|-------------|
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `EmailSender` | SMTP configuration for email notifications; set `UseStubEmailSender: true` to skip actual sending (logs to console instead) |
| `AdminUser` | Credentials for the seeded admin account |
| `Jwt` | JWT issuer, audience, access-token expiration, refresh-token lifetime, and signing key (min 32 chars) |
| `Cors:AllowedOrigins` | Allowed origins for CORS requests |

> **Note:** The base URLs for client redirect links (email confirmation and password reset) are defined as constants in [`SecurityConstants.cs`](src/Soundmates.Api/Common/Constants/SecurityConstants.cs) (`ConfirmEmailEndpointClientPath`, `ResetPasswordEndpointClientPath`), separate from the CORS configuration.

---

## 🤓 Prerequisites
- [Docker](https://www.docker.com)

---

## 🚀 Running the App

The app auto-applies pending migrations and seeds the admin user on startup (development only for migrations).

```bash
# Build and start all services
docker compose up --build -d

# Stop the application
docker compose down

or

# Stop the application and also remove created docker volumes
docker compose down -v
```

**By default the API is accessible at `https://localhost:8443`**

---

## 📚 API Documentation

Interactive API documentation is available in development mode:

| Resource | URL |
|----------|-----|
| **Scalar UI** | `https://localhost:<port>/scalar/v1` |
| **OpenAPI JSON** | `https://localhost:<port>/openapi/v1.json` |
| **Health Check** | `https://localhost:<port>/health` |

**The default <port> is 8443 for https scheme or 8080 for http (automatically redirects to https).**

---

## 📖 API Tutorial

### Authentication Flow

1. **Register** - `POST /auth/register` with email and password
2. **Confirm email** - click the link sent to the registered address (`POST /auth/confirm-email`)
3. **Login** - `POST /auth/login` — returns access token (15 min) and sets refresh token cookie (7 days)
4. **Access protected endpoints** - include `Authorization: Bearer <access_token>` header, or rely on the auth cookie
5. **Refresh tokens** - `POST /auth/refresh` when access token expires (rotates refresh token)
6. **Logout** - `POST /auth/logout` to revoke the current refresh token

> **Important** When using cookie auth, every state-mutating endpoint (so basically every POST/PUT/DELETE) is protected against CSRF. You need to first get a CSRF token by calling the `GET /auth/csrf-token` endpoint. This endpoint sets required `XSRF-TOKEN` cookie and returns a token in the response body that needs to be included in a `X-CSRF-TOKEN` header.

> **Note** By default in development, the email stub service is used — rather than sending actual emails, it logs them to the console. This behavior is configurable as described in [Configuration](#️-configuration)

### Profile Setup

After email confirmation, the profile is **incomplete** (`IsFirstLogin = true`). Complete profile setup via `PUT /users/profile` to unlock full functionality.

### Matching Workflow

1. **Configure preferences** - `PUT /matching/match-preference` to set filters
2. **Browse artists** - `GET /matching/artists` returns potential artist matches
3. **Browse bands** - `GET /matching/bands` returns potential band matches
4. **Like/Dislike** - `POST /matching/like` or `POST /matching/dislike`
5. **Mutual match** - When both users like each other, a match is created
6. **Chat** - Send messages via `POST /messages` to matched users

### Validation Rules

All constraints are defined in [`Common/Constants/ApplicationConstants.cs`](src/Soundmates.Api/Common/Constants/ApplicationConstants.cs) and [`Common/Constants/SecurityConstants.cs`](src/Soundmates.Api/Common/Constants/SecurityConstants.cs) so that they are easily configurable:

| Rule | Value |
|------|-------|
| Password length | 8-32 characters |
| Max music samples | 5 per user |
| Max sample size | 100 MB |
| Max profile pictures | 5 per user |
| Max image size | 5 MB |
| Message length | Up to 4000 characters |

**Additional password requirements:**
- Lowercase letter
- Uppercase letter
- Digit
- Special character
- Standard printable ASCII characters only
