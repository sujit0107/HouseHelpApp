namespace HouseHelp.Application.Common;

public class BookingOptions
{
    public int RequestExpirySeconds { get; set; } = 120;
    public int ArrivalGraceMinutes { get; set; } = 10;
    public int PartialRefundPercent { get; set; } = 50;
}
