﻿using CurriculumDomain.Features.Quizzes.CQRS.Command;
using CurriculumDomain.Features.Quizzes.DTO.Command;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Quizzes;
using CurriculumInfrastructure.HttpClients;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DomainEntities = CurriculumEntites.Entities.Quizzes;
namespace CurriculumInfrastructure.Features.Quizzes.Quiz.CQRS.Command
{
    public class SubmitQuizAnswerCommandHandler : IRequestHandler<SubmitQuizAnswerCommand, CommitResult>
    {
        private readonly CurriculumDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        private readonly StudentClient _TrackerClient;
        private readonly Guid? _userId;

        public SubmitQuizAnswerCommandHandler(CurriculumDbContext dbContext,
                                             IWebHostEnvironment configuration,
                                             IHttpContextAccessor httpContextAccessor,
                                             StudentClient trackerClient)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
            _userId = httpContextAccessor.GetIdentityUserId();
            _TrackerClient = trackerClient;
        }

        public async Task<CommitResult> Handle(SubmitQuizAnswerCommand request, CancellationToken cancellationToken)
        {
            // ================= 1. Insert all user's quiz attempts ===========================
            DomainEntities.Quiz? quiz = await _dbContext.Set<DomainEntities.Quiz>()
                                                       .Where(a => a.Id.Equals(request.UserQuizAnswersRequest.QuizId))
                                                       .Include(a => a.QuizForms)
                                                       .ThenInclude(a => a.Answers)
                                                       .SingleOrDefaultAsync(cancellationToken);

            foreach (UserQuizAnswerRequest questionAnswerRequest in request.UserQuizAnswersRequest.QuizAnswerRequests)
            {
                try
                {
                    var answer = new QuizAttempt
                    {
                        StudentUserId = _userId.GetValueOrDefault(),
                        IsCorrect = quiz.QuizForms.SelectMany(a => a.Answers).Any(a => a.Id.Equals(questionAnswerRequest.AnswerId) && a.IsCorrect),
                        QuizForm = quiz.QuizForms.SingleOrDefault(a => a.QuizId.Equals(request.UserQuizAnswersRequest.QuizId) && a.QuestionId.Equals(questionAnswerRequest.QuestionId)),
                        UserAnswerId = questionAnswerRequest.AnswerId,
                    };
                    _dbContext.Set<QuizAttempt>().Add(answer);
                }
                catch (Exception ex)
                {
                    throw ex;
                }

            }


            try
            {

                int StudentCorrectAnswersScore = _dbContext.ChangeTracker.Entries<QuizAttempt>().Where(a => a.State == EntityState.Added && a.Entity.IsCorrect).Count();

                await _dbContext.SaveChangesAsync(cancellationToken);

                CommitResult commitResult = await _TrackerClient.SubmitStudentQuizAnswerAsync(new UpdateStudentQuizRequest
                {
                    StudentUserScore = StudentCorrectAnswersScore,
                    TimeSpentInSec = request.UserQuizAnswersRequest.TimeSpent,
                    TotalQuizScore = request.UserQuizAnswersRequest.QuizAnswerRequests.Count,
                    QuizId = request.UserQuizAnswersRequest.QuizId,
                }, cancellationToken);

                return new CommitResult
                {
                    ResultType = ResultType.Ok,
                };
            }
            catch (Exception Ex)
            {

                throw;
            }


            //================== 2- insert the user quiz score in the tracker =========================


        }
    }
}