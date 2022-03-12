using FluentValidation;
using IdentityDomain.Features.ResetPassword.CQRS.Command;
using JsonLocalizer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityDomain.Features.ResetPassword.Validators.Command;
public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.ResetPasswordRequest.NewPassword).Cascade(CascadeMode.Stop).NotNull().WithMessage(resourceJsonManager["XV0013"]);
        RuleFor(a => a.ResetPasswordRequest.IdentityUserId).Cascade(CascadeMode.Stop).NotNull().WithMessage(resourceJsonManager["XV0013"]);

    }
}