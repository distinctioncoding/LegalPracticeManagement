# Legal Practice Management System — Backend (Phase 1)

ASP.NET Core Web API for the law-firm practice management system described in the BRD
(Vol 1), Architecture (Vol 2), and Functional Spec (Vol 3). Built as a **modular monolith**
exactly as Volume 2 prescribes — deliberately simple enough for a junior full-stack team.

> **Status:** This repo is an *initial code base + one reference feature*. The **Client
> module** is implemented end-to-end (API → Application → Domain → Infrastructure → tests)
> to show the pattern every other module should follow. Other modules (Intake, Matter,
> Tasks, Deadlines, Documents…) are intentionally **not** built yet.

## Tech stack

| Concern         | Choice                                             |
|-----------------|----------------------------------------------------|
| Backend         | ASP.NET Core Web API (.NET 9)                      |
| Data            | EF Core + **Azure SQL** (SQL Server provider)      |
| Auth            | ASP.NET Identity                                   |
| Files (later)   | Azure Blob Storage                                 |
| Validation      | Plain code inside the services                     |
| Docs            | Swagger UI                                         |

## Solution structure (Volume 2 §5.2)

```
LegalPracticeManagement.sln
└── src/
    ├── LawFirm.Api             # Controllers, middleware, DI, Swagger — the host
    ├── LawFirm.Application     # Feature services, DTOs, mapping (business logic)
    ├── LawFirm.Domain          # Entities, enums, constants (no framework dependencies)
    ├── LawFirm.Infrastructure  # EF Core DbContext, configs, migrations, helpers
    └── LawFirm.Shared          # Result wrapper, pagination, cross-cutting primitives
```

Dependency direction (arrows point to *references*):

```
Api ─▶ Application ─▶ Infrastructure ─▶ Domain ─▶ Shared
```

Application services use the concrete `LawFirmDbContext` directly (no repository or
`IApplicationDbContext` abstraction — Volume 2 §5.4), so Application references
Infrastructure. Infrastructure does **not** reference Application, keeping the graph acyclic.

In the **Application** layer, code is organized **by feature** (`Features/Clients/…`) so
everything for a feature stays together (Volume 2 §6.2). In the **Api** layer, controllers
live in a flat `Controllers/` folder.

## Running it locally

The development profile uses an **in-memory database** (`Database:Provider = InMemory`), so no
SQL Server is required. Authentication is not wired up yet, so endpoints are open — just:

```bash
cd src/LawFirm.Api
dotnet run
```

Then open **Swagger** at the URL printed in the console (e.g. `http://localhost:5083/swagger`).
Three demo clients are seeded automatically and you can call every endpoint directly.

> Because there is no authenticated user yet, the `CreatedBy`/`UpdatedBy` audit fields are
> stamped as `system`.

## Running against real Azure SQL

1. In `appsettings.json` (or environment/Key Vault) set:
   - `Database:Provider = SqlServer`
   - `ConnectionStrings:DefaultConnection` → your Azure SQL connection string
2. Apply migrations:
   ```bash
   dotnet ef database update --project src/LawFirm.Infrastructure --startup-project src/LawFirm.Api
   ```
3. `dotnet run`.

> **Auth is deferred.** Microsoft Entra ID (bearer-token validation) and role-based
> restrictions are planned but intentionally not wired up yet — add them before any non-local
> environment.

## Where to go next

Follow the delivery order in Volume 3 §17. The next feature (Intake or Reference Data) should
copy the Client module's shape: a `Features/<Name>` folder in Application with DTOs +
`I<Name>Service`/`<Name>Service` (validating input in the service), an entity + EF
configuration in Infrastructure, a migration, and a thin controller in the Api.
```
