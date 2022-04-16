using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.SearchClass.CQRS.Query;
using TransactionDomain.Features.Classes.SearchClass.DTO.Query;
using TransactionEntites.Entities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Classes.SearchClass.CQRS.Query;
public class SearchClassQueryHandler : IRequestHandler<SearchClassQuery, CommitResult<ClassResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    public SearchClassQueryHandler(IHttpContextAccessor httpContextAccessor, TrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<CommitResult<ClassResponse>> Handle(SearchClassQuery request, CancellationToken cancellationToken)
    {
        DomainEntities.TeacherClass? teacherClass = await _dbContext.Set<DomainEntities.TeacherClass>()
                                      .Where(a => a.TeacherId.Equals(_userId) && a.Id.Equals(request.ClassId))
                                      .SingleOrDefaultAsync(cancellationToken);
        return new CommitResult<ClassResponse>
        {
            ResultType = ResultType.Ok,
            Value = teacherClass?.Adapt<ClassResponse>()
        };
    }
}