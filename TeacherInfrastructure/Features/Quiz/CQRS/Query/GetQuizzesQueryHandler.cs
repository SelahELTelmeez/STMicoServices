using TeacherDomain.Features.Assignment.CQRS.Query;
using TeacherDomain.Features.Quiz.DTO.Query;
using TeacherEntities.Entities.TeacherActivity;

namespace TeacherInfrastructure.Features.Quiz.CQRS.Query
{
    public class GetQuizzesQueryHandler : IRequestHandler<GetQuizzesQuery, CommitResults<QuizResponse>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly Guid? _userId;
        public GetQuizzesQueryHandler(TeacherDbContext dbContext, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
        }

        public async Task<CommitResults<QuizResponse>> Handle(GetQuizzesQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<TeacherQuiz> teacherQuizzes = await _dbContext.Set<TeacherQuiz>()
                                                                      .Where(a => a.Creator.Equals(_userId))
                                                                      .Include(a => a.TeacherClasses)
                                                                      .ThenInclude(a => a.ClassEnrollees)
                                                                      .ToListAsync(cancellationToken);

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
                        Title = quiz.Title,
                        EntrolledCounter = quiz.TeacherClasses.SelectMany(a => a.ClassEnrollees).Where(a => a.IsActive).Count(),
                    };
                }
                yield break;
            }

            return new CommitResults<QuizResponse>
            {
                ResultType = ResultType.Ok,
                Value = Mapper()
            };
        }
    }
}
