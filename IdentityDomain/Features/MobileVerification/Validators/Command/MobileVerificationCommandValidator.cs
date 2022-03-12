using FluentValidation;
using IdentityDomain.Features.MobileVerification.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.MobileVerification.Validators.Command;
public class MobileVerificationCommandValidator : AbstractValidator<MobileVerificationCommand>
{
    public MobileVerificationCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.MobileVerificationRequest.MobileNumber)
                             .Cascade(CascadeMode.Stop)
                             .NotEmpty()
                             .WithMessage(resourceJsonManager["XV0030"])
                             .MinimumLength(11)
                             .WithMessage(resourceJsonManager["XV0031"])
                             .Matches(@"^[0-9]*$")
                             .WithMessage(resourceJsonManager["XV0032"])
                             .Matches(@"[0+][1+]\d")
                             .WithMessage(resourceJsonManager["XV0033"]);

        RuleFor(a => a.MobileVerificationRequest.Code)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(resourceJsonManager["XV0034"]);
    }
}