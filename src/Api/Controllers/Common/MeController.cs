using HouseHelp.Api.Extensions;
using HouseHelp.Contracts.Auth;
using HouseHelp.Contracts.Common;
using HouseHelp.Domain.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseHelp.Api.Controllers.Common;

[Authorize]
[Route("me")]
public class MeController : ApiControllerBase
{
    private readonly IUserRepository _userRepository;

    public MeController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<MeResponseDto>> Get(CancellationToken cancellationToken)
    {
        var userId = User.GetUserId();
        var user = await _userRepository.GetByIdAsync(userId, cancellationToken);
        if (user is null)
        {
            return NotFound();
        }

        var summary = new UserSummaryDto(user.Id, user.Phone, user.Email, user.Name, user.Role);
        return Ok(new MeResponseDto(summary, User.GetPermissions()));
    }
}
