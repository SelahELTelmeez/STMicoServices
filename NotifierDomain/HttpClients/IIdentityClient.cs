using SharedModule.DTO;

namespace NotifierDomain.HttpClients;

public interface IIdentityClient
{
    public Task<ICommitResults<LimitedProfileResponse>?> GetLimitedProfilesAsync(IEnumerable<Guid> Identities, CancellationToken cancellationToken);
}
