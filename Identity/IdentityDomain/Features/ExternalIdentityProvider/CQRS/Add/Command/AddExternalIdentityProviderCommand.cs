﻿using Flaminco.CommitResult;
using IdentityDomain.Features.ExternalIdentityProvider.DTO.Add.Command;

namespace IdentityDomain.Features.ExternalIdentityProvider.CQRS.Add.Command;
public record AddExternalIdentityProviderCommand(AddExternalIdentityProviderRequest AddExternalIdentityProviderRequest) : IRequest<ICommitResult>;