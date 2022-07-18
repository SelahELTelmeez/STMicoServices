using TeacherDomain.Features.Quiz.CQRS.Query;
using TeacherDomain.Features.Quiz.DTO.Query;
using TeacherEntities.Entities.TeacherActivity;
using TeacherEntities.Entities.TeacherClasses;

namespace TeacherInfrastructure.Features.Quiz.CQRS.Query;

public class GetQuizByIdQueryHandler : IRequestHandler<GetQuizByIdQuery, ICommitResult<QuizResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetQuizByIdQueryHandler(TeacherDbContext dbContext,
                                  IHttpContextAccessor httpContextAccessor,
                                  IWebHostEnvironment configuration)

    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<ICommitResult<QuizResponse>> Handle(GetQuizByIdQuery request, CancellationToken cancellationToken)
    {
        TeacherQuiz? teacherQuiz = await _dbContext.Set<TeacherQuiz>()
                                                      .Where(a => a.Id.Equals(request.Id))
                                                      .FirstOrDefaultAsync(cancellationToken);
        if (teacherQuiz == null)
        {
            return ResultType.NotFound.GetValueCommitResult<QuizResponse>(default, "XTEC0008", _resourceJsonManager["XTEC0008"]);
        }

        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>()
                                                    .FirstOrDefaultAsync(a => a.Id == request.ClassId, cancellationToken);

        return ResultType.Ok.GetValueCommitResult(new QuizResponse
        {
            Description = teacherQuiz.Description,
            CreatedOn = teacherQuiz.StartDate,
            EndDate = teacherQuiz.EndDate,
            Id = teacherQuiz.Id,
            GetQuizDetailesId = teacherQuiz.QuizId,
            Title = teacherQuiz.Title,
            EnrolledCounter = 0,
            SubjectName = teacherQuiz.SubjectName,
            LessonName = teacherQuiz.LessonName,
            ClassName = teacherClass?.Name ?? string.Empty
        });
    }
}
