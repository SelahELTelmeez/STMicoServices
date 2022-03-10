using IdentityDomain.Features.ResetPassword.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.ResetPassword.CQRS.Command;
public record IdentityResetPasswordCommand(IdentityResetPasswordRequestDTO IdentityResetPasswordRequest) : IRequest<CommitResult<IdentityResetPasswordResponseDTO>>;