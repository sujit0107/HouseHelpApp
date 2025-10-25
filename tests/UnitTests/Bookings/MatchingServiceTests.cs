using HouseHelp.Application.Bookings;
using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Repositories;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HouseHelp.UnitTests.Bookings;

public class MatchingServiceTests
{
    [Fact]
    public async Task ReturnsHelpersFromRepository()
    {
        var helper = new HelperProfile { Id = Guid.NewGuid(), UserId = Guid.NewGuid(), Skills = new[] { "Cleaning" }, BaseRatePerHour = 200, RatingAvg = 4.5, JobsDone = 10, User = new User { Name = "A" } };
        var repo = new Mock<IAvailabilityRepository>();
        repo.Setup(x => x.SearchHelpersAsync(It.IsAny<HelperSearchCriteria>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<HelperProfile> { helper });
        var service = new MatchingService(repo.Object, NullLogger<MatchingService>.Instance);

        var result = await service.SearchHelpersAsync(new HelperSearchCriteria("Cleaning", null, null, null, null, null), CancellationToken.None);

        Assert.Single(result.Helpers);
        Assert.Equal(helper.UserId, result.Helpers[0].HelperId);
    }
}
