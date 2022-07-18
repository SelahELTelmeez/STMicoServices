using Flaminco.CommitResult;
using IdentityDomain.Features.UpdateProfile.DTO.Command;

namespace IdentityDomain.Features.UpdateProfile.CQRS.Command;

public record UpdateProfileCommand(UpdateProfileRequest UpdateProfile) : IRequest<ICommitResult>;


