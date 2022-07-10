using Flaminco.CommitResult;
using IdentityDomain.Features.ForgetPassword.DTO.Command;

namespace IdentityDomain.Features.ForgetPassword.CQRS.Command;
public record ForgetPasswordCommand(ForgetPasswordRequest ForgetPasswordRequest) : IRequest<ICommitResult>;