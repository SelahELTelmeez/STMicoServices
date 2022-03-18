using IdentityDomain.Features.GetAvatars.DTO.Query;
using ResultHandler;

namespace IdentityDomain.Features.GetAvatars.CQRS.Query;
public record class GetAvatarsQuery() : IRequest<CommitResult<List<AvatarResponseDTO>>>;