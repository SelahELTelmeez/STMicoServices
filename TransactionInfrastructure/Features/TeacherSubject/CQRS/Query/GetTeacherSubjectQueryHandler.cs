using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.TeacherSubject.CQRS.Query;
using TransactionDomain.Features.TeacherSubject.DTO.Query;
using TransactionEntites.Entities;
using TransactionInfrastructure.HttpClients;
using TransactionInfrastructure.Utilities;
using DomianEntities = TransactionEntites.Entities.TeacherSubjects;
namespace TransactionInfrastructure.Features.TeacherSubject.CQRS.Query;

public class GetTeacherSubjectQueryHandler : IRequestHandler<GetTeacherSubjectQuery, CommitResults<TeacherSubjectReponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly CurriculumClient _CurriculumClient;

    public GetTeacherSubjectQueryHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor, CurriculumClient curriculumClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _CurriculumClient = curriculumClient;
    }
    public async Task<CommitResults<TeacherSubjectReponse>?> Handle(GetTeacherSubjectQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<DomianEntities.TeacherSubject>? teacherSubjects = await _dbContext.Set<DomianEntities.TeacherSubject>()
                                                                                      .Where(a => a.TeacherId.Equals(_userId))
                                                                                      .ToListAsync(cancellationToken);
        if (teacherSubjects.Any())
        {
            return await _CurriculumClient.GetTeacherSubjectsDetailsAsync(teacherSubjects.Select(a => a.SubjectId), cancellationToken);
        }
        else
        {
            return new CommitResults<TeacherSubjectReponse>
            {
                ResultType = ResultType.Ok
            };
        }
    }
}
