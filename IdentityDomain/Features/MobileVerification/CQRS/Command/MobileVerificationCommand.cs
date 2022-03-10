using IdentityDomain.Features.MobileVerification.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.MobileVerification.CQRS.Command;
public record MobileVerificationCommand(MobileVerificationRequestDTO MobileVerificationRequest) : IRequest<CommitResult>;