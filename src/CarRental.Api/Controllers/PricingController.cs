using CarRental.Application.DTOs.Requests;
using CarRental.Application.DTOs.Responses;
using CarRental.Application.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace CarRental.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Produces("application/json")]
public class PricingController(IPricingService pricingService) : ControllerBase
{
    [HttpPost("quote")]
    [ProducesResponseType<PriceQuoteResponse>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PriceQuoteResponse>> GetQuote(PriceQuoteRequest request, CancellationToken cancellationToken)
        => Ok(await pricingService.GetQuoteAsync(request, cancellationToken));
}
