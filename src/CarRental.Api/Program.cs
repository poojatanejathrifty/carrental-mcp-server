using CarRental.Api.Middleware;
using CarRental.Application;
using CarRental.Infrastructure;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.OpenApi;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, loggerConfiguration) => loggerConfiguration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext());

    // Application and Infrastructure DI - this composition root is the only place
    // that wires concrete stored-procedure repositories into the service interfaces.
    builder.Services.AddApplicationServices();
    builder.Services.AddInfrastructureServices(builder.Configuration);

    builder.Services.AddControllers();
    builder.Services.AddFluentValidationAutoValidation();

    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo
        {
            Title = "CarRental API",
            Version = "v1",
            Description = "Car rental management API - vehicles, availability, pricing, customers, bookings and payments."
        });
    });

    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var connectionString = builder.Configuration.GetConnectionString("CarRentalDb")
        ?? throw new InvalidOperationException("Connection string 'CarRentalDb' is not configured.");

    builder.Services.AddHealthChecks()
        .AddSqlServer(connectionString, name: "sqlserver", tags: ["ready"]);

    var app = builder.Build();

    app.UseSerilogRequestLogging();
    app.UseExceptionHandler();

    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options => options.SwaggerEndpoint("/swagger/v1/swagger.json", "CarRental API v1"));
    }

    app.UseHttpsRedirection();
    app.MapControllers();

    app.MapHealthChecks("/health/live", new HealthCheckOptions
    {
        Predicate = _ => false
    });

    app.MapHealthChecks("/health/ready", new HealthCheckOptions
    {
        Predicate = check => check.Tags.Contains("ready"),
        ResponseWriter = HealthCheckUiWriter.WriteResponse
    });

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)
{
    Log.Fatal(ex, "CarRental.Api terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}

internal static class HealthCheckUiWriter
{
    public static Task WriteResponse(HttpContext context, HealthReport report)
    {
        context.Response.ContentType = "application/json";
        var payload = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(entry => new
            {
                name = entry.Key,
                status = entry.Value.Status.ToString(),
                description = entry.Value.Description
            }),
            totalDurationMs = report.TotalDuration.TotalMilliseconds
        };

        return context.Response.WriteAsJsonAsync(payload);
    }
}
