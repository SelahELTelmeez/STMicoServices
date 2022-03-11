using FluentValidation;
using IdentityDomain.Features.ChangeEmailOrMobile.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.ChangeEmailOrMobile.Validator.Command;
public class ChangeEmailOrMobileCommandValidator : AbstractValidator<ChangeEmailOrMobileCommand>
{
    public ChangeEmailOrMobileCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.ChangeEmailOrMobileRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage(resourceJsonManager["XV0111"]);
        RuleFor(a => a.ChangeEmailOrMobileRequest.Password).Cascade(CascadeMode.Stop).Empty().WithMessage(resourceJsonManager["XV0111"]);
        RuleFor(a => a.ChangeEmailOrMobileRequest.NewEmail).Cascade(CascadeMode.Stop).Empty().WithMessage(resourceJsonManager["XV0111"]);
        RuleFor(a => a.ChangeEmailOrMobileRequest.NewMobileNumber).Cascade(CascadeMode.Stop).Empty().WithMessage(resourceJsonManager["XV0111"]);
    }
}