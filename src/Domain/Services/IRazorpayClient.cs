using HouseHelp.Domain.Services.Models;

namespace HouseHelp.Domain.Services;

public interface IRazorpayClient
{
    Task<PaymentIntentResult> CreateIntentAsync(PaymentIntentRequest request, CancellationToken cancellationToken = default);
    Task<PaymentCaptureResult> CaptureAsync(string paymentId, long amount, CancellationToken cancellationToken = default);
    Task<PaymentRefundResult> RefundAsync(string paymentId, long amount, string reason, CancellationToken cancellationToken = default);
}
