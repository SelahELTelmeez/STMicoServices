using IdentityDomain.Features.ChangePassword.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.ChangePassword.CQRS.Command;
public record ChangePasswordCommand(ChangePasswordRequest ChangePasswordRequest) : IRequest<CommitResult>;