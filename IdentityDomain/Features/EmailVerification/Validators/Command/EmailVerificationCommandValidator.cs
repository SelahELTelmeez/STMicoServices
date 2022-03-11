using FluentValidation;
using IdentityDomain.Features.EmailVerification.CQRS.Command;

namespace IdentityDomain.Features.EmailVerification.Validators.Command;
public class EmailVerificationCommandValidator : AbstractValidator<EmailVerificationCommand>
{
    public EmailVerificationCommandValidator()
    {
        RuleFor(a => a.EmailVerificationRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.EmailVerificationRequest.Email).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.EmailVerificationRequest.Code).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
    }
}