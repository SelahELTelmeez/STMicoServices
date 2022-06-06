using Microsoft.EntityFrameworkCore.ChangeTracking;
using TeacherDomain.Features.Assignment.CQRS.Command;
using TeacherEntites.Entities.Shared;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.TeacherActivity;
using TeacherEntities.Entities.TeacherClasses;
using TeacherEntities.Entities.Trackers;

namespace TeacherInfrastructure.Features.Assignment.CQRS.Command;

public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, ICommitResult>
{
    private readonly TeacherDbContext _dbContext;
    private readonly Guid? _userId;
    public CreateAssignmentCommandHandler(TeacherDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<ICommitResult> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {

        IEnumerable<TeacherClass> teacherClasses = await _dbContext.Set<TeacherClass>()
                                                                   .Where(a => request.CreateAssignmentRequest.Classes.Contains(a.Id))
                                                                   .Include(a => a.ClassEnrollees)
                                                                   .ToListAsync(cancellationToken);

        EntityEntry<TeacherAssignment> teacherAssignment = _dbContext.Set<TeacherAssignment>().Add(new TeacherAssignment
        {
            AttachmentId = request.CreateAssignmentRequest.AttachmentId,
            Creator = _userId.GetValueOrDefault(),
            Description = request.CreateAssignmentRequest.Description,
            EndDate = request.CreateAssignmentRequest.EndDate,
            Title = request.CreateAssignmentRequest.Title,
            TeacherClasses = teacherClasses.ToList()
        });

        foreach (ClassEnrollee studentEnrolled in teacherClasses.SelectMany(a => a.ClassEnrollees))
        {
            _dbContext.Set<TeacherAssignmentActivityTracker>().Add(new TeacherAssignmentActivityTracker
            {
                ClassId = studentEnrolled.ClassId,
                ActivityStatus = ActivityStatus.New,
                StudentId = studentEnrolled.StudentId,
                TeacherAssignmentId = teacherAssignment.Entity.Id
            });
        }

        await _dbContext.SaveChangesAsync(cancellationToken);

        return ResultType.Ok.GetCommitResult();
    }
}
