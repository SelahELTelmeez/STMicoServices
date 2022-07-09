using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherEntities.Entities.TeacherActivity;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;

public class GetClassesByQuizIdQueryHandler : IRequestHandler<GetClassesByQuizIdQuery, ICommitResults<ClassBriefResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetClassesByQuizIdQueryHandler(TeacherDbContext dbContext, IWebHostEnvironment configuration,
                                     IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<ICommitResults<ClassBriefResponse>> Handle(GetClassesByQuizIdQuery request, CancellationToken cancellationToken)
    {
        TeacherQuiz? teacherQuiz = await _dbContext.Set<TeacherQuiz>()
                                                   .Include(a => a.TeacherClasses)
                                                   .FirstOrDefaultAsync(a => a.Id == request.QuizId, cancellationToken);

        if (teacherQuiz == null)
        {
            return new CommitResults<ClassBriefResponse>
            {
                ErrorCode = "XTEC0010",
                ErrorMessage = _resourceJsonManager["XTEC0010"],
                ResultType = ResultType.Ok
            };
        }


        IEnumerable<ClassBriefResponse> Mapper()
        {
            foreach (var item in teacherQuiz.TeacherClasses)
            {
                yield return new ClassBriefResponse
                {
                    Id = item.Id,
                    Name = item.Name
                };
            }
        }

        return ResultType.Ok.GetValueCommitResults(Mapper());
    }
}
