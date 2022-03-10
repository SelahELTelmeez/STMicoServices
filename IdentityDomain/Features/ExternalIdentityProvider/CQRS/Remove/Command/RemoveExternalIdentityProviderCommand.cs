﻿using IdentityDomain.Features.ExternalIdentityProvider.DTO.Remove.Command;
using ResultHandler;

namespace IdentityDomain.Features.ExternalIdentityProvider.CQRS.Command;
public record RemoveExternalIdentityProviderCommand(RemoveExternalIdentityProviderRequestDTO RemoveExternalIdentityProviderRequest) : IRequest<CommitResult>;