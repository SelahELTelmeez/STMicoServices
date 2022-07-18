using CurriculumDomain.Features.Lessons.GetLessonDetails.CQRS.Query;
using CurriculumDomain.Features.Lessons.GetLessonDetails.DTO.Query;
using CurriculumEntites.Entities;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using DomainEntities = CurriculumEntites.Entities.Lessons;

namespace CurriculumInfrastructure.Features.Lessons.GetLessonDetails.CQRS.Query;

public class GetLessonDetailsByIdQueryHandler : IRequestHandler<GetLessonDetailsQuery, CommitResult<LessonDetailsReponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IDistributedCache _cache;

    public GetLessonDetailsByIdQueryHandler(CurriculumDbContext dbContext, IWebHostEnvironment configuration, IHttpContextAccessor httpContextAccessor, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _cache = cache;
    }

    public async Task<CommitResult<LessonDetailsReponse>> Handle(GetLessonDetailsQuery request, CancellationToken cancellationToken)
    {
        LessonDetailsReponse? cachedLessonDetailsReponse = await _cache.GetFromCacheAsync<int, LessonDetailsReponse>(request.LessonId, "Curriculum-GetLessonDetails", cancellationToken);

        if (cachedLessonDetailsReponse == null)
        {
            DomainEntities.Lesson? lesson = await _dbContext.Set<DomainEntities.Lesson>().FirstOrDefaultAsync(a => a.Id.Equals(request.LessonId), cancellationToken: cancellationToken);
            if (lesson == null)
            {
                return new CommitResult<LessonDetailsReponse>
                {
                    ResultType = ResultType.NotFound,
                    ErrorCode = "XCUR0001",
                    ErrorMessage = _resourceJsonManager["XCUR0001"]
                };
            }

            cachedLessonDetailsReponse = lesson.Adapt<LessonDetailsReponse>();

            await _cache.SaveToCacheAsync(request.LessonId, cachedLessonDetailsReponse, "Curriculum-GetLessonDetails", cancellationToken);

        }
        return new CommitResult<LessonDetailsReponse>
        {
            ResultType = ResultType.Ok,
            Value = cachedLessonDetailsReponse
        };

    }
}



