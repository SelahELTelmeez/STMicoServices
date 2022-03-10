using FluentValidation;
using IdentityDomain.Features.ForgetPassword.CQRS.Command;

namespace IdentityDomain.Features.ForgetPassword.Validators.Command;
public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
{
    public ForgetPasswordCommandValidator()
    {
        RuleFor(a => a.ForgetPasswordRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
    }
}