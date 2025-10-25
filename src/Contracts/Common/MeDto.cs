using HouseHelp.Contracts.Auth;

namespace HouseHelp.Contracts.Common;

public record MeResponseDto(UserSummaryDto User, string[] Permissions);
