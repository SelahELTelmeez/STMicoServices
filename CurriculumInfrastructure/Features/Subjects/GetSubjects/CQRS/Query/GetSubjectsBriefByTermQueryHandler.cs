using CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using CurriculumInfrastructure.HttpClients;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;

namespace CurriculumInfrastructure.Features.Subjects.GetSubjects.CQRS.Query
{
    public class GetSubjectsBriefByTermQueryHandler : IRequestHandler<GetSubjectsBriefByTermQuery, CommitResults<SubjectBriefResponse>>
    {
        private readonly CurriculumDbContext _dbContext;
        private readonly IdentityClient _IdentityClient;
        private readonly JsonLocalizerManager _resourceJsonManager;

        public GetSubjectsBriefByTermQueryHandler(CurriculumDbContext dbContext,
                                                IdentityClient identityClient,
                                                IWebHostEnvironment configuration,
                                                IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _IdentityClient = identityClient;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }
        public async Task<CommitResults<SubjectBriefResponse>> Handle(GetSubjectsBriefByTermQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<Subject> subjects = await _dbContext.Set<Subject>()
                                                           .Where(a => a.IsAppShow == true)
                                                           .Where(a => a.Grade == request.Grade && a.Term == request.TermId)
                                                           .ToListAsync(cancellationToken);

            if (!subjects.Any())
            {
                return new CommitResults<SubjectBriefResponse>()
                {
                    ResultType = ResultType.Empty,
                    ErrorCode = "X0006",
                    ErrorMessage = _resourceJsonManager["X0006"]
                };
            }

            CommitResults<GradeResponse>? grades = await _IdentityClient.GetGradesDetailesAsync(subjects.Select(a => a.Grade), cancellationToken);

            if (!grades.IsSuccess)
            {
                return new CommitResults<SubjectBriefResponse>
                {
                    ErrorCode = grades.ErrorCode,
                    ErrorMessage = grades.ErrorMessage,
                    ResultType = grades.ResultType
                };
            }

            IEnumerable<SubjectBriefResponse> Mapper()
            {
                foreach (Subject subject in subjects)
                {
                    GradeResponse? gradeResponse = grades.Value.SingleOrDefault(a => a.Id == subject.Grade);
                    yield return new SubjectBriefResponse
                    {
                        Id = subject.Id,
                        InternalIcon = $"http://www.almoallem.com/media/LMSAPP/SubjectIcon/Icon/teacher/{subject.Title}.png",
                        Name = subject.ShortName,
                        RewardPoints = subject.RewardPoints
                    };
                }
                yield break;
            }

            return new CommitResults<SubjectBriefResponse>
            {
                ResultType = ResultType.Ok,
                Value = Mapper()
            };
        }
    }
}
