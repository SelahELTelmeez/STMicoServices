using FluentValidation;
using IdentityDomain.Features.ResetPassword.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.ResetPassword.Validators.Command;
public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.ResetPasswordRequest.NewPassword).Cascade(CascadeMode.Stop).NotNull().WithMessage(resourceJsonManager["XV0013"]);
        RuleFor(a => a.ResetPasswordRequest.IdentityUserId).Cascade(CascadeMode.Stop).NotNull().WithMessage(resourceJsonManager["XV0013"]);

    }
}