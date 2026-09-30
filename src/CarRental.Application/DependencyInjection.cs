using CarRental.Application.Interfaces.Services;
using CarRental.Application.Services;
using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace CarRental.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IVehicleService, VehicleService>();
        services.AddScoped<IAvailabilityService, AvailabilityService>();
        services.AddScoped<ILocationService, LocationService>();
        services.AddScoped<IPricingService, PricingService>();
        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IBookingService, BookingService>();
        services.AddScoped<IPaymentService, PaymentService>();

        services.AddValidatorsFromAssemblyContaining(typeof(DependencyInjection));

        return services;
    }
}
