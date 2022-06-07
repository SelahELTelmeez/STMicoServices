using CurriculumDomain.Features.Quizzes.CQRS.Query;
using CurriculumDomain.Features.Quizzes.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace CurriculumInfrastructure.Features.Quizzes.CQRS.Query
{
    public class GetStudentAttemptsQueryHandler : IRequestHandler<GetStudentAttemptsQuery, CommitResult<QuizAttemptResponse>>
    {
        private readonly CurriculumDbContext _dbContext;
        public GetStudentAttemptsQueryHandler(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<CommitResult<QuizAttemptResponse>> Handle(GetStudentAttemptsQuery request, CancellationToken cancellationToken)
        {
            CurriculumEntites.Entities.Quizzes.Quiz? quiz = await _dbContext.Set<CurriculumEntites.Entities.Quizzes.Quiz>()
                                                                            .Where(a => a.Id == request.QuizId)
                                                                            .Include(a => a.QuizForms)
                                                                            .ThenInclude(a => a.Question)
                                                                            .Include(a => a.QuizForms)
                                                                            .ThenInclude(a => a.Answers)
                                                                            .Include(a => a.QuizForms)
                                                                            .ThenInclude(a => a.Attempts)
                                                                            .ThenInclude(a => a.UserAnswer)
                                                                            .SingleOrDefaultAsync(cancellationToken);

            if (quiz == null)
            {
                return new CommitResult<QuizAttemptResponse>
                {
                    ResultType = ResultType.NotFound,
                    ErrorCode = "XXXX",
                    ErrorMessage = "XXXX"
                };
            }


            return new CommitResult<QuizAttemptResponse>()
            {
                ResultType = ResultType.Ok,
                Value = new QuizAttemptResponse
                {
                    Duration = quiz.QuizForms.Sum(a => a.DurationInSec),
                    Id = request.QuizId,
                    LessonId = quiz.LessonId,
                    SubjectId = quiz.SubjectId,
                    UnitId = quiz.UnitId,
                    QuizFormsAttempts = quiz.QuizForms.Select(a => new QuizFormAttemptResponse
                    {
                        Id = a.Question.Id,
                        Type = (int)a.Question.Type,
                        Value = a.Question.Type == FormType.Image ? $"https://www.selaheltelmeez.com/Media21-22/{quiz.SubjectId}/mcq/{a.Question.Value}" : a.Question.Value,
                        Hint = a.Hint,
                        AttemptAnswerId = a.Attempts.Where(a => a.StudentUserId == request.StudentId).OrderByDescending(a => a.CreatedOn).FirstOrDefault()?.UserAnswerId,
                        AnswerResponses = a.Answers.Select(quizAnswer => new QuizAnswerResponse
                        {
                            Id = quizAnswer.Id,
                            Type = (int)quizAnswer.Type,
                            Value = quizAnswer.Type == FormType.Image ? $"https://www.selaheltelmeez.com/Media21-22/{quiz.SubjectId}/mcq/{quizAnswer.Value}" : quizAnswer.Value,
                            IsCorrect = quizAnswer.IsCorrect,
                        }).ToList()
                    }).ToList()

                }
            };
        }
    }
}
