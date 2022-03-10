using FluentValidation;
using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Add.Command;

namespace IdentityDomain.Features.ExternalIdentityProvider.Validators.Command.Add;
public class AddExternalIdentityProviderCommandValidator : AbstractValidator<AddExternalIdentityProviderCommand>
{
    public AddExternalIdentityProviderCommandValidator()
    {
        RuleFor(a => a.AddExternalIdentityProviderRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.AddExternalIdentityProviderRequest.Name).Cascade(CascadeMode.Stop).Empty().WithMessage("");
        RuleFor(a => a.AddExternalIdentityProviderRequest.IdentityUserId).Cascade(CascadeMode.Stop).Empty().WithMessage("");
    }
}