## Project Description

Soundmates is a RESTful backend API for a musician-matching platform. Users register as either an individual `Artist` or a `Band`, set match preferences (location, age, gender, band size, tags), browse potential matches, like/dislike, and chat with mutual matches in real-time. Audio samples (MP3/MP4) and profile pictures (JPEG) are served as static files.

## Tech Stack

- **C# on .NET 10**
- ASP.NET Core (Minimal APIs) with EF Core and ASP.NET Core Identity
- SQL Server database, managed through migrations with EF Core
- SignalR for real-time events, MailKit for SMTP
- JWT bearer auth and cookie-based auth (dual "BearerOrCookie" policy scheme)
- CSRF protection via ASP.NET Core antiforgery
- Rate limiting (fixed-window, per-IP, 10 requests/minute) applied to auth endpoints
- OpenAPI via `Microsoft.AspNetCore.OpenApi` + Scalar UI (dev only)

### Testing project

- Integration tests with xUnit and libraries: Testcontainers, FluentAssertions, Respawn

## Architecture & Patterns

- **Vertical Slice Architecture** — each feature lives in `Features/<Domain>/<Feature>/` and contains a `*Endpoint.cs` (Minimal API handler) and where applicable `*Request.cs` (request record), `*Response.cs` (response record), and `*Validator.cs` (FluentValidation). Handlers return `IResult` via `TypedResults`.
- **Validation** — every request type that expects a request body has a companion FluentValidation `*Validator.cs`. The `ValidationFilter` endpoint filter (`Common/Filters/ValidationFilter.cs`) runs validation automatically before the handler body executes. Shared rules (password complexity, birth date range, GUID format) live in `Common/Validation/Rules/`.
- **Authorization** — a global fallback policy (`SetFallbackPolicy` in `Extensions/ServiceCollectionExtensions.cs`) requires an authenticated user on **every** endpoint by default; public endpoints must opt out explicitly with `.AllowAnonymous()` (all `Dictionaries/*` and most `Auth/*` endpoints do). The role-based `RequireAdmin` policy guards admin-only endpoints such as `BlockUser`.
- **Auth in endpoints** — protected endpoints inject `IAuthService` and call `GetAuthorizedUserAsync(HttpContext.User, checkForFirstLogin: true/false)`. It returns `null` (treat as `TypedResults.Unauthorized()`) when the user is missing, has an unconfirmed email, is deactivated (`IsActive == false`), or — when `checkForFirstLogin: true` (the default) — has not completed profile setup (`IsFirstLogin == true`).
- **CSRF** — mutation endpoints (POST/PUT/DELETE) that use cookie auth attach `ValidateCsrfTokenFilter` (`Common/Filters/ValidateCsrfTokenFilter.cs`). Clients first call `GET /auth/csrf-token` to receive the `XSRF-TOKEN` cookie and then include it as `X-CSRF-TOKEN` in subsequent requests.
- **Pagination** — endpoints that return lists accept `limit`/`offset` query parameters and validate them with `PaginationValidator` (`Common/Validation/PaginationValidator.cs`).
- **Entity IDs** — all entities derive from `EntityBase` (`Common/Entities/EntityBase.cs`), which provides an `Id` defaulting to `Guid.CreateVersion7()` (time-ordered v7 UUID) and a `CreatedAt` timestamp.
- **Profile state machine** — a fresh `User` has `IsFirstLogin = true` and incomplete profile fields; the profile update endpoint flips it. `IsBand` is null until set. `UserMatchPreference` is created with defaults at registration time.
- **Matching** — `Like` and `Dislike` both derive from the abstract `Reaction` entity and are unique on `(GiverId, ReceiverId)` (DB constraint). A `Match` is created when a `Like` is reciprocated. `Match.User1Id`/`User2Id` ordering is **not** canonicalized (`User1Id` is whoever liked back to complete the match, `User2Id` the original liker), so every match query must check both positions: `(User1Id == me && User2Id == other) || (User1Id == other && User2Id == me)`. Distance filtering uses the Haversine formula inlined directly in the `GetPotentialMatches*` endpoints — it must be EF-translatable, so it cannot be extracted into a helper method.
- **Static media** — uploads land under `wwwroot/images/` and `wwwroot/samples/` (paths in `ApplicationConstants`), served by `app.UseStaticFiles()`. Limits: 5 MB images, 100 MB samples, 5 of each per user.
- **Real-time** — `EventHub` (SignalR, `Common/Hubs/EventHub.cs`) is mapped at `/eventHub` and groups connections by `userId`. For WebSocket connections the JWT is read from the `access_token` query string parameter.
- **Error handling** — built-in `app.UseExceptionHandler()` + `AddProblemDetails()` catches all unhandled exceptions and returns a `ProblemDetails` response with a `traceId` extension field.

## Project Structure

