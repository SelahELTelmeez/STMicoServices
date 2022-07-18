using CurriculumDomain.Features.Reports.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using CurriculumInfrastructure.HttpClients;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.DTO;

namespace CurriculumInfrastructure.Features.Reports.CQRS.Query
{
    public class GetSubjectsBriefProgressQueryHandler : IRequestHandler<GetSubjectsBriefProgressQuery, CommitResults<SubjectBriefProgressResponse>>
    {
        private readonly CurriculumDbContext _dbContext;
        private readonly IdentityClient _identityClient;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDistributedCache _cahce;

        public GetSubjectsBriefProgressQueryHandler(CurriculumDbContext dbContext, IHttpContextAccessor httpContextAccessor, IdentityClient identityClient, IDistributedCache cahce)
        {
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _identityClient = identityClient;
            _cahce = cahce;
        }
        public async Task<CommitResults<SubjectBriefProgressResponse>> Handle(GetSubjectsBriefProgressQuery request, CancellationToken cancellationToken)
        {
            CommitResult<int>? identityGrade = await _identityClient.GetStudentGradesAsync(request.StudentId ?? _httpContextAccessor.GetIdentityUserId(), cancellationToken);

            if (!identityGrade.IsSuccess)
            {
                return new CommitResults<SubjectBriefProgressResponse>
                {
                    ErrorCode = identityGrade.ErrorCode,
                    ErrorMessage = identityGrade.ErrorMessage,
                    ResultType = identityGrade.ResultType
                };
            }

            IEnumerable<SubjectBriefProgressResponse>? cachedSubjectBriefProgressResponses = await _cahce.GetFromCacheAsync<string, IEnumerable<SubjectBriefProgressResponse>>($"{identityGrade.Value}-{request.Term}", "Curriculum-GetSubjectsBriefProgress", cancellationToken);

            if (cachedSubjectBriefProgressResponses == null)
            {
                IEnumerable<Subject>? subjects = await _dbContext.Set<Subject>()
                                   .Where(a => a.Grade == identityGrade.Value && a.Term == request.Term && a.IsAppShow == true)
                                   .Include(a => a.Units)
                                   .ThenInclude(a => a.Lessons)
                                   .ThenInclude(a => a.Clips)
                                   .ToListAsync(cancellationToken);

                IEnumerable<SubjectBriefProgressResponse> Mapper()
                {
                    foreach (Subject subject in subjects)
                    {
                        yield return new SubjectBriefProgressResponse
                        {
                            SubjectId = subject.Id,
                            SubjectName = subject.ShortName,
                            TotalSubjectScore = subject.Units.SelectMany(a => a.Lessons).SelectMany(a => a.Clips).Sum(a => a.Points) ?? 0
                        };
                    }
                }

                cachedSubjectBriefProgressResponses = Mapper();

                await _cahce.SaveToCacheAsync<string, IEnumerable<SubjectBriefProgressResponse>>($"{identityGrade.Value}-{request.Term}", cachedSubjectBriefProgressResponses, "Curriculum-GetSubjectsBriefProgress", cancellationToken);
            }

            return new CommitResults<SubjectBriefProgressResponse>
            {
                ResultType = ResultType.Ok,
                Value = cachedSubjectBriefProgressResponses
            };
        }
    }

}
