using CurriculumDomain.Features.Subjects.CQRS.Query;
using CurriculumDomain.Features.Subjects.DTO;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using CurriculumInfrastructure.HttpClients;
using Microsoft.EntityFrameworkCore;
using SharedModule.DTO;

namespace CurriculumInfrastructure.Features.Subjects.CQRS.Query
{
    public class GetSubjectsByFiltersQueryHandler : IRequestHandler<GetSubjectsByFiltersQuery, CommitResults<SubjectProfileResponse>>
    {
        private readonly CurriculumDbContext _dbContext;
        private readonly IdentityClient _IdentityClient;

        public GetSubjectsByFiltersQueryHandler(CurriculumDbContext dbContext, IdentityClient identityClient)
        {
            _dbContext = dbContext;
            _IdentityClient = identityClient;
        }

        public async Task<CommitResults<SubjectProfileResponse>> Handle(GetSubjectsByFiltersQuery request, CancellationToken cancellationToken)
        {

            IEnumerable<Subject> subjects = await _dbContext.Set<Subject>()
                                                            .Where(a => a.IsAppShow == true)
                                                            .Where(a => a.Grade == request.Grade && a.Term == request.TermId)
                                                            .ToListAsync(cancellationToken);



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
                    GradeResponse? gradeResponse = grades.Value.SingleOrDefault(a => a.Id == subject.Grade);
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

            return new CommitResults<SubjectProfileResponse>
            {
                ResultType = ResultType.Ok,
                Value = Mapper()
            };
        }
    }
}
