using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.SearchClassBySubject.CQRS.Query;
using TransactionDomain.Features.Classes.SearchClassBySubject.DTO.Query;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;

namespace TransactionInfrastructure.Features.Classes.SearchClassBySubject.CQRS.Query;
public class SearchClassBySubjectQueryHandler : IRequestHandler<SearchClassBySubjectQuery, CommitResults<SearchClassBySubjectResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    public SearchClassBySubjectQueryHandler(IHttpContextAccessor httpContextAccessor, TrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<CommitResults<SearchClassBySubjectResponse>> Handle(SearchClassBySubjectQuery request, CancellationToken cancellationToken)
    {
        List<DomainEntities.TeacherClass>? teacherClass = await _dbContext.Set<DomainEntities.TeacherClass>()
                                      .Where(a => a.TeacherId.Equals(_userId) && a.SubjectId.Equals(request.SubjectId))
                                      .ToListAsync(cancellationToken);
        return new CommitResults<SearchClassBySubjectResponse>
        {
            ResultType = ResultType.Ok,
            Value = teacherClass?.Adapt<List<SearchClassBySubjectResponse>>()
        };
    }
}