using TeacherDomain.Features.Assignment.CQRS.Query;
using TeacherDomain.Features.Assignment.DTO.Query;
using TeacherEntities.Entities.TeacherActivity;
using TeacherEntities.Entities.TeacherClasses;
using TeacherEntities.Entities.Trackers;

namespace TeacherInfrastructure.Features.Assignment.CQRS.Query;

public class GetAssignmentByIdQueryHandler : IRequestHandler<GetAssignmentByIdQuery, ICommitResult<AssignmentResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly string? _userId;
    public GetAssignmentByIdQueryHandler(TeacherDbContext dbContext,
                                         IHttpContextAccessor httpContextAccessor,
                                         IWebHostEnvironment configuration)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<ICommitResult<AssignmentResponse>> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        TeacherAssignment? teacherAssignment = await _dbContext.Set<TeacherAssignment>()
                                                               .Where(a => a.Id.Equals(request.Id))
                                                               .FirstOrDefaultAsync(cancellationToken);
        if (teacherAssignment == null)
        {
            return ResultType.NotFound.GetValueCommitResult<AssignmentResponse>(default, "XTEC0009", _resourceJsonManager["XTEC0009"]);
        }

        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>()
                                                     .FirstOrDefaultAsync(a => a.Id == request.ClassId, cancellationToken);

        TeacherAssignmentActivityTracker? teacherAssignmentActivityTracker = await _dbContext.Set<TeacherAssignmentActivityTracker>()
                                                                                             .FirstOrDefaultAsync(a => a.StudentId == _userId &&
                                                                                                                       a.ClassId == request.ClassId &&
                                                                                                                       a.TeacherAssignmentId == teacherAssignment.Id, cancellationToken);

        return ResultType.Ok.GetValueCommitResult(new AssignmentResponse
        {
            Description = teacherAssignment.Description,
            CreatedOn = teacherAssignment.CreatedOn.GetValueOrDefault(),
            EndDate = teacherAssignment.EndDate,
            Id = teacherAssignment.Id,
            Title = teacherAssignment.Title,
            LessonName = teacherAssignment.LessonName,
            SubjectName = teacherAssignment.SubjectName,
            SubjectId = teacherClass?.SubjectId,
            AttachmentId = teacherAssignment.AttachmentId,
            ClassName = teacherClass?.Name,
            EnrolledCounter = 0,
            TeacherAssignmentActivityTrackerId = teacherAssignmentActivityTracker?.Id
        });
    }
}
