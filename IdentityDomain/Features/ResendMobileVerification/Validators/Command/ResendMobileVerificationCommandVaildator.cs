using FluentValidation;
using IdentityDomain.Features.ResendMobileVerification.CQRS.Command;

namespace IdentityDomain.Features.ResendMobileVerification.Validators.Command;
public class ResendMobileVerificationCommandVaildator : AbstractValidator<ResendMobileVerificationCommand>
{
    public ResendMobileVerificationCommandVaildator()
    {
        RuleFor(a => a.ResendMobileVerificationRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.ResendMobileVerificationRequest.IdentityUserId).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
    }
}