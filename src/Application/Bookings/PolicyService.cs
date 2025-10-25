using HouseHelp.Application.Common;
using HouseHelp.Domain.Enums;
using HouseHelp.Domain.Services;
using Microsoft.Extensions.Options;

namespace HouseHelp.Application.Bookings;

public class PolicyService : IPolicyService
{
    private readonly BookingOptions _options;

    public PolicyService(IOptions<BookingOptions> options)
    {
        _options = options.Value;
    }

    public decimal CalculateRefundPercentage(BookingState state, DateTimeOffset now, DateTimeOffset startAt)
    {
        if (state is BookingState.Requested)
        {
            return 100m;
        }

        if (state is BookingState.Accepted && now < startAt)
        {
            return _options.PartialRefundPercent;
        }

        return 0m;
    }
}
