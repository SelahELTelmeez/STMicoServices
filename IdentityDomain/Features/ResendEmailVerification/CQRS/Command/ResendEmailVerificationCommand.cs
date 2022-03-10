﻿using IdentityDomain.Features.ResendEmailVerification.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.ResendEmailVerification.CQRS.Command;
public record ResendEmailVerificationCommand(ResendEmailVerificationRequestDTO ResendEmailVerificationRequest) : IRequest<CommitResult>;