using HouseHelp.Domain.Entities;

namespace HouseHelp.Domain.Repositories;

public interface IAvailabilityRepository
{
    Task<IReadOnlyList<Availability>> GetForHelperAsync(Guid helperId, CancellationToken cancellationToken = default);
    Task UpsertRangeAsync(IEnumerable<Availability> availabilities, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<HelperProfile>> SearchHelpersAsync(HelperSearchCriteria criteria, CancellationToken cancellationToken = default);
}

public record HelperSearchCriteria(
    string? Skill,
    DateTimeOffset? StartAt,
    DateTimeOffset? EndAt,
    decimal? PriceMin,
    decimal? PriceMax,
    string? Sort);
