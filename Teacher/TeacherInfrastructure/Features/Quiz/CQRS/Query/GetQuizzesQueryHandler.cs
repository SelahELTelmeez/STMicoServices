using TeacherDomain.Features.Assignment.CQRS.Query;
using TeacherDomain.Features.Quiz.DTO.Query;
using TeacherEntities.Entities.TeacherActivity;

namespace TeacherInfrastructure.Features.Quiz.CQRS.Query
{
    public class GetQuizzesQueryHandler : IRequestHandler<GetQuizzesQuery, ICommitResults<QuizResponse>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly string? _userId;
        private readonly JsonLocalizerManager _resourceJsonManager;

        public GetQuizzesQueryHandler(TeacherDbContext dbContext,
                                      IHttpContextAccessor httpContextAccessor,
                                      IWebHostEnvironment configuration)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }

        public async Task<ICommitResults<QuizResponse>> Handle(GetQuizzesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<TeacherQuiz> teacherQuizzes = await _dbContext.Set<TeacherQuiz>()
                                                                      .Where(a => a.Creator.Equals(_userId))
                                                                      .Include(a => a.TeacherClasses)
                                                                      .ThenInclude(a => a.ClassEnrollees)
                                                                      .ToListAsync(cancellationToken);
            if (!teacherQuizzes.Any())
            {
                return ResultType.NotFound.GetValueCommitResults(Array.Empty<QuizResponse>());
            }

            IEnumerable<QuizResponse> Mapper()
            {
                foreach (TeacherQuiz quiz in teacherQuizzes)
                {
                    yield return new QuizResponse
                    {
                        Description = quiz.Description,
                        CreatedOn = quiz.StartDate,
                        EndDate = quiz.EndDate,
                        Id = quiz.Id,
                        GetQuizDetailesId = quiz.QuizId,
                        LessonName = quiz.LessonName,
                        SubjectName = quiz.SubjectName,
                        ClassName = string.Empty,
                        Title = quiz.Title,
                        EnrolledCounter = quiz.TeacherClasses.SelectMany(a => a.ClassEnrollees).Where(a => a.IsActive).Count(),
                    };
                }
                yield break;
            }

            return ResultType.Ok.GetValueCommitResults(Mapper());
        }
    }
}
