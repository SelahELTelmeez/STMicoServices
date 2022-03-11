using FluentValidation;
using IdentityDomain.Features.EmailVerification.CQRS.Command;

namespace IdentityDomain.Features.EmailVerification.Validators.Command;
public class EmailVerificationCommandValidator : AbstractValidator<EmailVerificationCommand>
{
    public EmailVerificationCommandValidator()
    {
        RuleFor(a => a.MobileVerificationRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.MobileVerificationRequest.Email).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.MobileVerificationRequest.Code).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
    }
}