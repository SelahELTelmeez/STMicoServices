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
public class GetCurriculumLessonQueryHandler : IRequestHandler<GetCurriculumLessonQuery, CommitResult<CurriculumLessonClipResponseDTO>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetCurriculumLessonQueryHandler(CurriculumDbContext dbContext, JsonLocalizerManager resourceJsonManager)
    {
        _dbContext = dbContext;
        _resourceJsonManager = resourceJsonManager;
    }

    public async Task<CommitResult<CurriculumLessonClipResponseDTO>> Handle(GetCurriculumLessonQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the Curriculum Id existance first, with the provided data.
        List<CurriculumClipResponseDTO>? curriculumLessonClips = await _dbContext.Set<Clip>()
                                                                                 .Where(a => a.LessonId.Equals(request.LessonId))
                                                                                 .ProjectToType<CurriculumClipResponseDTO>()
                                                                                 .ToListAsync();
        
        if (curriculumLessonClips == null)
        {
            return new CommitResult<CurriculumLessonClipResponseDTO>
            {
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"], // Data of student Curriculum Details is not exist.
                ResultType = ResultType.NotFound,
            };
        }

        return new CommitResult<CurriculumLessonClipResponseDTO>
        {
            ResultType = ResultType.Ok,
            Value = new CurriculumLessonClipResponseDTO
            {
                Clips = curriculumLessonClips,
                Types = curriculumLessonClips.Select(a => a.Type.Value).Distinct().ToList()
            }
        };
    }
}