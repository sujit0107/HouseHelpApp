using HouseHelp.Domain.Services;
using HouseHelp.Domain.Services.Models;
using Microsoft.Extensions.Logging;

namespace HouseHelp.Infrastructure.Payments;

public class FakeRazorpayClient : IRazorpayClient
{
    private readonly ILogger<FakeRazorpayClient> _logger;

    public FakeRazorpayClient(ILogger<FakeRazorpayClient> logger)
    {
        _logger = logger;
    }

    public Task<PaymentIntentResult> CreateIntentAsync(PaymentIntentRequest request, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Creating fake Razorpay intent for {Amount}", request.AmountMinor);
        return Task.FromResult(new PaymentIntentResult(Guid.NewGuid().ToString(), "fake-secret", "created"));
    }

    public Task<PaymentCaptureResult> CaptureAsync(string paymentId, long amount, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Capturing fake Razorpay payment {PaymentId}", paymentId);
        return Task.FromResult(new PaymentCaptureResult(Guid.NewGuid().ToString(), "captured"));
    }

    public Task<PaymentRefundResult> RefundAsync(string paymentId, long amount, string reason, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Refunding fake Razorpay payment {PaymentId}", paymentId);
        return Task.FromResult(new PaymentRefundResult(Guid.NewGuid().ToString(), "refunded"));
    }
}
