using HouseHelp.Application.Admin;
using HouseHelp.Contracts.Admin;
using HouseHelp.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseHelp.Api.Controllers.Admin;

[Authorize(Policy = Policies.RequireAdmin)]
[Route("admin/helpers")]
public class HelpersAdminController : ApiControllerBase
{
    private readonly AdminService _adminService;

    public HelpersAdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<ActionResult<AdminHelpersResponseDto>> Get([FromQuery] KycStatus? kycStatus, CancellationToken cancellationToken)
    {
        var response = await _adminService.GetHelpersAsync(kycStatus, cancellationToken);
        return Ok(response);
    }

    [HttpPost("{id:guid}/approve")]
    public async Task<IActionResult> Approve(Guid id, CancellationToken cancellationToken)
    {
        await _adminService.UpdateHelperKycAsync(id, KycStatus.Approved, cancellationToken);
        return NoContent();
    }

    [HttpPost("{id:guid}/reject")]
    public async Task<IActionResult> Reject(Guid id, CancellationToken cancellationToken)
    {
        await _adminService.UpdateHelperKycAsync(id, KycStatus.Rejected, cancellationToken);
        return NoContent();
    }
}
