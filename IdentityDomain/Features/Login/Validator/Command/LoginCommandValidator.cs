using FluentValidation;
using IdentityDomain.Features.Login.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.Login.Validator.Command;
public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        When(a => !string.IsNullOrEmpty(a.LoginRequest.OfficeId) || !string.IsNullOrEmpty(a.LoginRequest.FacebookId) || !string.IsNullOrEmpty(a.LoginRequest.GoogleId), () => {

        })
       .Otherwise(() =>
       {
           RuleFor(a => a.LoginRequest.PasswordHash)
         .Cascade(CascadeMode.Stop)
         .NotEmpty()
         .WithMessage(resourceJsonManager["XV0022"])
         .MinimumLength(7)
         .WithMessage(resourceJsonManager["XV0023"]);
           When(a => !string.IsNullOrEmpty(a.LoginRequest.Email), () =>
           {
               RuleFor(a => a.LoginRequest.Email)
               .Cascade(CascadeMode.Stop)
               .NotEmpty()
               .WithMessage(resourceJsonManager["XV0024"])
               .EmailAddress()
               .WithMessage(resourceJsonManager["XV0025"]);
           }).Otherwise(() =>
           {
               RuleFor(a => a.LoginRequest.MobileNumber)
                               .Cascade(CascadeMode.Stop)
                               .NotEmpty()
                               .WithMessage(resourceJsonManager["XV0026"])
                               .MinimumLength(11)
                               .WithMessage(resourceJsonManager["XV0027"])
                               .Matches(@"^[0-9]*$")
                               .WithMessage(resourceJsonManager["XV0028"])
                               .Matches(@"[0+][1+]\d")
                               .WithMessage(resourceJsonManager["XV0029"]);
           });
       });
    }
}
