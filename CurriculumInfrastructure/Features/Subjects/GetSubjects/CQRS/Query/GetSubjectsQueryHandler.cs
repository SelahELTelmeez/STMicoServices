using CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetSubjects.DTO.Query;
using CurriculumEntites.Entities;
using Microsoft.EntityFrameworkCore;
using DomainEntitiesSubjects = CurriculumEntites.Entities.Subjects;
namespace CurriculumInfrastructure.Features.Subjects.GetSubjects.CQRS.Query;
public class GetSubjectsQueryHandler : IRequestHandler<GetSubjectsQuery, CommitResults<SubjectResponse>>
{
    private readonly CurriculumDbContext _dbContext;

    public GetSubjectsQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommitResults<SubjectResponse>> Handle(GetSubjectsQuery request, CancellationToken cancellationToken)
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

            return new CommitResults<SubjectResponse>
            {
                ResultType = ResultType.Ok,
                Value = Mapper()
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