using FluentValidation;
using HouseHelp.Contracts.Payments;

namespace HouseHelp.Application.Validators;

public class CreatePaymentIntentRequestValidator : AbstractValidator<CreatePaymentIntentRequestDto>
{
    public CreatePaymentIntentRequestValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
    }
}

public class PaymentCaptureRequestValidator : AbstractValidator<PaymentCaptureRequestDto>
{
    public PaymentCaptureRequestValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
    }
}

public class PaymentRefundRequestValidator : AbstractValidator<PaymentRefundRequestDto>
{
    public PaymentRefundRequestValidator()
    {
        RuleFor(x => x.BookingId).NotEmpty();
        RuleFor(x => x.Reason).NotEmpty();
    }
}
