using FluentValidation;
using HouseHelp.Contracts.Helpers;

namespace HouseHelp.Application.Validators;

public class UpdateAvailabilityRequestValidator : AbstractValidator<UpdateAvailabilityRequestDto>
{
    public UpdateAvailabilityRequestValidator()
    {
        RuleForEach(x => x.Slots).SetValidator(new AvailabilityDtoValidator());
    }

    private sealed class AvailabilityDtoValidator : AbstractValidator<AvailabilityDto>
    {
        public AvailabilityDtoValidator()
        {
            RuleFor(x => x.Date).NotEmpty();
            RuleFor(x => x.Start).NotEmpty();
            RuleFor(x => x.End).GreaterThan(x => x.Start);
        }
    }
}
