using FluentValidation;
using IdentityDomain.Features.ChangePassword.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.ChangePassword.Validator.Command;
public class ChangePasswordCommandValidator : AbstractValidator<ChangePasswordCommand>
{
    public ChangePasswordCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.ChangePasswordRequest.OldPassword)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .WithMessage(resourceJsonManager["XV0008"]);

        RuleFor(a => a.ChangePasswordRequest.NewPassword)
         .Cascade(CascadeMode.Stop)
         .NotEmpty()
         .WithMessage(resourceJsonManager["XV0009"]);
    }
}