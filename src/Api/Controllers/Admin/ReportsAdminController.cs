using HouseHelp.Application.Admin;
using HouseHelp.Contracts.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseHelp.Api.Controllers.Admin;

[Authorize(Policy = Policies.RequireAdmin)]
[Route("admin/reports")]
public class ReportsAdminController : ApiControllerBase
{
    private readonly AdminService _adminService;

    public ReportsAdminController(AdminService adminService)
    {
        _adminService = adminService;
    }

    [HttpGet]
    public async Task<ActionResult<AdminReportResponseDto>> Get([FromQuery] string metric, [FromQuery] string period, CancellationToken cancellationToken)
    {
        var report = await _adminService.GetReportAsync(metric, period, cancellationToken);
        return Ok(report);
    }
}
