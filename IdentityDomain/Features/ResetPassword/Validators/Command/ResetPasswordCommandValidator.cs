using FluentValidation;
using IdentityDomain.Features.ResetPassword.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.ResetPassword.Validators.Command;
public class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        When(a => !string.IsNullOrEmpty(a.ResetPasswordRequest.Email), () =>
        {
            RuleFor(a => a.ResetPasswordRequest.Email)
                           .Cascade(CascadeMode.Stop)
                           .NotEmpty()
                           .WithMessage(resourceJsonManager["XV0044"])
                           .EmailAddress()
                           .WithMessage(resourceJsonManager["XV0045"]);
        }).Otherwise(() =>
        {
            RuleFor(a => a.ResetPasswordRequest.MobileNumber)
                                    .Cascade(CascadeMode.Stop)
                                    .NotEmpty()
                                    .WithMessage(resourceJsonManager["XV0046"])
                                    .MinimumLength(11)
                                    .WithMessage(resourceJsonManager["XV0047"])
                                    .Matches(@"^[0-9]*$")
                                    .WithMessage(resourceJsonManager["XV0048"])
                                    .Matches(@"[0+][1+]\d")
                                    .WithMessage(resourceJsonManager["XV0049"]);

            RuleFor(a => a.ResetPasswordRequest.VerificationCode)
                .Cascade(CascadeMode.Stop)
                .NotEmpty()
                .WithMessage(resourceJsonManager["XV0050"]);
        });


        RuleFor(a => a.ResetPasswordRequest.NewPassword)
        .Cascade(CascadeMode.Stop)
        .NotEmpty()
        .WithMessage(resourceJsonManager["XV0051"])
        .MinimumLength(7)
        .WithMessage(resourceJsonManager["XV0052"]);
    }
}