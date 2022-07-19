using FluentValidation;
using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Add.Command;

namespace IdentityDomain.Features.ExternalIdentityProvider.Validators.Command.Add;
public class AddExternalIdentityProviderCommandValidator : AbstractValidator<AddExternalIdentityProviderCommand>
{
    public AddExternalIdentityProviderCommandValidator()
    {
    }
}