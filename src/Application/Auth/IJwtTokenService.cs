using HouseHelp.Contracts.Auth;
using HouseHelp.Domain.Entities;

namespace HouseHelp.Application.Auth;

public interface IJwtTokenService
{
    TokenResponseDto GenerateTokens(User user);
    TokenResponseDto RefreshTokens(User user);
}
