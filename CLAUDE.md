# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```
# Build everything
dotnet build CarRental.sln

# Run the REST API (Swagger at /swagger, health at /health/live and /health/ready)
dotnet run --project src/CarRental.Api

# Run the MCP server directly (stdio transport - it just sits idle waiting on stdin
# unless something speaks MCP JSON-RPC to it; needs DOTNET_ENVIRONMENT=Development
# set manually since nothing sets it when run this way)
DOTNET_ENVIRONMENT=Development dotnet run --project src/CarRental.MCP

# Call one MCP tool directly without any MCP client, for fast manual testing/debugging
# (spawns the published exe, not `dotnet run`, and drives it over stdio):
./test-mcp-call.ps1 -ListTools
./test-mcp-call.ps1 -ToolName list_vehicles
./test-mcp-call.ps1 -ToolName get_vehicle -ArgsJson '{"vehicleId":48213}'

# Publish the MCP server (this, not `dotnet run`, is what Claude Desktop/Code actually
# spawns - it re-restores/rebuilds on every launch otherwise). Must be run with Claude
# Desktop fully closed first, or the copy fails - the running process locks the DLLs.
dotnet publish src/CarRental.MCP -c Release -o publish/CarRental.MCP

# Deploy one changed stored procedure file to the real database (CREATE OR ALTER is
# idempotent, safe to rerun). Editing a .sql file under database/ changes nothing by
# itself - the app only ever calls a stored procedure by name against whatever is
# currently deployed in the server's catalog, never reads these files.
sqlcmd -S <server> -d <database> -U <user> -P <password> -i database/StoredProcedures/<File>.sql -C

# Full local DB bootstrap (schema + all stored procedures, in dependency order)
sqlcmd -S (localdb)\MSSQLLocalDB -d CarRentalDb -i database/Deploy.sql
```

There is no test project in the solution and no lint config - `dotnet build` is the only
verification step available.

**After changing anything under `src/CarRental.MCP` or `database/StoredProcedures/`,
both a republish AND a redeploy are required, and Claude Desktop must be fully quit and
reopened (not just the chat window closed) before it picks up either change** - this is
the most common source of "I fixed it but it's still broken" confusion in this repo.

### Debugging the MCP server

MCP clients spawn `CarRental.MCP.exe` as a child process over stdio, so there's no normal
"run and see output" loop. To set breakpoints: run `dotnet run --project src/CarRental.MCP`
in one terminal (leave it running), find its PID (`tasklist | findstr CarRental.MCP`), then
in VS Code use `Debug: Select and Start Debugging` -> `.NET Core Attach` -> pick that PID.
Drive it by pasting raw JSON-RPC lines into that same terminal (it's the process's actual
stdin):
```
{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"2025-06-18","capabilities":{},"clientInfo":{"name":"manual-test","version":"1.0"}}}
{"jsonrpc":"2.0","method":"notifications/initialized"}
{"jsonrpc":"2.0","id":2,"method":"tools/call","params":{"name":"list_vehicles","arguments":{}}}
```
If more than one `CarRental.MCP.exe` is running, `processName`-based attach configs fail
with "multiple processes found" - kill stragglers (`taskkill /F /IM CarRental.MCP.exe`) or
attach by exact PID instead.

Claude Desktop's own logs for this server are at
`%LOCALAPPDATA%\Claude\logs\mcp-server-carrental.log` (per-server) and
`%LOCALAPPDATA%\Claude\logs\mcp.log` (general) - check these first for the real exception
behind any "generic error" Claude reports, before guessing.

## Architecture

Clean Architecture with two composition roots sharing one Application layer:

```
CarRental.Domain            entities + enums, no dependencies
      ^
CarRental.Application       interfaces (services + repositories), DTOs, service logic, validators
      ^
CarRental.Infrastructure    implements repository interfaces via Dapper + stored procedures
      ^                     (the only project allowed to open a SqlConnection)
      |
      +-- CarRental.Api     composition root: REST controllers -> Application service interfaces
      +-- CarRental.MCP     composition root: MCP tools -> Application service interfaces
