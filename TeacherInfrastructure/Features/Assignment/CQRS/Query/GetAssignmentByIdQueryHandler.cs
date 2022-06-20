using System;
using TeacherDomain.Features.Assignment.CQRS.Query;
using TeacherDomain.Features.Assignment.DTO.Query;
using TeacherEntities.Entities.TeacherActivity;

namespace TeacherInfrastructure.Features.Assignment.CQRS.Query
{
    public class GetAssignmentByIdQueryHandler : IRequestHandler<GetAssignmentByIdQuery, ICommitResult<AssignmentResponse>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly JsonLocalizerManager _resourceJsonManager;
        public GetAssignmentByIdQueryHandler(TeacherDbContext dbContext,
                                             IHttpContextAccessor httpContextAccessor,
                                             IWebHostEnvironment configuration)
        {
            _dbContext = dbContext;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }

        public async Task<ICommitResult<AssignmentResponse>> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
        {
            TeacherAssignment? teacherAssignment = await _dbContext.Set<TeacherAssignment>()
                                                                    .Where(a => a.Id.Equals(request.Id))
                                                                    .SingleOrDefaultAsync(cancellationToken);
            if (teacherAssignment == null)
            {
                return ResultType.NotFound.GetValueCommitResult<AssignmentResponse>(default, "X0017", _resourceJsonManager["X0017"]);
            }



            return ResultType.Ok.GetValueCommitResult(new AssignmentResponse
            {
                Description = teacherAssignment.Description,
                CreatedOn = teacherAssignment.CreatedOn.GetValueOrDefault(),
                EndDate = teacherAssignment.EndDate,
                Id = teacherAssignment.Id,
                Title = teacherAssignment.Title,
                EnrolledCounter = 0,
            });
        }
    }
}
