using HouseHelp.Application.Bookings;
using HouseHelp.Contracts.Residents;
using HouseHelp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseHelp.Api.Controllers.Residents;

[Authorize(Policy = Policies.RequireResident)]
[Route("helpers")]
public class HelpersController : ApiControllerBase
{
    private readonly MatchingService _matchingService;

    public HelpersController(MatchingService matchingService)
    {
        _matchingService = matchingService;
    }

    [HttpGet("search")]
    public async Task<ActionResult<HelperSearchResponseDto>> Search([FromQuery] string? skill, [FromQuery] DateTimeOffset? startAt, [FromQuery] DateTimeOffset? endAt, [FromQuery] decimal? priceMin, [FromQuery] decimal? priceMax, [FromQuery] string? sort, CancellationToken cancellationToken)
    {
        var criteria = new HelperSearchCriteria(skill, startAt, endAt, priceMin, priceMax, sort);
        var result = await _matchingService.SearchHelpersAsync(criteria, cancellationToken);
        return Ok(result);
    }
}
