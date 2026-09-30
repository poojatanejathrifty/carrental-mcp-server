using System.ComponentModel;
using CarRental.Application.DTOs;
using CarRental.Application.Interfaces.Services;
using ModelContextProtocol.Server;

namespace CarRental.MCP.Tools;

[McpServerToolType]
public static class LocationTools
{
    [McpServerTool(Name = "list_locations"), Description("Lists every rentable location (code and description) where a car can be picked up or dropped off. search_available_vehicles already accepts a location name directly, so this is only needed to browse all locations or to disambiguate when a search comes back empty.")]
    public static Task<IReadOnlyList<LocationDto>> ListLocations(ILocationService locationService, CancellationToken cancellationToken)
        => locationService.GetAllAsync(cancellationToken);
}
