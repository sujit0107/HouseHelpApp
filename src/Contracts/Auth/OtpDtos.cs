namespace HouseHelp.Contracts.Auth;

public record OtpRequestDto(string Phone);

public record OtpRequestResponseDto(string RequestId);

public record OtpVerifyDto(string RequestId, string Code, string Phone, string? Role);

public record TokenResponseDto(string AccessToken, string RefreshToken, UserSummaryDto User);
