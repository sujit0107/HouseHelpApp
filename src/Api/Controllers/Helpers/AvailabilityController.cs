using HouseHelp.Api.Extensions;
using HouseHelp.Application.Helpers;
using HouseHelp.Contracts.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseHelp.Api.Controllers.Helpers;

[Authorize(Policy = Policies.RequireHelper)]
[Route("me/availability")]
public class AvailabilityController : ApiControllerBase
{
    private readonly AvailabilityService _availabilityService;

    public AvailabilityController(AvailabilityService availabilityService)
    {
        _availabilityService = availabilityService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<AvailabilityDto>>> Get(CancellationToken cancellationToken)
    {
        var helperId = User.GetUserId();
        var slots = await _availabilityService.GetAvailabilityAsync(helperId, cancellationToken);
        return Ok(slots);
    }

    [HttpPut]
    public async Task<IActionResult> Put([FromBody] UpdateAvailabilityRequestDto request, CancellationToken cancellationToken)
    {
        await _availabilityService.UpdateAvailabilityAsync(User.GetUserId(), request, cancellationToken);
        return NoContent();
    }
}
