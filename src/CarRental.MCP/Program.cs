using CarRental.Application;
using CarRental.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

// MCP clients spawn this process with whatever working directory they happen to use
// (often the workspace root, not this exe's own folder), but appsettings.*.json ship
// next to the exe - so the content root must be anchored there explicitly, or config
// loading silently finds nothing depending on who launched it.
var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

// appsettings.Development.json (loaded automatically when DOTNET_ENVIRONMENT=Development)
// holds the real local-dev connection string and is gitignored - never committed.
// Production/staging instead supply ConnectionStrings__CarRentalDb as an environment
// variable rather than shipping a credentials file.

// Stdio is the MCP transport's wire format - only protocol frames may hit stdout,
// so all diagnostic logging is redirected to stderr.
builder.Logging.ClearProviders();
builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

// Composition root: CarRental.MCP wires the concrete stored-procedure repositories
// (CarRental.Infrastructure) into CarRental.Application's service interfaces here.
// Every tool below only ever calls an Application service interface - it never
// touches SqlConnection, Dapper, or SQL Server directly.
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

await builder.Build().RunAsync();
