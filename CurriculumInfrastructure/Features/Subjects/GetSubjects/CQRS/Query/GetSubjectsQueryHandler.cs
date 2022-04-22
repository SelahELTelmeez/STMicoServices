using CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetSubjects.DTO.Query;
using CurriculumEntites.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using DomainEntitiesSubjects = CurriculumEntites.Entities.Subjects;
namespace CurriculumInfrastructure.Features.Subjects.GetSubjects.CQRS.Query;
public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, CommitResults<SubjectResponse>>
{
    private readonly CurriculumDbContext _dbContext;

    public GetSubjectsQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommitResults<SubjectResponse>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
    {
        return new CommitResults<SubjectResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<DomainEntitiesSubjects.Subject>()
                              .Where(a => request.SubjectIds.Contains(a.Id))
                              .ProjectToType<SubjectResponse>()
                              .ToListAsync(cancellationToken)
        };
    }

}