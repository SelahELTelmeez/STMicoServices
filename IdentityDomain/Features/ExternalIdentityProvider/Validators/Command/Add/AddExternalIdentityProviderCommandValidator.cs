using FluentValidation;
using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Add.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.ExternalIdentityProvider.Validators.Command.Add;
public class AddExternalIdentityProviderCommandValidator : AbstractValidator<AddExternalIdentityProviderCommand>
{
    public AddExternalIdentityProviderCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.AddExternalIdentityProviderRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage(resourceJsonManager["XV0013"]);
        RuleFor(a => a.AddExternalIdentityProviderRequest.Name).Cascade(CascadeMode.Stop).Empty().WithMessage(resourceJsonManager["XV0014"]);
    }
}