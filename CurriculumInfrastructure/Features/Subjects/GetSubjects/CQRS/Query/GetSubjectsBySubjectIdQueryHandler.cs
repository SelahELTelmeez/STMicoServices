using CurriculumDomain.Features.Subjects.GetSubjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetSubjects.DTO.Query;
using CurriculumEntites.Entities;
using Microsoft.EntityFrameworkCore;
using DomainEntitiesSubjects = CurriculumEntites.Entities.Subjects;

namespace CurriculumInfrastructure.Features.Subjects.GetSubjects.CQRS.Query;
public class GetSubjectsBySubjectIdQueryHandler : IRequestHandler<GetSubjectsBySubjectIdQuery, CommitResults<SubjectResponse>>
{
    private readonly CurriculumDbContext _dbContext;

    public GetSubjectsBySubjectIdQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<CommitResults<SubjectResponse>> Handle(GetSubjectsBySubjectIdQuery request, CancellationToken cancellationToken)
        => new CommitResults<SubjectResponse>
        {
            ResultType = ResultType.Ok,
            Value = await _dbContext.Set<DomainEntitiesSubjects.Subject>()
                               .Where(a => request.SubjectIds.Equals(a.Id))
                               .Select(a => new SubjectResponse
                               {
                                   Id = a.Id,
                                   Grade = a.Grade,
                                   FullyQualifiedName = a.FullyQualifiedName,
                                   IsAppShow = a.IsAppShow,
                                   RewardPoints = a.RewardPoints,
                                   ShortName = a.ShortName,
                                   TeacherGuide = a.TeacherGuide,
                                   Term = a.Term,
                                   Title = a.Title
                               })
                               .ToListAsync(cancellationToken)
        };
}