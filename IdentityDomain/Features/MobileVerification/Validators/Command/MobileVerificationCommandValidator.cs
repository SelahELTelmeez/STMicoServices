using FluentValidation;
using IdentityDomain.Features.MobileVerification.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.MobileVerification.Validators.Command;
public class MobileVerificationCommandValidator : AbstractValidator<MobileVerificationCommand>
{
    public MobileVerificationCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.MobileVerificationRequest.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(resourceJsonManager["XV0034"]);
    }
}