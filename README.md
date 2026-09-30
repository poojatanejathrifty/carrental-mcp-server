# CarRental Solution

Clean Architecture ASP.NET Core (.NET 9) solution for a car rental system, with two
composition roots — a REST API and an MCP server — sharing the same Application layer.

## Project layout and dependency rules

```
CarRental.Domain            entities + enums, no dependencies
      ^
CarRental.Application       interfaces (services + repositories), DTOs, service logic, validators
      ^
CarRental.Infrastructure    implements the repository interfaces using Dapper + stored procedures
      ^                     (the only project allowed to talk to SQL Server)
      |
      +-- CarRental.Api     composition root: wires Infrastructure into Application via DI,
      |                     controllers depend only on Application service interfaces
      |
      +-- CarRental.MCP     composition root: wires Infrastructure into Application via DI,
                             MCP tools depend only on Application service interfaces
```

Dependency inversion: `Application` declares `IVehicleRepository`, `IAvailabilityRepository`,
`IPricingRepository`, `ICustomerRepository`, `IBookingRepository`, `IPaymentRepository` and the
service interfaces (`IVehicleService`, `IAvailabilityService`, `IPricingService`,
`ICustomerService`, `IBookingService`, `IPaymentService`). `Infrastructure` implements the
repository interfaces; nothing in `Api` or `MCP` controller/tool code ever opens a
`SqlConnection` or references Dapper — that only happens inside
`CarRental.Infrastructure/Persistence/SqlDataAccess.cs`, the single centralized data-access
class every repository goes through. Stored procedure names are centralized in
`CarRental.Infrastructure/Persistence/StoredProcedures.cs`.

Domain entities (`Vehicle`, `Customer`, `Booking`, ...) never cross the API boundary — every
endpoint and MCP tool returns a DTO from `CarRental.Application/DTOs`.

## Database

SQL scripts live under `database/`:

- `database/Tables/001_CreateTables.sql` — schema
- `database/StoredProcedures/*.sql` — one file per entity, all data access goes through these
- `database/Deploy.sql` — runs everything in order via `sqlcmd`

```
sqlcmd -S (localdb)\MSSQLLocalDB -d CarRentalDb -i database/Deploy.sql
```

⚠️ **`001_CreateTables.sql` describes a clean sample schema, but the database currently in
use for real testing (`MPDBTDX`) is a pre-existing, unrelated real-world schema instead**
(`FleetMaster`, `AGREEMENTS`, `LOCATION` in place of `dbo.Vehicle`/`Booking`/etc.). Stored
procedures are migrated to the real schema one at a time via column aliasing, so different
procedures currently query different schemas - see the migration status table and real-data
quirks (trailing-space-padded codes, `GM_LocationType` meaning, etc.) in `CLAUDE.md` before
touching anything under `database/StoredProcedures/`.

## Configuration and secrets

The connection string is read from `ConnectionStrings:CarRentalDb`, resolved in this order:

1. `appsettings.json` (kept blank — committed, safe to share)
2. `appsettings.{Environment}.json` — e.g. `appsettings.Development.json`. **This file holds the
   real per-developer connection string and is git-ignored** (see `.gitignore`), so it's never
   committed and never leaves this machine via source control. A checked-in
   `appsettings.Development.json.example` in each project shows the expected shape with
   placeholder values — copy it to `appsettings.Development.json` and fill in your own.
3. Environment variable `ConnectionStrings__CarRentalDb` (double underscore) — use this in
   staging/production instead of a file, set at the hosting platform's secret/config layer.

Both `CarRental.Api` and `CarRental.MCP` load `appsettings.Development.json` automatically when
running as the `Development` environment (`ASPNETCORE_ENVIRONMENT` / `DOTNET_ENVIRONMENT` —
`.mcp.json` sets this for the MCP server). `CarRental.MCP/Program.cs` explicitly anchors its
content root to the executable's own folder (`AppContext.BaseDirectory`), so config loading
works correctly regardless of the working directory the MCP client launches it from — a
published console app can't rely on the process's current directory for this.

**Never** put a real password in `appsettings.json` (the non-environment-specific one) or in
anything under `bin/`/`obj/`/`publish/` that isn't itself git-ignored — those are exactly the
locations that get built, zipped, or shipped without anyone specifically intending to share a
secret.

⚠️ Whatever value you put in `appsettings.Development.json`, treat it as a live secret from the
moment it's typed into a terminal, chat, or ticket — rotate it if it's ever been pasted
somewhere outside the file itself.

**Before running `database/Deploy.sql` against any non-empty/shared database**, confirm the
target first — it creates tables and stored procedures that could collide with an existing
schema. Prefer a dedicated database for this solution over reusing one that already serves
another system.

## Running

```
dotnet build CarRental.sln

# REST API — Swagger UI at /swagger, health checks at /health/live and /health/ready
dotnet run --project src/CarRental.Api

# MCP server (stdio transport)
dotnet run --project src/CarRental.MCP
```

## MCP client registration

`.mcp.json` at the repo root registers `CarRental.MCP` as a project-scoped MCP server for
Claude Code, pointing at the **published** executable (not `dotnet run`, which re-restores/
rebuilds on every launch — slow and unnecessary for a process a client spawns repeatedly):

```json
{
  "mcpServers": {
    "carrental": {
      "command": "C:\\PoojaWorkSpace\\VSMCPServer\\publish\\CarRental.MCP\\CarRental.MCP.exe",
      "env": { "DOTNET_ENVIRONMENT": "Development" }
    }
  }
}
```

Claude Code only picks up `.mcp.json` changes on startup/reload — restart Claude Code (or
reload the window) after adding or editing it before the `carrental` tools appear.

Re-publish after any code change before the client will see it:

```
dotnet publish src/CarRental.MCP -c Release -o publish/CarRental.MCP
```

For Claude Desktop or VS Code's MCP extension, use the same `command`/`env` shape in their
respective config files (`claude_desktop_config.json` / `.vscode/mcp.json`).

## Cross-cutting concerns (Api)

- Serilog structured console logging (`Program.cs`, `appsettings.json`)
- Centralized exception handling via `IExceptionHandler` (`Middleware/GlobalExceptionHandler.cs`)
  mapping `NotFoundException` → 404, `ConflictException` → 409, `BusinessRuleException` /
  `ValidationException` → 400, everything else → 500, all as RFC 9110 `ProblemDetails`
- FluentValidation auto-validation on every request DTO
- Swagger/OpenAPI (`/swagger`)
- Health checks: `/health/live` (process liveness) and `/health/ready` (SQL Server dependency)
