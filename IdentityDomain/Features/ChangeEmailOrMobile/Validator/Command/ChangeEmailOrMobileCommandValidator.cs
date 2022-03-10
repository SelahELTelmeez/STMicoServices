using FluentValidation;
using IdentityDomain.Features.ChangeEmailOrMobile.CQRS.Command;

namespace IdentityDomain.Features.ChangeEmailOrMobile.Validator.Command
{
    internal class ChangeEmailOrMobileCommandValidator
    {
    }
}
public class ChangeEmailOrMobileCommandValidator : AbstractValidator<ChangeEmailOrMobileCommand>
{
    public ChangeEmailOrMobileCommandValidator()
    {
        RuleFor(a => a.ChangeEmailOrMobileRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.ChangeEmailOrMobileRequest.IdentityUserId).Cascade(CascadeMode.Stop).Empty().WithMessage("");
        RuleFor(a => a.ChangeEmailOrMobileRequest.Password).Cascade(CascadeMode.Stop).Empty().WithMessage("");
        RuleFor(a => a.ChangeEmailOrMobileRequest.NewEmail).Cascade(CascadeMode.Stop).Empty().WithMessage("");
        RuleFor(a => a.ChangeEmailOrMobileRequest.NewMobileNumber).Cascade(CascadeMode.Stop).Empty().WithMessage("");
    }
}