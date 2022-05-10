using CurriculumDomain.Features.Lessons.GetLessonsBrief.CQRS.Query;
using CurriculumEntites.Entities;
using Mapster;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;
using DomainEntities = CurriculumEntites.Entities.Lessons;
namespace CurriculumInfrastructure.Features.Lessons.GetLessonsBrief.CQRS.Query;

public class GetLessonsBriefQueryHandler : IRequestHandler<GetLessonsBriefQuery, CommitResults<LessonBriefResponse>>
{
    private readonly CurriculumDbContext _dbContext;

    public GetLessonsBriefQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task<CommitResults<LessonBriefResponse>> Handle(GetLessonsBriefQuery request, CancellationToken cancellationToken)
        => new CommitResults<LessonBriefResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<DomainEntities.Lesson>().Where(a => request.LessonIds.Contains(a.Id)).ProjectToType<LessonBriefResponse>().ToListAsync(cancellationToken)
        };
}

