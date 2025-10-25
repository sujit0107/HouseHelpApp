using HouseHelp.Application.Auth;
using HouseHelp.Contracts.Auth;
using Microsoft.AspNetCore.Mvc;

namespace HouseHelp.Api.Controllers;

[Route("auth")]
public class AuthController : ApiControllerBase
{
    private readonly AuthService _authService;

    public AuthController(AuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("otp/request")]
    public async Task<ActionResult<OtpRequestResponseDto>> RequestOtp([FromBody] OtpRequestDto request, CancellationToken cancellationToken)
    {
        var requestId = await _authService.RequestOtpAsync(request.Phone, cancellationToken);
        return Ok(new OtpRequestResponseDto(requestId));
    }

    [HttpPost("otp/verify")]
    public async Task<ActionResult<TokenResponseDto>> VerifyOtp([FromBody] OtpVerifyDto request, CancellationToken cancellationToken)
    {
        var response = await _authService.VerifyOtpAsync(request.RequestId, request.Code, request.Phone, request.Role, cancellationToken);
        return Ok(response);
    }

    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponseDto>> Refresh([FromBody] RefreshTokenRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _authService.RefreshTokenAsync(request.RefreshToken, cancellationToken);
        return Ok(response);
    }
}
