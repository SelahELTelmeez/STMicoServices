using CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;
using CurriculumEntites.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedModule.DTO;
using DomainEntitiesSubjects = CurriculumEntites.Entities.Subjects;
namespace CurriculumInfrastructure.Features.Subjects.GetSubjects.CQRS.Query;
public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, CommitResults<SubjectResponse>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly IDistributedCache _cache;

    public GetSubjectsQueryHandler(CurriculumDbContext dbContext, IDistributedCache cache)
    {
        _dbContext = dbContext;
        _cache = cache;
    }

    public async Task<CommitResults<SubjectResponse>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<SubjectResponse>? cachedSubjectResponse = await _cache.GetFromCacheAsync<string, IEnumerable<SubjectResponse>>(string.Join(',', request.SubjectIds), "Curriculum-GetSubjects", cancellationToken);

        if (cachedSubjectResponse == null)
        {
            IEnumerable<DomainEntitiesSubjects.Subject> subjects = await _dbContext.Set<DomainEntitiesSubjects.Subject>()
                             .Where(a => request.SubjectIds.Contains(a.Id))
                             .ToListAsync(cancellationToken);

            if (subjects.Any())
            {
                IEnumerable<SubjectResponse> Mapper()
                {
                    foreach (DomainEntitiesSubjects.Subject subject in subjects)
                    {
                        yield return new SubjectResponse
                        {
                            FullyQualifiedName = subject.FullyQualifiedName,
                            Grade = subject.Grade,
                            Id = subject.Id,
                            IsAppShow = subject.IsAppShow,
                            RewardPoints = subject.RewardPoints,
                            ShortName = subject.ShortName,
                            TeacherGuide = subject.TeacherGuide,
                            Term = subject.Term,
                            Title = subject.Title,
                            PrimaryIcon = $"http://www.almoallem.com/media/LMSAPP/TeacherSubjectIcon/{subject.Id[..6]}.png",
                            InternalIcon = $"http://www.almoallem.com/media/LMSAPP/SubjectIcon/Icon/teacher/{subject.Title}.png",
                        };
                    }
                    yield break;
                }
                cachedSubjectResponse = Mapper();

                await _cache.SaveToCacheAsync<string, IEnumerable<SubjectResponse>>(string.Join(',', request.SubjectIds), cachedSubjectResponse, "Curriculum-GetSubjects", cancellationToken);
            }

            return new CommitResults<SubjectResponse>
            {
                ResultType = ResultType.Ok,
                Value = cachedSubjectResponse
            };
        }
        else
        {
            return new CommitResults<SubjectResponse>()
            {
                ResultType = ResultType.Empty,
                Value = Array.Empty<SubjectResponse>()
            };
        }
    }
}