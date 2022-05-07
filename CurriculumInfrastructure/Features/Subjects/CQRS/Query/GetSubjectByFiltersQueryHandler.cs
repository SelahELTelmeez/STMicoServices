using CurriculumDomain.Features.Subjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.DTO;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using Microsoft.EntityFrameworkCore;

namespace CurriculumInfrastructure.Features.Subjects.CQRS.Query
{
    public class GetSubjectByFiltersQueryHandler : IRequestHandler<GetSubjectByFiltersQuery, CommitResults<SubjectProfileResponse>>
    {
        private readonly CurriculumDbContext _dbContext;
        public GetSubjectByFiltersQueryHandler(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommitResults<SubjectProfileResponse>> Handle(GetSubjectByFiltersQuery request, CancellationToken cancellationToken)
        {
            return new CommitResults<SubjectProfileResponse>
            {
                ResultType = ResultType.Ok,
                Value = await _dbContext.Set<Subject>()
                                        .Where(a => a.Grade == request.Grade && a.Term == request.TermId && a.IsAppShow.GetValueOrDefault() == true)
                                        .Select(a => new SubjectProfileResponse
                                        {
                                            Id = a.Id,
                                            Name = a.ShortName,
                                            Icon = $"http://www.almoallem.com/media/LMSAPP/SubjectIcon/Icon/teacher/{a.Title}.png"
                                        }).ToListAsync(cancellationToken)
            };
        }
    }
}
