using IdentityDomain.Features.IdentityAvatars.DTO.Query;
using ResultHandler;

namespace IdentityDomain.Features.IdentityAvatars.CQRS.Query;
public record class GetIdentityAvatarsQuery() : IRequest<CommitResult<List<IdentityAvatarResponse>>>;