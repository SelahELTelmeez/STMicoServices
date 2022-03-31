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
public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, CommitResult<bool>>
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

    public async Task<CommitResult<bool>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        Guid? IdentityUserId = _httpContextAccessor.GetIdentityUserId();

        if(IdentityUserId == null)
        {
            return new CommitResult<bool>
            {
                ErrorCode = "X0010",
                ErrorMessage = _resourceJsonManager["X0010"], // Duplicated User data, try to sign in instead.
                ResultType = ResultType.Invalid, // TODO: Add Result Type: Duplicated
            };
        }

        _dbContext.Set<DomainEntities.Quiz>().Add(new DomainEntities.Quiz
        {
            Creator = IdentityUserId.Value,
            LessonId = request.QuizRequest.LessonId,
            SubjectId = request.QuizRequest.SubjectId,
            UnitId = request.QuizRequest?.UnitId,
            QuizForms = request.QuizRequest.QuizFormRequests.Adapt<List<DomainEntities.QuizForm>>(),
            UserQuizs = request.QuizRequest.QuizFormRequests.Adapt<List<DomainEntities.UserQuiz>>()
        });
        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommitResult<bool>
        {
            ResultType = ResultType.Ok,
            Value = true
        };
    }
}