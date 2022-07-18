using CurriculumDomain.Features.Subjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.DTO;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using CurriculumInfrastructure.HttpClients;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.DTO;

namespace CurriculumInfrastructure.Features.Subjects.CQRS.Query
{
    public class GetSubjectsByFiltersQueryHandler : IRequestHandler<GetSubjectsByFiltersQuery, CommitResults<SubjectProfileResponse>>
    {
        private readonly CurriculumDbContext _dbContext;
        private readonly IdentityClient _IdentityClient;
        private readonly JsonLocalizerManager _resourceJsonManager;
        private readonly IDistributedCache _cahce;

        public GetSubjectsByFiltersQueryHandler(CurriculumDbContext dbContext,
                                                IdentityClient identityClient,
                                                IWebHostEnvironment configuration,
                                                IHttpContextAccessor httpContextAccessor,
                                                IDistributedCache cahce)
        {
            _dbContext = dbContext;
            _IdentityClient = identityClient;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
            _cahce = cahce;
        }

        public async Task<CommitResults<SubjectProfileResponse>> Handle(GetSubjectsByFiltersQuery request, CancellationToken cancellationToken)
        {

            IEnumerable<SubjectProfileResponse>? cachedSubjectProfileResponses = await _cahce.GetFromCacheAsync<string, IEnumerable<SubjectProfileResponse>>($"{request.Grade}-{request.TermId}", "Curriculum-GetSubjectsByFilters", cancellationToken);

            if (cachedSubjectProfileResponses == null)
            {
                IEnumerable<Subject> subjects = await _dbContext.Set<Subject>()
                                                          .Where(a => a.IsAppShow == true)
                                                          .Where(a => a.Grade == request.Grade && a.Term == request.TermId)
                                                          .ToListAsync(cancellationToken);

                if (!subjects.Any())
                {
                    return new CommitResults<SubjectProfileResponse>()
                    {
                        ResultType = ResultType.Empty,
                        ErrorCode = "XCUR0006",
                        ErrorMessage = _resourceJsonManager["XCUR0006"]
                    };
                }

                CommitResults<GradeResponse>? grades = await _IdentityClient.GetGradesDetailesAsync(subjects.Select(a => a.Grade), cancellationToken);

                if (!grades.IsSuccess)
                {
                    return new CommitResults<SubjectProfileResponse>
                    {
                        ErrorCode = grades.ErrorCode,
                        ErrorMessage = grades.ErrorMessage,
                        ResultType = grades.ResultType
                    };
                }

                IEnumerable<SubjectProfileResponse> Mapper()
                {
                    foreach (Subject subject in subjects)
                    {
                        GradeResponse? gradeResponse = grades.Value.FirstOrDefault(a => a.Id == subject.Grade);
                        yield return new SubjectProfileResponse
                        {
                            Id = subject.Id,
                            Icon = $"http://www.almoallem.com/media/LMSAPP/SubjectIcon/Icon/teacher/{subject.Title}.png",
                            Name = subject.ShortName,
                            GradeShortName = gradeResponse?.ShortName,
                        };
                    }
                    yield break;
                }

                cachedSubjectProfileResponses = Mapper();

                await _cahce.SaveToCacheAsync($"{request.Grade}-{request.TermId}", cachedSubjectProfileResponses, "Curriculum-GetSubjectsByFilters", cancellationToken);
            }

            return new CommitResults<SubjectProfileResponse>
            {
                ResultType = ResultType.Ok,
                Value = cachedSubjectProfileResponses
            };
        }
    }
}
