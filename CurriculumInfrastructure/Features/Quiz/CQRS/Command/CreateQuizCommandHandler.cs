using CurriculumDomain.Features.Quiz.CQRS.Command;
using CurriculumEntites.Entities;
using DomainEntities = CurriculumEntites.Entities.Quizzes;
using CurriculumInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ResultHandler;
using Microsoft.EntityFrameworkCore;
using Mapster;

namespace CurriculumInfrastructure.Features.Quiz.CQRS.Command;
public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, CommitResult<int>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateQuizCommandHandler(CurriculumDbContext dbContext,
                                         IWebHostEnvironment configuration,
                                         IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Here We will create new Quiz with list of Quiz Form and List of User Quizs
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CommitResult<int>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        // Here we get the id of user to create quiz for this user. 
        Guid? IdentityUserId = _httpContextAccessor.GetIdentityUserId();

        // Here we check if user id is not null or not.
        if (IdentityUserId == null)
        {
            return new CommitResult<int>
            {
                ErrorCode = "X0010",
                ErrorMessage = _resourceJsonManager["X0010"], // Duplicated User data, try to sign in instead.
                ResultType = ResultType.Invalid, // TODO: Add Result Type: Duplicated
            };
        }

        // Here we set data of quiz then add this data to table of quiz then save changes
        DomainEntities.Quiz quiz = new DomainEntities.Quiz
        {
            Creator = IdentityUserId.Value,
            LessonId = request.QuizRequest.LessonId,
            SubjectId = request.QuizRequest.SubjectId,
            UnitId = request.QuizRequest?.UnitId,
            QuizForms = request.QuizRequest.QuizFormRequests.Adapt<List<DomainEntities.QuizForm>>(),
            UserQuizs = request.QuizRequest.QuizFormRequests.Adapt<List<DomainEntities.UserQuiz>>()
        };

        _dbContext.Set<DomainEntities.Quiz>().Add(quiz);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Here we'll return id of quiz
        return new CommitResult<int>
        {
            ResultType = ResultType.Ok,
            Value = quiz.Id
        };
    }
}