using FluentValidation;
using IdentityDomain.Features.ExternalIdentityProvider.CQRS.Command;
using JsonLocalizer;

namespace IdentityDomain.Features.ExternalIdentityProvider.Validators.Command.Remove;
public class RemoveExternalIdentityProviderCommandValidator : AbstractValidator<RemoveExternalIdentityProviderCommand>
{
    public RemoveExternalIdentityProviderCommandValidator(JsonLocalizerManager resourceJsonManager)
    {
        RuleFor(a => a.RemoveExternalIdentityProviderRequest).Cascade(CascadeMode.Stop).NotNull().WithMessage(resourceJsonManager["XV0015"]);
        RuleFor(a => a.RemoveExternalIdentityProviderRequest.Name).Cascade(CascadeMode.Stop).Empty().WithMessage(resourceJsonManager["XV0016"]);
    }
}