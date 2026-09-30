using CarRental.Application.Common;
using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Repositories;
using CarRental.Application.Interfaces.Services;
using CarRental.Domain.Entities;
using CarRental.Domain.Enums;
using Microsoft.Extensions.Logging;

namespace CarRental.Application.Services;

public class VehicleService(IVehicleRepository vehicleRepository, ILogger<VehicleService> logger) : IVehicleService
{
    public async Task<VehicleDto> GetByIdAsync(int vehicleId, CancellationToken cancellationToken = default)
    {
        var vehicle = await vehicleRepository.GetByIdAsync(vehicleId, cancellationToken)
            ?? throw new NotFoundException(nameof(Vehicle), vehicleId);

        return ToDto(vehicle);
    }

    public async Task<IReadOnlyList<VehicleDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var vehicles = await vehicleRepository.GetAllAsync(cancellationToken);
        return vehicles.Select(ToDto).ToList();
    }

    public async Task<IReadOnlyList<VehicleDto>> GetByLocationAsync(int locationId, CancellationToken cancellationToken = default)
    {
        var vehicles = await vehicleRepository.GetByLocationAsync(locationId, cancellationToken);
        return vehicles.Select(ToDto).ToList();
    }

    public async Task<VehicleDto> CreateAsync(CreateVehicleRequest request, CancellationToken cancellationToken = default)
    {
        var vehicle = new Vehicle
        {
            VehicleCategoryId = request.VehicleCategoryId,
            CurrentLocationId = request.CurrentLocationId.ToString(),
            Make = request.Make,
            Model = request.Model,
            Year = request.Year,
            LicensePlate = request.LicensePlate,
            Vin = request.Vin,
            Color = request.Color,
            Odometer = request.Odometer,
            Status = VehicleStatus.Available
        };

        vehicle.VehicleId = await vehicleRepository.CreateAsync(vehicle, cancellationToken);
        logger.LogInformation("Created vehicle {VehicleId} with plate {LicensePlate}", vehicle.VehicleId, vehicle.LicensePlate);

        return ToDto(vehicle);
    }

    public async Task DeleteAsync(int vehicleId, CancellationToken cancellationToken = default)
    {
        var deleted = await vehicleRepository.DeleteAsync(vehicleId, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException(nameof(Vehicle), vehicleId);
        }
    }

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
        EngineCapacity         = vehicle.EngineCapacity ?? string.Empty,
        VehicleType            = vehicle.VehicleType ?? string.Empty,
        LicensePlate           = vehicle.LicensePlate ?? string.Empty

    };
}
