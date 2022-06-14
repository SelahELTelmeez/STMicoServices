using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherEntities.Entities.TeacherActivity;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;

public class GetClassesByQuizIdQueryHandler : IRequestHandler<GetClassesByQuizIdQuery, ICommitResults<ClassBriefResponse>>
{
    private readonly TeacherDbContext _dbContext;
    public GetClassesByQuizIdQueryHandler(TeacherDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<ICommitResults<ClassBriefResponse>> Handle(GetClassesByQuizIdQuery request, CancellationToken cancellationToken)
    {
        TeacherQuiz? teacherQuiz = await _dbContext.Set<TeacherQuiz>()
                                                   .Include(a => a.TeacherClasses)
                                                   .SingleOrDefaultAsync(a => a.Id == request.QuizId, cancellationToken);

        if (teacherQuiz == null)
        {
            return new CommitResults<ClassBriefResponse>
            {
                ErrorCode = "X0001",
                ErrorMessage = "X0001",
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
