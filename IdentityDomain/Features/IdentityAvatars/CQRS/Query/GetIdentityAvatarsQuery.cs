using Flaminco.CommitResult;
using IdentityDomain.Features.IdentityAvatars.DTO.Query;

namespace IdentityDomain.Features.IdentityAvatars.CQRS.Query;
public record class GetIdentityAvatarsQuery(Guid? UserId) : IRequest<ICommitResults<IdentityAvatarResponse>>;