using System.Security.Claims;
using System.Text;
using HouseHelp.Contracts.Auth;
using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Enums;
using HouseHelp.Domain.Repositories;
using HouseHelp.Domain.Services;
using Microsoft.Extensions.Logging;

namespace HouseHelp.Application.Auth;

public class AuthService
{
    private readonly IUserRepository _users;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IOtpProvider _otpProvider;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository users,
        IUnitOfWork unitOfWork,
        IOtpProvider otpProvider,
        IJwtTokenService jwtTokenService,
        ILogger<AuthService> logger)
    {
        _users = users;
        _unitOfWork = unitOfWork;
        _otpProvider = otpProvider;
        _jwtTokenService = jwtTokenService;
        _logger = logger;
    }

    public Task<string> RequestOtpAsync(string phone, CancellationToken cancellationToken)
    {
        return _otpProvider.RequestAsync(phone, cancellationToken);
    }

    public async Task<TokenResponseDto> VerifyOtpAsync(string requestId, string code, string phone, string? role, CancellationToken cancellationToken)
    {
        var verified = await _otpProvider.VerifyAsync(requestId, code, cancellationToken);
        if (!verified)
        {
            throw new InvalidOperationException("Invalid OTP");
        }

        var user = await _users.GetByPhoneAsync(phone, cancellationToken);
        if (user is null)
        {
            user = new User
            {
                Id = Guid.NewGuid(),
                Phone = phone,
                Role = ResolveRole(role),
                CreatedAt = DateTimeOffset.UtcNow,
                IsActive = true
            };
            await _users.AddAsync(user, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else if (!user.IsActive)
        {
            throw new InvalidOperationException("User is inactive");
        }

        _logger.LogInformation("OTP verified for {Phone}", phone);
        return _jwtTokenService.GenerateTokens(user);
    }

    public async Task<TokenResponseDto> RefreshTokenAsync(string refreshToken, CancellationToken cancellationToken)
    {
        var payload = Encoding.UTF8.GetString(Convert.FromBase64String(refreshToken));
        var parts = payload.Split(':');
        if (parts.Length == 0 || !Guid.TryParse(parts[0], out var userId))
        {
            throw new InvalidOperationException("Invalid refresh token");
        }

        var user = await _users.GetByIdAsync(userId, cancellationToken) ?? throw new InvalidOperationException("Invalid refresh token");
        return _jwtTokenService.RefreshTokens(user);
    }

    private static UserRole ResolveRole(string? role)
    {
        if (string.IsNullOrWhiteSpace(role))
        {
            return UserRole.Resident;
        }

        return Enum.TryParse<UserRole>(role, true, out var parsed) ? parsed : UserRole.Resident;
    }

    public static IEnumerable<Claim> BuildClaims(User user)
    {
        yield return new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());
        yield return new Claim(ClaimTypes.MobilePhone, user.Phone);
        yield return new Claim(ClaimTypes.Role, user.Role.ToString());
        if (!string.IsNullOrWhiteSpace(user.Name))
        {
            yield return new Claim(ClaimTypes.Name, user.Name);
        }
        if (!string.IsNullOrWhiteSpace(user.Email))
        {
            yield return new Claim(ClaimTypes.Email, user.Email);
        }
    }
}
