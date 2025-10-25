namespace HouseHelp.Contracts.Residents;

public record HelperSearchResponseDto(IReadOnlyList<HelperSummaryDto> Helpers);

public record HelperSummaryDto(Guid HelperId, Guid UserId, string? Name, string[] Skills, decimal BaseRatePerHour, double RatingAvg, int JobsDone);
