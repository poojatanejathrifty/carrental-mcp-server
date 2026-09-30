using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Interfaces.Services;
using CarRental.Domain.Entities;

namespace CarRental.Application.Services;

public class AvailabilityService(IAvailabilityRepository availabilityRepository) : IAvailabilityService
{
    public async Task<IReadOnlyList<VehicleDto>> SearchAvailableVehiclesAsync(AvailabilitySearchRequest request, CancellationToken cancellationToken = default)
    {
        var vehicles = await availabilityRepository.GetAvailableVehiclesAsync(
            request.Location,
            request.PickupDateUtc,
            request.ReturnDateUtc,
            request.VehicleCategoryId,
            cancellationToken);

        return vehicles.Select(ToDto).ToList();
    }

    public Task<bool> IsVehicleAvailableAsync(int vehicleId, DateTime pickupDateUtc, DateTime returnDateUtc, CancellationToken cancellationToken = default)
        => availabilityRepository.IsVehicleAvailableAsync(vehicleId, pickupDateUtc, returnDateUtc, cancellationToken);

    private static VehicleDto ToDto(Vehicle vehicle) => new()
    {
        FleetCode              = vehicle.FleetCode,
        FleetDescription       = vehicle.FleetDescription,
        CurrentLocationId      = vehicle.CurrentLocationId,
        HireType               = vehicle.HireType,
        GroupCode              = vehicle.GroupCode,
        Make                   = vehicle.Make,
        Model                  = vehicle.Model,
        Color                  = vehicle.Color,
        BrandCode              = vehicle.BrandCode,
        Year                   = vehicle.Year,
        EngineCapacity         = vehicle.EngineCapacity,
        VehicleType            = vehicle.VehicleType,
        LicensePlate           = vehicle.LicensePlate
    };
}
