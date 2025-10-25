using HouseHelp.Application.Payments;
using HouseHelp.Contracts.Payments;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseHelp.Api.Controllers.Payments;

[Authorize]
[Route("payments")]
public class PaymentsController : ApiControllerBase
{
    private readonly PaymentService _paymentService;

    public PaymentsController(PaymentService paymentService)
    {
        _paymentService = paymentService;
    }

    [HttpPost("create-intent")]
    public async Task<ActionResult<PaymentResponseDto>> CreateIntent([FromBody] CreatePaymentIntentRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _paymentService.CreateIntentAsync(request, cancellationToken);
        return Ok(response);
    }

    [HttpPost("capture")]
    public async Task<ActionResult<PaymentResponseDto>> Capture([FromBody] PaymentCaptureRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _paymentService.CaptureAsync(request, cancellationToken);
        return Ok(response);
    }

    [Authorize(Policy = Policies.RequireAdmin)]
    [HttpPost("refund")]
    public async Task<ActionResult<PaymentResponseDto>> Refund([FromBody] PaymentRefundRequestDto request, CancellationToken cancellationToken)
    {
        var response = await _paymentService.RefundAsync(request, cancellationToken);
        return Ok(response);
    }
}
