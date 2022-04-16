using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.SearchClassByTeacher.CQRS.Query;
using TransactionDomain.Features.Classes.SearchClassByTeacher.DTO.Query;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;

namespace TransactionInfrastructure.Features.Classes.SearchClassByTeacher.CQRS.Query;
public class SearchClassByTeacherQueryHandler : IRequestHandler<SearchClassByTeacherQuery, CommitResults<SearchClassByTeacherResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    public SearchClassByTeacherQueryHandler(IHttpContextAccessor httpContextAccessor, TrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<CommitResults<SearchClassByTeacherResponse>> Handle(SearchClassByTeacherQuery request, CancellationToken cancellationToken)
    {
        List<DomainEntities.TeacherClass>? teacherClass = await _dbContext.Set<DomainEntities.TeacherClass>()
                                      .Where(a => a.TeacherId.Equals(_userId))
                                      .ToListAsync(cancellationToken);

        return new CommitResults<SearchClassByTeacherResponse>
        {
            ResultType = ResultType.Ok,
            Value = teacherClass?.Adapt<List<SearchClassByTeacherResponse>>()
        };
    }
}