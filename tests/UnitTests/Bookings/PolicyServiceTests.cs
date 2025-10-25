using HouseHelp.Application.Bookings;
using HouseHelp.Application.Common;
using HouseHelp.Domain.Enums;
using Microsoft.Extensions.Options;
using Xunit;

namespace HouseHelp.UnitTests.Bookings;

public class PolicyServiceTests
{
    private readonly PolicyService _service = new(Options.Create(new BookingOptions { PartialRefundPercent = 50 }));

    [Theory]
    [InlineData(BookingState.Requested, 100)]
    [InlineData(BookingState.Accepted, 50)]
    [InlineData(BookingState.InProgress, 0)]
    public void CalculatesRefundsCorrectly(BookingState state, decimal expected)
    {
        var result = _service.CalculateRefundPercentage(state, DateTimeOffset.UtcNow.AddHours(-1), DateTimeOffset.UtcNow.AddHours(1));
        Assert.Equal(expected, result);
    }
}
