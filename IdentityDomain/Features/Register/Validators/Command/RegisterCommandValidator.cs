using FluentValidation;
using IdentityDomain.Features.Register.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.Register.Validators.Command;
public class RegisterCommandValidator : AbstractValidator<RegisterCommand>
{
    public RegisterCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.RegisterRequest.PasswordHash)
           .Cascade(CascadeMode.Stop)
           .NotEmpty()
           .WithMessage(resourceJsonManager["XV0035"])
           .MinimumLength(7)
           .WithMessage(resourceJsonManager["XV0036"]);

        RuleFor(a => a.RegisterRequest.FullName)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(resourceJsonManager["XV0037"]);


        When(a => !a.RegisterRequest.Email.Equals(string.Empty), () =>
        {
            RuleFor(a => a.RegisterRequest.Email)
            .Cascade(CascadeMode.Stop)
            .NotEmpty()
            .WithMessage(resourceJsonManager["XV0039"])
            .EmailAddress()
            .WithMessage(resourceJsonManager["XV0040"]);

        }).Otherwise(() =>
        {
            RuleFor(a => a.RegisterRequest.MobileNumber)
             .Cascade(CascadeMode.Stop)
             .NotEmpty()
             .WithMessage("Empty")
             .MinimumLength(11)
             .WithMessage(resourceJsonManager["XV0041"])
             .Matches(@"^[0-9]*$")
             .WithMessage(resourceJsonManager["XV0042"])
             .Matches(@"[0+][1+]\d")
             .WithMessage(resourceJsonManager["XV0043"]);
        });
    }
}
