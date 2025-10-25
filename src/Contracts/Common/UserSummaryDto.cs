using HouseHelp.Domain.Enums;

namespace HouseHelp.Contracts.Auth;

public record UserSummaryDto(Guid Id, string Phone, string? Email, string? Name, UserRole Role);
