using CurriculumDomain.Features.GetCurriculumLesson.CQRS.Query;
using CurriculumDomain.Features.GetCurriculumLesson.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.Lessons;
using JsonLocalizer;
using Mapster;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace CurriculumInfrastructure.Features.GetCurriculumLesson.CQRS.Query;
public class GetCurriculumLessonQueryHandler : IRequestHandler<GetCurriculumLessonQuery, CommitResult<List<CurriculumLessonClipResponseDTO>>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetCurriculumLessonQueryHandler(CurriculumDbContext dbContext, JsonLocalizerManager resourceJsonManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
    }

    public async Task<CommitResult<List<CurriculumLessonClipResponseDTO>>> Handle(GetCurriculumLessonQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the Curriculum Id existance first, with the provided data.
        List<CurriculumLessonClipResponseDTO>? curriculumLessonClips = await _dbContext.Set<Clip>().Where(a => a.LessonId.Equals(request.LessonId)).Include(x => x.LessonFK).ProjectToType<CurriculumLessonClipResponseDTO>().ToListAsync(cancellationToken);
        
        if (curriculumLessonClips == null)
        {
            return new CommitResult<List<CurriculumLessonClipResponseDTO>>
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"], // Data of student Curriculum Details is not exist.
                ResultType = ResultType.NotFound,
            };
        }

        return new CommitResult<List<CurriculumLessonClipResponseDTO>>
        {
            ResultType = ResultType.Ok,
            Value = curriculumLessonClips
        };
    }
}