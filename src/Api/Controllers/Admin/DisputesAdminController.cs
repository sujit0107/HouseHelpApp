using HouseHelp.Application.Admin;
using HouseHelp.Contracts.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseHelp.Api.Controllers.Admin;

[Authorize(Policy = Policies.RequireAdmin)]
[Route("admin/disputes")]
public class DisputesAdminController : ApiControllerBase
{
    private readonly AdminService _adminService;

    public DisputesAdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<ActionResult<AdminDisputesResponseDto>> Get(CancellationToken cancellationToken)
    {
        var response = await _adminService.GetDisputesAsync(cancellationToken);
        return Ok(response);
    }

    [HttpPost("{id:guid}/resolve")]
    public async Task<IActionResult> Resolve(Guid id, [FromBody] ResolveDisputeRequestDto request, CancellationToken cancellationToken)
    {
        await _adminService.ResolveDisputeAsync(id, request.Resolution, cancellationToken);
        return NoContent();
    }
}
