using SharedModule.DTO;

namespace NotifierDomain.HttpClients;

public interface IIdentityClient
{
    public Task<CommitResults<LimitedProfileResponse>?> GetLimitedProfilesAsync(IEnumerable<Guid> Identities, CancellationToken cancellationToken);
}
