using FluentValidation;
using IdentityDomain.Features.ConfirmOTPCode.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.ConfirmOTPCode.Validators.Command;
public class ConfirmOTPCodeCommandValidator : AbstractValidator<ConfirmOTPCodeCommand>
{
    public ConfirmOTPCodeCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.OTPCode)
        .Cascade(CascadeMode.Stop)
        .NotEmpty()
        .WithMessage(resourceJsonManager["XV0052"]);
    }
}