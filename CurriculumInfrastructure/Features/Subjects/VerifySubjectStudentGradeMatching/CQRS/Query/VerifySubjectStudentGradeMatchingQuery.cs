using CurriculumDomain.Features.Subjects.VerifySubjectStudentGradeMatching.CQRS.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Subjects;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace CurriculumInfrastructure.Features.Subjects.VerifySubjectStudentGradeMatching.CQRS.Query;

public class VerifySubjectStudentGradeMatchingQueryHandler : IRequestHandler<VerifySubjectStudentGradeMatchingQuery, CommitResult<bool>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public VerifySubjectStudentGradeMatchingQueryHandler(CurriculumDbContext dbContext,
                                                         IWebHostEnvironment configuration,
                                                         IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<CommitResult<bool>> Handle(VerifySubjectStudentGradeMatchingQuery request, CancellationToken cancellationToken)
    {
        Subject? subject = await _dbContext.Set<Subject>().SingleOrDefaultAsync(a => a.Id.Equals(request.SubjectId) && a.Grade.Equals(request.GradeId), cancellationToken);
        if (subject == null)
        {
            return new CommitResult<bool>
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "X0004",
                ErrorMessage = _resourceJsonManager["X0004"]
            };
        }
        return new CommitResult<bool>
        {
            ResultType = ResultType.Ok,
            Value = subject != null
        };
    }
}


