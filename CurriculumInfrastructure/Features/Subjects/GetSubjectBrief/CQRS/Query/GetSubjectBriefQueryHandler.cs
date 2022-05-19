using CurriculumDomain.Features.Subjects.GetSubjectBrief.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;

namespace CurriculumInfrastructure.Features.Subjects.GetSubjectBrief.CQRS.Query;
public class GetSubjectsBriefQueryHandler : IRequestHandler<GetSubjectsBriefQuery, CommitResults<SubjectBriefResponse>>
{
    private readonly CurriculumDbContext _dbContext;

    public GetSubjectsBriefQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommitResults<SubjectBriefResponse>> Handle(GetSubjectsBriefQuery request, CancellationToken cancellationToken)
    {
        return new CommitResults<SubjectBriefResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<Subject>().Where(a => request.SubjectIds.Contains(a.Id)).ProjectToType<SubjectBriefResponse>().ToListAsync(cancellationToken)
        };
    }
}