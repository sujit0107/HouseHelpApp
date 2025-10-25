using HouseHelp.Domain.Enums;

namespace HouseHelp.Domain.Services;

public interface IPolicyService
{
    decimal CalculateRefundPercentage(BookingState state, DateTimeOffset now, DateTimeOffset startAt);
}
