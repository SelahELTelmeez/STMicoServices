using CurriculumDomain.Features.Subjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.DTO;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using Microsoft.EntityFrameworkCore;

namespace CurriculumInfrastructure.Features.Subjects.CQRS.Query
{
    public class GetSubjectsByFiltersQueryHandler : IRequestHandler<GetSubjectsByFiltersQuery, CommitResults<SubjectProfileResponse>>
    {
        private readonly CurriculumDbContext _dbContext;
        public GetSubjectsByFiltersQueryHandler(CurriculumDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<CommitResults<SubjectProfileResponse>> Handle(GetSubjectsByFiltersQuery request, CancellationToken cancellationToken)
        {
            return new CommitResults<SubjectProfileResponse>
            {
                ResultType = ResultType.Ok,
                Value = await _dbContext.Set<Subject>()
                                        .Where(a => a.IsAppShow == true)
                                        .Where(a => a.Grade == request.Grade && a.Term == request.TermId)
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
