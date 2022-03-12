using FluentValidation;
using IdentityDomain.Features.ChangeEmailOrMobile.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.ChangeEmailOrMobile.Validator.Command;
public class ChangeEmailOrMobileCommandValidator : AbstractValidator<ChangeEmailOrMobileCommand>
{
    public ChangeEmailOrMobileCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.ChangeEmailOrMobileRequest.Password).Cascade(CascadeMode.Stop)
                      .NotEmpty()
                      .WithMessage(resourceJsonManager["XV0001"]);

        When(a => !string.IsNullOrEmpty(a.ChangeEmailOrMobileRequest.NewEmail), () =>
              RuleFor(a => a.ChangeEmailOrMobileRequest.NewEmail)
             .Cascade(CascadeMode.Stop)
             .NotEmpty()
             .WithMessage(resourceJsonManager["XV0002"])
             .EmailAddress()
             .WithMessage("Is not Correct")
            ).Otherwise(() =>
            {
                RuleFor(a => a.ChangeEmailOrMobileRequest.NewMobileNumber)
                 .Cascade(CascadeMode.Stop)
                 .NotEmpty()
                 .WithMessage(resourceJsonManager["XV0004"])
                 .MinimumLength(11)
                 .WithMessage(resourceJsonManager["XV0005"])
                 .Matches(@"^[0-9]*$")
                 .WithMessage(resourceJsonManager["XV0006"]) 
                 .Matches(@"[0+][1+]\d")
                 .WithMessage("NewMobileNumber Is not Correct");
            });
    }
}