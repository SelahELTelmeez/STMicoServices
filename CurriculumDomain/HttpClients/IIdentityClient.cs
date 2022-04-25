namespace CurriculumDomain.HttpClients;

public interface IIdentityClient
{
    Task<CommitResult<int>?> GetStudentGradesAsync(CancellationToken cancellationToken);
}
