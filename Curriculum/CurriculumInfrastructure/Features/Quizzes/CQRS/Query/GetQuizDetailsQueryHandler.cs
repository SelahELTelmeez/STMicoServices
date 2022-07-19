using CurriculumDomain.Features.Quizzes.CQRS.Query;
using CurriculumDomain.Features.Quizzes.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Shared;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using DomainEntities = CurriculumEntites.Entities.Quizzes;

namespace CurriculumInfrastructure.Features.Quizzes.CQRS.Query;
public class GetQuizDetailsQueryHandler : IRequestHandler<GetQuizDetailsQuery, CommitResult<QuizDetailsResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IDistributedCache _cahce;
    public GetQuizDetailsQueryHandler(CurriculumDbContext dbContext,
                                      IWebHostEnvironment configuration,
                                      IHttpContextAccessor httpContextAccessor,
                                      IDistributedCache cahce)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _cahce = cahce;
    }
    public async Task<CommitResult<QuizDetailsResponse>> Handle(GetQuizDetailsQuery request, CancellationToken cancellationToken)
    {

        QuizDetailsResponse? cachedQuizDetailsResponse = await _cahce.GetFromCacheAsync<int, QuizDetailsResponse>(request.QuizId, "Curriculum-GetQuizDetails", cancellationToken);

        if (cachedQuizDetailsResponse == null)
        {
            DomainEntities.Quiz? quiz = await _dbContext.Set<DomainEntities.Quiz>()?
                                                                  .Where(a => a.Id == request.QuizId)?
                                                                  .Include(a => a.QuizForms)?
                                                                  .ThenInclude(a => a.Question)?
                                                                  .Include(a => a.QuizForms)?
                                                                  .ThenInclude(a => a.Answers)?
                                                                  .Include(a => a.QuizForms)?
                                                                  .ThenInclude(a => a.ClipFK)?
                                                                  .ThenInclude(a => a.LessonFK)?
                                                                  .ThenInclude(a => a.UnitFK)?
                                                                  .FirstOrDefaultAsync(cancellationToken);


            if (quiz == null)
            {
                return new CommitResult<QuizDetailsResponse>
                {
                    ResultType = ResultType.NotFound,
                    ErrorCode = "XCUR0003",
                    ErrorMessage = _resourceJsonManager["XCUR0003"]
                };
            }

            cachedQuizDetailsResponse = new QuizDetailsResponse
            {
                Duration = quiz.QuizForms.Sum(a => a.DurationInSec),
                Id = request.QuizId,
                LessonId = quiz.LessonId,
                SubjectId = quiz.SubjectId,
                UnitId = quiz.UnitId,
                QuestionResponses = quiz.QuizForms.Randomize().Select(a => new QuizQuestionResponse
                {
                    Id = a.Question.Id,
                    Type = (int)a.Question.Type,
                    Value = a.Question.Type == FormType.Image ? $"https://www.selaheltelmeez.com/Media21-22/{quiz.SubjectId}/mcq/{a.Question.Value}" : a.Question.Value,
                    Hint = a.Hint,
                    ClipExplanatory = a.ClipFK?.Adapt<QuizClipResponse>(),
                    AnswerResponses = a.Answers.Select(quizAnswer => new QuizAnswerResponse
                    {
                        Id = quizAnswer.Id,
                        Type = (int)quizAnswer.Type,
                        Value = quizAnswer.Type == FormType.Image ? $"https://www.selaheltelmeez.com/Media21-22/{quiz.SubjectId}/mcq/{quizAnswer.Value}" : quizAnswer.Value,
                        IsCorrect = quizAnswer.IsCorrect
                    }).ToList()
                }).ToList()
            };

            await _cahce.SaveToCacheAsync(request.QuizId, cachedQuizDetailsResponse, "Curriculum-GetQuizDetails", cancellationToken);
        }


        return new CommitResult<QuizDetailsResponse>()
        {
            ResultType = ResultType.Ok,
            Value = cachedQuizDetailsResponse
        };
    }
}