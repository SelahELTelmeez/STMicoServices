using CurriculumDomain.Features.LessonClip.CQRS.Query;
using CurriculumDomain.Features.LessonClip.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Clips;
using CurriculumInfrastructure.Utilities;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using ResultHandler;

namespace CurriculumInfrastructure.Features.LessonClip.CQRS.Query;
public class GetLessonClipQueryHandler : IRequestHandler<GetLessonClipQuery, CommitResult<LessonClipResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetLessonClipQueryHandler(CurriculumDbContext dbContext,
                                           IWebHostEnvironment configuration,
                                           IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<CommitResult<LessonClipResponse>> Handle(GetLessonClipQuery request, CancellationToken cancellationToken)
    {
        // 1.0 Check for the Subject Id existance first, with the provided data.
        List<Clip>? clips = await _dbContext.Set<Clip>()
                                            .Where(a => a.LessonId.Equals(request.LessonId))
                                            .OrderBy(a => a.Sort)
                                            .Include(a => a.LessonFK)
                                            .ThenInclude(a => a.UnitFK)
                                            .ThenInclude(a => a.SubjectFK)
                                            .ToListAsync(cancellationToken);

        if (clips == null)
        {
            return new CommitResult<LessonClipResponse>
            {
                ErrorCode = "X0015",
                ErrorMessage = _resourceJsonManager["X0015"], // Data of student Subject Details is not exist.
                ResultType = ResultType.NotFound,
            };
        }

        return new CommitResult<LessonClipResponse>
        {
            ResultType = ResultType.Ok,
            Value = new LessonClipResponse
            {
                Clips = clips.Adapt<List<ClipResponse>>(),
                Types = clips.Adapt<List<FilterTypesResponse>>().Distinct().ToList()
            }
        };
    }
}