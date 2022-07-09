using CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;
namespace CurriculumInfrastructure.Features.Subjects.GetSubjects.CQRS.Query;

public class GetSubjectsBriefByTermQueryHandler : IRequestHandler<GetSubjectsBriefByTermQuery, CommitResults<SubjectBriefResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    public GetSubjectsBriefByTermQueryHandler(CurriculumDbContext dbContext)

    {
        _dbContext = dbContext;
    }
    public async Task<CommitResults<SubjectBriefResponse>> Handle(GetSubjectsBriefByTermQuery request, CancellationToken cancellationToken)
    {
        return new CommitResults<SubjectBriefResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<Subject>()
                                    .Where(a => a.IsAppShow == true)
                                    .Where(a => a.Grade == request.Grade && a.Term == request.TermId)
                                    .ProjectToType<SubjectBriefResponse>()
                                    .ToListAsync(cancellationToken)
        };
    }
}
