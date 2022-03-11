using FluentValidation;
using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Command;

namespace IdentityDomain.Features.ExternalIdentityProvider.Validators.Command.Remove;
public class RemoveExternalIdentityProviderCommandValidator : AbstractValidator<RemoveExternalIdentityProviderCommand>
{
    public RemoveExternalIdentityProviderCommandValidator()
    {
        RuleFor(a => a.RemoveExternalIdentityProviderRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage("");
        RuleFor(a => a.RemoveExternalIdentityProviderRequest.Name).Cascade(CascadeMode.Stop).Empty().WithMessage("");
    }
}