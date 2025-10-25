namespace HouseHelp.Contracts.Helpers;

public record AvailabilityDto(Guid Id, DateOnly Date, TimeOnly Start, TimeOnly End, bool IsRecurring, string? RecurrenceRule, bool IsActive);

public record UpdateAvailabilityRequestDto(IReadOnlyList<AvailabilityDto> Slots);