```

- Every repository call goes through `CarRental.Infrastructure/Persistence/SqlDataAccess.cs`,
  the single centralized data-access class - nothing else references Dapper or
  `SqlConnection` directly.
- Stored procedure names are centralized as constants in
  `CarRental.Infrastructure/Persistence/StoredProcedures.cs`, grouped by entity. Repositories
  reference these constants, never hardcoded SP name strings.
- Domain entities never cross the API/MCP boundary - every controller and MCP tool returns a
  DTO from `CarRental.Application/DTOs`, mapped from the domain entity inside the relevant
  `*Service` class. Repository and Service are deliberately separate interfaces per entity
  (e.g. `IVehicleRepository` vs `IVehicleService`) specifically so this DTO-shaping logic has
  a home that isn't the SQL-facing layer.
- MCP tools are plain static methods tagged `[McpServerToolType]` / `[McpServerTool(Name = "...")]`
  in `CarRental.MCP/Tools/*.cs`, discovered by reflection via `WithToolsFromAssembly()` in
  `Program.cs`. The tool's `Name` is matched by exact string against the incoming
  `tools/call` request - there's no fuzzy matching at that layer. Natural-language ->
  tool-and-argument selection happens entirely on the MCP *client* side (Claude reading each
  tool's `[Description]` and its parameter DTO's shape), not in this server.
- `CarRental.MCP/Program.cs` anchors its content root to `AppContext.BaseDirectory` rather
  than the process's working directory, since an MCP client can spawn it from anywhere.
  Config loading (`appsettings.Development.json`) breaks silently if this is ever changed.
- `appsettings.Development.json` holds the real per-developer connection string and is
  git-ignored; `appsettings.json` is committed and kept blank. Production/staging supplies
  the connection string via the `ConnectionStrings__CarRentalDb` environment variable instead
  of a file.

### The real database is not the sample schema - migration is partial and per-procedure

`database/Tables/001_CreateTables.sql` defines a clean sample schema (`dbo.Vehicle`,
`dbo.Booking`, `dbo.BookingItem`, `dbo.Customer`, `dbo.Payment`, `dbo.RentalRate`). The
actual target database in current use (a real fleet system, connection string points at
`MPDBTDX`) has a completely different, pre-existing schema instead: `FleetMaster`,
`AGREEMENTS`, `LOCATION`. Stored procedures are migrated to the real schema one at a time,
via column aliasing in the `SELECT` so the C# side never changes - the **stored procedure**
is the only place that knows which schema it's actually querying. As of now:

| Procedure | Backing schema |
|---|---|
| `usp_Vehicle_GetAll` | Real (`FleetMaster`) |
| `usp_Vehicle_GetById`, `usp_Vehicle_GetByLocation`, `usp_Vehicle_Create/Update/UpdateStatus/Delete` | Sample schema still - **will fail against the real database** |
| `usp_Availability_GetAvailableVehicles` | Real (`FleetMaster` + `AGREEMENTS` + `LOCATION`) |
| `usp_Availability_IsVehicleAvailable` | Sample schema still - **will fail against the real database** |
| `usp_Location_GetAll` | Real (`LOCATION`) |
| Customer / Pricing / Booking / Payment procedures | Unverified against the real schema |

When adding or fixing a procedure against the real schema, expect these data quirks (already
worked around where migrated, but easy to reintroduce):
- `LOCATION.Code` and `FleetMaster.LCN_RevenueLocation` are fixed-width and padded with
  trailing spaces (`'DA3 '`, not `'DA3'`). `=` comparisons in SQL Server ignore trailing
  space padding, but `RIGHT()`/`LEN()`/string-building do not - always `RTRIM()` before
  using either.
- `GM_LocationType` does not mean "rentable" for `'R'` - airport terminals (e.g. `DA1`/`DA2`/
  `DA3`) are typed `'AP'`, other airports are `'OW'`. There is no confirmed business rule yet
  for which types are valid pickup locations, so no type filter is applied in
  `usp_Location_GetAll`/`usp_Availability_GetAvailableVehicles` - don't reintroduce one
  without confirming the real rule first.
- Location codes are not all 3 characters (e.g. `'2NDA'`) - don't assume a fixed width when
  declaring `varchar(n)` locals or table variables for them.
- `search_available_vehicles`'s `@Location` parameter accepts either an exact code or free
  text (`usp_Availability_GetAvailableVehicles` in `database/StoredProcedures/Availability.sql`
  resolves free text by matching every word against `LOCATION.Description`, with optional
  trailing-number narrowing for terminal numbers) - this is a heuristic, not real NLP, and
  won't handle spelled-out numbers or abbreviations. `list_locations` exists separately for
  browsing/disambiguating.
- `usp_Availability_GetAvailableVehicles` accepts `@PickupDateUtc`/`@ReturnDateUtc` but does
  not yet implement a real date-overlap check against `AGREEMENTS` - it only filters on
  "not currently on an active agreement," which is not the same as "free for this date range."
