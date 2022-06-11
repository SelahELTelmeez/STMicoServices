using CurriculumDomain.Features.Subjects.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.MCQS;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CurriculumInfrastructure.Features.Subjects.CQRS.Query
{
    public class CheckAnyMCQExistenceBySubjectIdQueryHandler : IRequestHandler<CheckAnyMCQExistenceBySubjectIdQuery, CommitResult<bool>>
    {
        private readonly CurriculumDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;

        public CheckAnyMCQExistenceBySubjectIdQueryHandler(CurriculumDbContext dbContext,
                                                           IWebHostEnvironment configuration,
                                                           IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }
        public async Task<CommitResult<bool>> Handle(CheckAnyMCQExistenceBySubjectIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<CurriculumEntites.Entities.Units.Unit>? units = await _dbContext.Set<CurriculumEntites.Entities.Units.Unit>().Include(a => a.Lessons).Where(a => a.SubjectId.Equals(request.SubjectId)).ToListAsync(cancellationToken);

            if (!units.Any())
            {
                return new CommitResult<bool>
                {
                    ResultType = ResultType.NotFound,
                    ErrorCode = "X0005",
                    ErrorMessage = _resourceJsonManager["X0005"]
                };
            }

            IEnumerable<int> lessonIds = units.SelectMany(a => a.Lessons).Select(a => a.Id);

            return new CommitResult<bool>
            {
                ResultType = ResultType.Ok,
                Value = await _dbContext.Set<MCQ>().Where(a => a.LessonId.HasValue).AnyAsync(a => lessonIds.Contains(a.LessonId.GetValueOrDefault()))
            };
        }
    }
}
