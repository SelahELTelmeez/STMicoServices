using FluentValidation;
using IdentityDomain.Features.MobileVerification.CQRS.Command;

namespace IdentityDomain.Features.MobileVerification.Validators.Command;
public class MobileVerificationCommandValidator : AbstractValidator<MobileVerificationCommand>
{
    public MobileVerificationCommandValidator()
    {
        RuleFor(a => a.MobileVerificationRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.MobileVerificationRequest.MobileNumber).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.MobileVerificationRequest.Code).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
    }
}