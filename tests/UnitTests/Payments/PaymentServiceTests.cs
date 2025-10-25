using HouseHelp.Application.Payments;
using HouseHelp.Contracts.Payments;
using HouseHelp.Domain.Entities;
using HouseHelp.Domain.Enums;
using HouseHelp.Domain.Repositories;
using HouseHelp.Domain.Services;
using HouseHelp.Domain.Services.Models;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HouseHelp.UnitTests.Payments;

public class PaymentServiceTests
{
    [Fact]
    public async Task CreateIntentCreatesPayment()
    {
        var booking = new Booking { Id = Guid.NewGuid(), PriceEstimate = 250, State = BookingState.Requested };
        var bookingRepo = new Mock<IBookingRepository>();
        bookingRepo.Setup(x => x.GetByIdAsync(booking.Id, It.IsAny<CancellationToken>())).ReturnsAsync(booking);
        var paymentRepo = new Mock<IPaymentRepository>();
        var razorpay = new Mock<IRazorpayClient>();
        razorpay.Setup(x => x.CreateIntentAsync(It.IsAny<PaymentIntentRequest>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentIntentResult("intent", "secret", "created"));
        var notifier = new Mock<IRealtimeNotifier>();
        var service = new PaymentService(paymentRepo.Object, bookingRepo.Object, Mock.Of<IUnitOfWork>(), razorpay.Object, notifier.Object, NullLogger<PaymentService>.Instance);

        var response = await service.CreateIntentAsync(new CreatePaymentIntentRequestDto(booking.Id), CancellationToken.None);

        Assert.Equal("intent", response.IntentId);
    }
}
