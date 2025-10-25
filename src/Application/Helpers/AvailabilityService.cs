using HouseHelp.Contracts.Helpers;
using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HouseHelp.Application.Helpers;

public class AvailabilityService
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly ILogger<AvailabilityService> _logger;

    public AvailabilityService(IAvailabilityRepository availabilityRepository, ILogger<AvailabilityService> logger)
    {
        _availabilityRepository = availabilityRepository;
        _logger = logger;
    }

    public async Task<IReadOnlyList<AvailabilityDto>> GetAvailabilityAsync(Guid helperId, CancellationToken cancellationToken)
    {
        var slots = await _availabilityRepository.GetForHelperAsync(helperId, cancellationToken);
        return slots.Select(Map).ToList();
    }

    public async Task UpdateAvailabilityAsync(Guid helperId, UpdateAvailabilityRequestDto request, CancellationToken cancellationToken)
    {
        var normalized = request.Slots
            .Select(s => new Availability
            {
                Id = s.Id == Guid.Empty ? Guid.NewGuid() : s.Id,
                HelperId = helperId,
                Date = s.Date,
                Start = s.Start,
                End = s.End,
                IsRecurring = s.IsRecurring,
                RecurrenceRule = s.RecurrenceRule,
                IsActive = s.IsActive
            })
            .OrderBy(s => s.Date)
            .ThenBy(s => s.Start)
            .ToList();

        // TODO: merge overlapping slots
        await _availabilityRepository.UpsertRangeAsync(normalized, cancellationToken);
    }

    private static AvailabilityDto Map(Availability availability)
        => new(availability.Id, availability.Date, availability.Start, availability.End, availability.IsRecurring, availability.RecurrenceRule, availability.IsActive);
}
