using SharedModule.DTO;

namespace CurriculumDomain.HttpClients;

public interface IIdentityClient
{
    Task<CommitResult<int>?> GetStudentGradesAsync(string? StudentId, CancellationToken cancellationToken);
    Task<CommitResults<GradeResponse>?> GetGradesDetailesAsync(IEnumerable<int> GradeIds, CancellationToken cancellationToken);
}
