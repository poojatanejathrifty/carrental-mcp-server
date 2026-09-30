using System.ComponentModel;
using CarRental.Application.DTOs;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.Interfaces.Services;
using ModelContextProtocol.Server;

namespace CarRental.MCP.Tools;

[McpServerToolType]
public static class AvailabilityTools
{
    [McpServerTool(Name = "search_available_vehicles"), Description("Searches for vehicles available at a location, optionally within a pickup/return date range. Location accepts an exact code (e.g. 'DA3') or free text (e.g. 'Dubai Airport Terminal 3', 'Dubai Airport') - no need to look up the code first. Pickup/return dates and vehicle category are all optional - omit them to just see what's currently at the location.")]
    public static Task<IReadOnlyList<VehicleDto>> SearchAvailableVehicles(IAvailabilityService availabilityService, AvailabilitySearchRequest request, CancellationToken cancellationToken)
        => availabilityService.SearchAvailableVehiclesAsync(request, cancellationToken);

    [McpServerTool(Name = "is_vehicle_available"), Description("Checks whether a specific vehicle is available for a date range.")]
    public static Task<bool> IsVehicleAvailable(IAvailabilityService availabilityService, int vehicleId, DateTime pickupDateUtc, DateTime returnDateUtc, CancellationToken cancellationToken)
        => availabilityService.IsVehicleAvailableAsync(vehicleId, pickupDateUtc, returnDateUtc, cancellationToken);
}
