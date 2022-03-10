using FluentValidation;
using IdentityDomain.Features.ResendEmailVerification.CQRS.Command;

namespace IdentityDomain.Features.ResendEmailVerification.Validators.Command;
public class ResendEmailVerificationCommandValidator : AbstractValidator<ResendEmailVerificationCommand>
{
    public ResendEmailVerificationCommandValidator()
    {
        RuleFor(a => a.ResendEmailVerificationRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.ResendEmailVerificationRequest.IdentityUserId).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
    }
}