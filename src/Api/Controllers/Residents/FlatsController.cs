using HouseHelp.Api.Extensions;
using HouseHelp.Contracts.Residents;
using HouseHelp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseHelp.Api.Controllers.Residents;

[Authorize(Policy = Policies.RequireResident)]
[Route("residents/flats")]
public class FlatsController : ApiControllerBase
{
    private readonly IFlatRepository _flatRepository;

    public FlatsController(IFlatRepository flatRepository)
    {
        _flatRepository = flatRepository;
    }

    [HttpGet]
    public async Task<ActionResult<ResidentFlatsResponseDto>> Get(CancellationToken cancellationToken)
    {
        var residentId = User.GetUserId();
        var flats = await _flatRepository.GetByResidentIdAsync(residentId, cancellationToken);
        var dtos = flats
            .Select(f => new ResidentFlatDto(f.Id, f.BuildingId, f.Number, f.Building?.Name ?? string.Empty))
            .ToList();
        return Ok(new ResidentFlatsResponseDto(dtos));
    }
}