```
SoundmatesAPI/
├── src/
│   └── Soundmates.Api/                  # Single project — all layers consolidated
│       ├── Common/
│       │   ├── Constants/
│       │   │   ├── ApplicationConstants.cs  # File limits, field lengths, etc.
│       │   │   └── SecurityConstants.cs     # Auth policy names, cookie names,
│       │   │                                #   CSRF header/cookie names, rate limit policy
│       │   ├── Entities/                # All EF Core entity classes (User, Artist, Band,
│       │   │                            #   Like, Dislike, Match, Message, Tag, etc.)
│       │   ├── Filters/
│       │   │   ├── ValidationFilter.cs      # Endpoint filter — runs FluentValidation
│       │   │   └── ValidateCsrfTokenFilter.cs  # Endpoint filter — validates CSRF token
│       │   ├── Helpers/                 # UserMediaUrlHelpers (builds absolute media URLs)
│       │   ├── Hubs/                    # EventHub — SignalR hub grouped by userId
│       │   ├── Options/                 # JwtOptions, AdminUserOptions,
│       │   │                            #   EmailSenderOptions, CorsOptions
│       │   ├── Services/                # IAuthService/AuthService (JWT, refresh tokens,
│       │   │                            #   email confirmation, password reset),
│       │   │                            #   IEmailService/EmailService/StubEmailService
│       │   └── Validation/
│       │       ├── PaginationValidator.cs
│       │       ├── GuidValidator.cs
│       │       └── Rules/               # BirthDateRules, PasswordRules, GuidRules
│       ├── Extensions/
│       │   ├── ServiceCollectionExtensions.cs  # AddConfigureAuthentication,
│       │   │                                   #   AddConfigureAuthorization,
│       │   │                                   #   AddConfigureIdentity,
│       │   │                                   #   AddConfigureOptions,
│       │   │                                   #   AddConfigureCors,
│       │   │                                   #   AddConfigureRateLimiting,
│       │   │                                   #   AddPersistence, AddEmailService,
│       │   │                                   #   AddConfigureOpenApi
│       │   ├── EndpointRouteBuilderExtensions.cs  # MapFeatureEndpoints (maps all routes)
│       │   └── WebApplicationExtensions.cs        # InitializeMigrateDatabaseAsync,
│       │                                          # SeedApplicationAdminUserAsync
│       ├── Features/                    # Vertical-slice endpoint folders
│       │   ├── Auth/                    # Login, Logout, Refresh, Register,
│       │   │                            #   ConfirmEmail, ResendEmailConfirmation,
│       │   │                            #   ForgotPassword, ResetPassword,
│       │   │                            #   ChangePassword, DeactivateAccount,
│       │   │                            #   CsrfToken, RevokeToken, RevokeAllTokens
│       │   ├── Dictionaries/            # GetCountries, GetCities, GetGenders,
│       │   │                            #   GetTags, GetTagCategories, GetBandRoles
│       │   ├── Matching/                # GetPotentialMatchesArtists/Bands,
│       │   │                            #   CreateLike, CreateDislike,
│       │   │                            #   GetMatches, MatchExists, Unmatch,
│       │   │                            #   GetMatchPreference, UpdateMatchPreference
│       │   ├── Messages/                # SendMessage, GetConversation,
│       │   │                            #   GetConversationsPreview, ViewConversation
│       │   ├── MusicSamples/            # UploadMusicSample, DeleteMusicSample
│       │   ├── ProfilePictures/         # UploadProfilePicture, DeleteProfilePicture
│       │   ├── Reports/                 # ReportUser, BlockUser (admin-only)
│       │   └── Users/                   # GetSelfProfile, GetOtherUserProfile,
│       │                                #   UpdateProfile
│       ├── Middleware/                  # LogRequestInfoMiddleware
│       ├── OpenApiTransformers/         # SecuritySchemesTransformer,
│       │                                #   AuthenticationTransformer,
│       │                                #   CsrfTokenHeaderTransformer (dev OpenAPI)
│       ├── Persistence/
│       │   ├── ApplicationDbContext.cs
│       │   ├── Configurations/          # IEntityTypeConfiguration per entity
│       │   ├── Migrations/              # EF Core-generated migration files
│       │   └── DataSeeding/             # SeedingScripts.cs (countries, cities, tags, etc.)
│       ├── appsettings.json             # Base config (logging)
│       ├── appsettings.Development.json # Connection string, JWT, SMTP, AdminUser, CORS
│       └── Program.cs                   # App entrypoint: DI, middleware, SignalR, static files
│
├── tests/                               # Test projects
|       ├── Auth/
|       ├── Common/                      # Common concerns, test helpers, CustomWebApplicationFactory.cs,
|       |                                # IntegrationTestBase.cs, TestConstants.cs, and more
│       ├── Dictionaries/
│       ├── Matching/
│       ├── Messages/
│       ├── MusicSamples/
│       ├── ProfilePictures/
│       ├── Reports/
│       └── Users/             
└── *.slnx                               # Solution file
```

## General Rules

- **Git Rules** - Treat git repository as read-only. You can read git state (git status, git diff, git log, git show, git branch, etc.) but **never** modify the repository. Some of the forbidden actions include: 
    - Creating, amending, or resetting commits (git commit, git commit --amend, git reset)
    - Modifying the staging area (git add, git restore --staged, git rm --cached)
    - Interacting with the remote (git push, git pull, git fetch)
    - Modifying .git/ content directly
    - Switching or creating branches (git checkout, git switch, git branch -d)
- **Database** The agent must never manage the database schema or state. The agent's job is only to write correct source code which will then be used by the user to generate related migrations manually. **Never** create or modify migration files or modify the database.

## C# Coding Rules

- Suffix async methods with `Async`
- Prefer record for DTOs and value objects, class for services and entities
- Use primary constructors for simple dependency injection
- Prefer sealed internal classes
- EF Core: always use AsNoTracking() for read-only queries