using CarRental.Application.Interfaces.Repositories;
using CarRental.Infrastructure.Configuration;
using CarRental.Infrastructure.Persistence;
using CarRental.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CarRental.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(options =>
        {
            configuration.GetSection(DatabaseOptions.SectionName).Bind(options);

            // Connection string is resolved from configuration's ConnectionStrings section,
            // which is populated from appsettings, user-secrets, or the
            // ConnectionStrings__CarRentalDb environment variable at startup.
            options.ConnectionString = configuration.GetConnectionString("CarRentalDb")
                ?? options.ConnectionString;
        });

        services.AddSingleton<ISqlDataAccess, SqlDataAccess>();

        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IAvailabilityRepository, AvailabilityRepository>();
        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<IPricingRepository, PricingRepository>();
        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }
}
