using HouseHelp.Contracts.Residents;
using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Repositories;
using Microsoft.Extensions.Logging;

namespace HouseHelp.Application.Bookings;

public class MatchingService
{
    private readonly IAvailabilityRepository _availabilityRepository;
    private readonly ILogger<MatchingService> _logger;

    public MatchingService(IAvailabilityRepository availabilityRepository, ILogger<MatchingService> logger)
    {
        _availabilityRepository = availabilityRepository;
        _logger = logger;
    }

    public async Task<HelperSearchResponseDto> SearchHelpersAsync(HelperSearchCriteria criteria, CancellationToken cancellationToken)
    {
        var helpers = await _availabilityRepository.SearchHelpersAsync(criteria, cancellationToken);
        var summaries = helpers
            .Select(h => new HelperSummaryDto(h.UserId, h.UserId, h.User?.Name, h.Skills, h.BaseRatePerHour, h.RatingAvg, h.JobsDone))
            .OrderByDescending(h => h.RatingAvg)
            .ToList();

        return new HelperSearchResponseDto(summaries);
    }

    public double Score(HelperProfile helper, decimal targetPrice)
    {
        var ratingScore = helper.RatingAvg * 10;
        var priceScore = (double)Math.Max(0, 100 - Math.Abs((helper.BaseRatePerHour - targetPrice) / (targetPrice == 0 ? 1 : targetPrice) * 100));
        var activityScore = helper.JobsDone / 10.0;
        var finalScore = ratingScore + priceScore + activityScore;
        _logger.LogDebug("Score for {HelperId} = {Score}", helper.Id, finalScore);
        return finalScore;
    }
}
