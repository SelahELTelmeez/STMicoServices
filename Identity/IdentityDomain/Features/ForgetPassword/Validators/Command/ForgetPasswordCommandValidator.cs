using FluentValidation;
using IdentityDomain.Features.ForgetPassword.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.ForgetPassword.Validators.Command;
public class ForgetPasswordCommandValidator : AbstractValidator<ForgetPasswordCommand>
{
    public ForgetPasswordCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        When(a => !a.ForgetPasswordRequest.Email.Equals(string.Empty), () =>
        {
            RuleFor(a => a.ForgetPasswordRequest.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(resourceJsonManager["XV0017"])
            .EmailAddress()
            .WithMessage(resourceJsonManager["XV0018"]);

        }).Otherwise(() =>
        {
            RuleFor(a => a.ForgetPasswordRequest.MobileNumber)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .WithMessage("Empty")
           .MinimumLength(11)
           .WithMessage(resourceJsonManager["XV0019"])
           .Matches(@"^[0-9]*$")
           .WithMessage(resourceJsonManager["XV0020"])
           .Matches(@"[0+][1+]\d")
           .WithMessage(resourceJsonManager["XV0021"]);
        });
    }
}