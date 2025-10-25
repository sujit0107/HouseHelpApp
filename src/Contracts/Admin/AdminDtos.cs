using HouseHelp.Domain.Enums;

namespace HouseHelp.Contracts.Admin;

public record AdminHelperSummaryDto(Guid HelperId, string? Name, string Phone, KycStatus KycStatus, double RatingAvg, int JobsDone);

public record AdminHelpersResponseDto(IReadOnlyList<AdminHelperSummaryDto> Helpers);

public record AdminDisputeDto(Guid Id, Guid BookingId, Guid OpenedBy, string Reason, string Status, DateTimeOffset CreatedAt, string? Resolution, DateTimeOffset? ResolvedAt);

public record AdminDisputesResponseDto(IReadOnlyList<AdminDisputeDto> Disputes);

public record ResolveDisputeRequestDto(string Resolution);

public record AdminReportResponseDto(string Metric, string Period, decimal Value);
