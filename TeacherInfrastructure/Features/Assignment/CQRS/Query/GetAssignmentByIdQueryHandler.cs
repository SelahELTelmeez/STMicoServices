using TeacherDomain.Features.Assignment.CQRS.Query;
using TeacherDomain.Features.Assignment.DTO.Query;
using TeacherEntities.Entities.TeacherActivity;
using TeacherEntities.Entities.TeacherClasses;

namespace TeacherInfrastructure.Features.Assignment.CQRS.Query;

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
            return ResultType.NotFound.GetValueCommitResult<AssignmentResponse>(default, "XTEC0009", _resourceJsonManager["XTEC0009"]);
        }

        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>()
                                                     .SingleOrDefaultAsync(a => a.Id == request.ClassId, cancellationToken);

        return ResultType.Ok.GetValueCommitResult(new AssignmentResponse
        {
            Description = teacherAssignment.Description,
            CreatedOn = teacherAssignment.CreatedOn.GetValueOrDefault(),
            EndDate = teacherAssignment.EndDate,
            Id = teacherAssignment.Id,
            Title = teacherAssignment.Title,
            LessonName = teacherAssignment.LessonName,
            SubjectName = teacherAssignment.SubjectName,
            ClassName = teacherClass?.Name,
            EnrolledCounter = 0,
        });
    }
}
