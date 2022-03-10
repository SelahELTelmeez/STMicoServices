using IdentityDomain.Features.ForgetPassword.DTO.Command;
using ResultHandler;

namespace IdentityDomain.Features.ForgetPassword.CQRS.Command;
public record IdentityForgetPasswordCommand(IdentityForgetPasswordRequestDTO IdentityForgetPasswordRequest) : IRequest<CommitResult<bool>>;