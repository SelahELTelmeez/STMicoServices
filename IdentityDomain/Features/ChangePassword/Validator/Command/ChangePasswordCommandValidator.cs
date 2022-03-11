using FluentValidation;
using IdentityDomain.Features.ChangePassword.CQRS.Command;

namespace IdentityDomain.Features.ChangePassword.Validator.Command;
public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator()
    {
        RuleFor(a => a.IdentityChangePasswordRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.IdentityChangePasswordRequest.OldPassword).Cascade(CascadeMode.Stop).Empty().WithMessage("");
        RuleFor(a => a.IdentityChangePasswordRequest.NewPassword).Cascade(CascadeMode.Stop).Empty().WithMessage("");
    }
}