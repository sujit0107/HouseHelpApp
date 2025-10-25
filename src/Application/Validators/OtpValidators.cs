using FluentValidation;
using HouseHelp.Contracts.Auth;

namespace HouseHelp.Application.Validators;

public class OtpRequestValidator : AbstractValidator<OtpRequestDto>
{
    public OtpRequestValidator()
    {
        RuleFor(x => x.Phone).NotEmpty().Length(10, 15);
    }
}

public class OtpVerifyValidator : AbstractValidator<OtpVerifyDto>
{
    public OtpVerifyValidator()
    {
        RuleFor(x => x.RequestId).NotEmpty();
        RuleFor(x => x.Code).NotEmpty();
        RuleFor(x => x.Phone).NotEmpty().Length(10, 15);
    }
}
