using CurriculumDomain.Features.LessonClipScore.CQRS.Query;
using CurriculumDomain.Features.LessonClipScore.DTO.Query;
using CurriculumEntites.Entities;
using Microsoft.EntityFrameworkCore;
using ResultHandler;
using DomainEntitiesClips = CurriculumEntites.Entities.Clips;

namespace CurriculumInfrastructure.Features.LessonClipScore.CQRS.Query;
public class GetLessonClipScoresQueryHandler : IRequestHandler<GetLessonClipScoresQuery, CommitResult<List<LessonClipResponse>>>
{
    private readonly CurriculumDbContext _dbContext;

    public GetLessonClipScoresQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    //TODO: Add CommitResult
    public async Task<CommitResult<List<LessonClipResponse>>> Handle(GetLessonClipScoresQuery request, CancellationToken cancellationToken)
    {
        return new CommitResult<List<LessonClipResponse>>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<DomainEntitiesClips.Clip>()
                               .Where(a => a.LessonId.Equals(request.LessonId))
                               .Include(a => a.LessonFK)
                               .Select(a => new LessonClipResponse
                               {
                                   Id = a.Id,
                                   Name = a.Title,
                                   Ponits = a.Points,
                               }).ToListAsync(cancellationToken)
        };
    }
}