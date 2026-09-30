using System.ComponentModel;
using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Services;
using ModelContextProtocol.Server;

namespace CarRental.MCP.Tools;

[McpServerToolType]
public static class VehicleTools
{
    [McpServerTool(Name = "get_vehicle"), Description("Gets a single vehicle by its id.")]
    public static Task<VehicleDto> GetVehicle(IVehicleService vehicleService, int vehicleId, CancellationToken cancellationToken)
        => vehicleService.GetByIdAsync(vehicleId, cancellationToken);

    [McpServerTool(Name = "list_vehicles"), Description("Lists all vehicles in the fleet.")]
    public static Task<IReadOnlyList<VehicleDto>> ListVehicles(IVehicleService vehicleService, CancellationToken cancellationToken)
        => vehicleService.GetAllAsync(cancellationToken);

    [McpServerTool(Name = "list_vehicles_by_location"), Description("Lists vehicles currently parked at a given location.")]
    public static Task<IReadOnlyList<VehicleDto>> ListVehiclesByLocation(IVehicleService vehicleService, int locationId, CancellationToken cancellationToken)
        => vehicleService.GetByLocationAsync(locationId, cancellationToken);

    [McpServerTool(Name = "register_vehicle"), Description("Registers a new vehicle into the fleet.")]
    public static Task<VehicleDto> RegisterVehicle(IVehicleService vehicleService, CreateVehicleRequest request, CancellationToken cancellationToken)
        => vehicleService.CreateAsync(request, cancellationToken);
}
