using CurriculumDomain.Features.Lessons.GetClipsBrief.CQRS.Query;
using CurriculumEntites.Entities;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;
using DomainEntitiesClips = CurriculumEntites.Entities.Clips;

namespace CurriculumInfrastructure.Features.Lessons.GetClipsBrief.CQRS.Query;
public class GetClipsBriefByLessonIdQueryHandler : IRequestHandler<GetClipsBriefByLessonIdQuery, CommitResults<ClipBriefResponse>>
{
    private readonly CurriculumDbContext _dbContext;

    public GetClipsBriefByLessonIdQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommitResults<ClipBriefResponse>> Handle(GetClipsBriefByLessonIdQuery request, CancellationToken cancellationToken)
    {
        return new CommitResults<ClipBriefResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<DomainEntitiesClips.Clip>()
                               .Where(a => a.LessonId.Equals(request.LessonId))
                               .Select(a => new ClipBriefResponse
                               {
                                   Id = a.Id,
                                   Name = a.Title,
                                   Ponits = a.Points,
                               }).ToListAsync(cancellationToken)
        };
    }
}