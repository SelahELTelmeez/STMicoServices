using FluentValidation;
using IdentityDomain.Features.EmailVerification.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.EmailVerification.Validators.Command;
public class EmailVerificationCommandValidator : AbstractValidator<EmailVerificationCommand>
{
    public EmailVerificationCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.EmailVerificationRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage(resourceJsonManager["XV0010"]);
        RuleFor(a => a.EmailVerificationRequest.Email).Cascade(CascadeMode.Stop).NotNull().WithMessage(resourceJsonManager["XV0011"]);
        RuleFor(a => a.EmailVerificationRequest.Code).Cascade(CascadeMode.Stop).NotNull().WithMessage(resourceJsonManager["XV0012"]);
    }
}