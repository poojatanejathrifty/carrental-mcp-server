using System.ComponentModel;
using CarRental.Application.DTOs.Requests;
using CarRental.Application.DTOs.Responses;
using CarRental.Application.Interfaces.Services;
using ModelContextProtocol.Server;

namespace CarRental.MCP.Tools;

[McpServerToolType]
public static class PricingTools
{
    [McpServerTool(Name = "get_price_quote"), Description("Calculates a rental price quote for a vehicle category, location and date range.")]
    public static Task<PriceQuoteResponse> GetPriceQuote(IPricingService pricingService, PriceQuoteRequest request, CancellationToken cancellationToken)
        => pricingService.GetQuoteAsync(request, cancellationToken);
}
