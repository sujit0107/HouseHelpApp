using HouseHelp.Domain.Enums;

namespace HouseHelp.Contracts.Helpers;

public record HelperJobSummaryDto(Guid BookingId, BookingState State, DateTimeOffset StartAt, DateTimeOffset EndAt, string ServiceType, decimal PriceEstimate);

public record HelperJobsResponseDto(IReadOnlyList<HelperJobSummaryDto> Jobs);
