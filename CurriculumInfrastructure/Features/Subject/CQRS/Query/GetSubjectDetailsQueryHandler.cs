using CurriculumDomain.Features.Subject.CQRS.Query;
using CurriculumDomain.Features.Subject.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumInfrastructure.Utilities;
using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using ResultHandler;
using CurriculumEntites.Entities.Subjects;
using Mapster;
using Microsoft.EntityFrameworkCore;

namespace CurriculumInfrastructure.Features.Subject.CQRS.Query
{
    public class GetSubjectDetailsQueryHandler : IRequestHandler<GetSubjectDetailsQuery, CommitResults<SubjectDetailsResponse>>
    {
        private readonly CurriculumDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;

        public GetSubjectDetailsQueryHandler(CurriculumDbContext dbContext,
                                             IWebHostEnvironment configuration,
                                             IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }

        public async Task<CommitResults<SubjectDetailsResponse>> Handle(GetSubjectDetailsQuery request, CancellationToken cancellationToken)
        {
            CurriculumEntites.Entities.Subjects.Subject? curriculumDetails = await _dbContext.Set<CurriculumEntites.Entities.Subjects.Subject>()
                                                                                   .SingleOrDefaultAsync(a => a.Id.Equals(request.SubjectId), cancellationToken: cancellationToken);

            if (curriculumDetails == null)
            {
                return new CommitResults<SubjectDetailsResponse>
                {
                    ErrorCode = "X0016",
                    ErrorMessage = _resourceJsonManager["X0016"], //??? // Data of student Subject Details is not exist.
                    ResultType = ResultType.NotFound,
                };
            }
            return new CommitResults<SubjectDetailsResponse>
            {
                ResultType = ResultType.Ok,
                Value = (IEnumerable<SubjectDetailsResponse>)curriculumDetails.Adapt<SubjectDetailsResponse>(),
            };
        }
    }
}