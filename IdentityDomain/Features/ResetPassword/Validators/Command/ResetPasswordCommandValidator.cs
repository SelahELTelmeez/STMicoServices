using FluentValidation;
using IdentityDomain.Features.ResetPassword.CQRS.Command;

namespace IdentityDomain.Features.ResetPassword.Validators.Command;
public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(a => a.ResetPasswordRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.ResetPasswordRequest.IdentityUserId).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
    }
}