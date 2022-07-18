using FluentValidation;
using IdentityDomain.Features.ConfirmForgetPassword.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.ConfirmForgetPassword.Validators.Command;
public class ConfirmOTPCodeCommandValidator : AbstractValidator<ConfirmForgetPasswordCommand>
{
    public ConfirmOTPCodeCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.OTPVerificationRequest)
        .Cascade(CascadeMode.Stop)
        .NotEmpty()
        .WithMessage(resourceJsonManager["XV0052"]);
    }
}