using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.CQRS.Query;
using TransactionDomain.Features.Classes.DTO.Query;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;

namespace TransactionInfrastructure.Features.Classes.CQRS.Query;
public class SearchClassByTeacherQueryHandler : IRequestHandler<SearchClassByTeacherQuery, CommitResults<ClassResponse>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    public SearchClassByTeacherQueryHandler(IHttpContextAccessor httpContextAccessor, TrackerDbContext dbContext)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<CommitResults<ClassResponse>> Handle(SearchClassByTeacherQuery request, CancellationToken cancellationToken)
    {
        return new CommitResults<ClassResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<DomainEntities.TeacherClass>()
                                      .Where(a => a.TeacherId.Equals(_userId))
                                      .ProjectToType<ClassResponse>()
                                      .ToListAsync(cancellationToken)
        };
    }
}