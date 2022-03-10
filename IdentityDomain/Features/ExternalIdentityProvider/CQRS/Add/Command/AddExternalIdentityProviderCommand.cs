﻿using IdentityDomain.Features.ExternalIdentityProvider.DTO.Add.Command;
using ResultHandler;

namespace IdentityDomain.Features.ExternalIdentityProvider.CQRS.Add.Command;
public record AddExternalIdentityProviderCommand(AddExternalIdentityProviderRequestDTO AddExternalIdentityProviderRequest) : IRequest<CommitResult>;