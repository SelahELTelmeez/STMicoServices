using CurriculumDomain.Features.Lessons.GetLessonDetails.DTO.Query;
using CurriculumEntites.Entities;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DomainEntities = CurriculumEntites.Entities.Lessons;

namespace CurriculumDomain.Features.Lessons.GetLessonDetails.CQRS.Query;

public class GetLessonDetailsByIdQueryHandler : IRequestHandler<GetLessonDetailsQuery, CommitResult<LessonDetailsReponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetLessonDetailsByIdQueryHandler(CurriculumDbContext dbContext, IWebHostEnvironment configuration, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<CommitResult<LessonDetailsReponse>> Handle(GetLessonDetailsQuery request, CancellationToken cancellationToken)
    {
        DomainEntities.Lesson? lesson = await _dbContext.Set<DomainEntities.Lesson>().SingleOrDefaultAsync(a => a.Id.Equals(request.LessonId), cancellationToken: cancellationToken);
        if (lesson == null)
        {
            return new CommitResult<LessonDetailsReponse>
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "X0001",
                ErrorMessage = _resourceJsonManager["X0001"]
            };
        }
        return new CommitResult<LessonDetailsReponse>
        {
            ResultType = ResultType.Ok,
            Value = lesson.Adapt<LessonDetailsReponse>(),
        };
    }
}



