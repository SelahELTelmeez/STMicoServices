using CurriculumDomain.Features.Quizzes.Quiz.CQRS.Command;
using CurriculumEntites.Entities;
using CurriculumInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurriculumInfrastructure.Features.Quizzes.Quiz.CQRS.Command
{
    public class SubmitQuizUserAnswersCommandHandler : IRequestHandler<UserQuizAnswerCommand, CommitResult<int>>
    {
        private readonly CurriculumDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SubmitQuizUserAnswersCommandHandler(CurriculumDbContext dbContext,
                                             IWebHostEnvironment configuration,
                                             IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<CommitResult<int>> Handle(UserQuizAnswerCommand request, CancellationToken cancellationToken)
        {
            Guid? IdentityUserId = _httpContextAccessor.GetIdentityUserId();

            //==================1-check is user enter quiz first in QuizeUserScore and if not insert new record =========================

            //==================2-submit user quiz answers==================================

            //==================3-edit on TotalDegree at DB table QuizeUserScore=========================



            //
            return new CommitResult<int>
            {
                ResultType = ResultType.Ok,
                // Value = 
            };
        }
    }
}
