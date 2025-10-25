using FluentValidation;
using HouseHelp.Contracts.Residents;

namespace HouseHelp.Application.Validators;

public class CreateBookingRequestValidator : AbstractValidator<CreateBookingRequestDto>
{
    public CreateBookingRequestValidator()
    {
        RuleFor(x => x.HelperId).NotEmpty();
        RuleFor(x => x.FlatId).NotEmpty();
        RuleFor(x => x.ServiceType).NotEmpty();
        RuleFor(x => x.EndAt).GreaterThan(x => x.StartAt);
        RuleFor(x => x.PaymentMethod).NotEmpty();
    }
}
