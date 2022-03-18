using IdentityDomain.Features.UpdateProfile.DTO.Command;
using ResultHandler;
namespace IdentityDomain.Features.UpdateProfile.CQRS.Command;

public record UpdateProfileCommand(UpdateProfileRequestDTO UpdateProfile) : IRequest<CommitResult<UpdateProfileResponseDTO>>;


