using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using TransactionDomain.Features.Assignment.CQRS.Command;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Shared;
using TransactionEntites.Entities.TeacherActivity;
using TransactionEntites.Entities.TeacherClasses;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Assignment.CQRS.Command;

public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, CommitResult>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    public CreateAssignmentCommandHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
    }

    public async Task<CommitResult> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {

        IEnumerable<TeacherClass> teacherClasses = await _dbContext.Set<TeacherClass>()
                                                                   .Where(a => request.CreateAssignmentRequest.Classes.Contains(a.Id))
                                                                   .Include(a => a.StudentEnrolls)
                                                                   .ToListAsync(cancellationToken);

        EntityEntry<TeacherAssignment> teacherAssignment = _dbContext.Set<TeacherAssignment>().Add(new TeacherAssignment
        {
            AttachmentUrl = request.CreateAssignmentRequest.AttachmentUrl,
            Creator = _userId.GetValueOrDefault(),
            Description = request.CreateAssignmentRequest.Description,
            StartDate = request.CreateAssignmentRequest.StartDate,
            EndDate = request.CreateAssignmentRequest.EndDate,
            Title = request.CreateAssignmentRequest.Title,
            TeacherClasses = teacherClasses.ToList()
        });

        foreach (StudentEnrollClass studentEnrolled in teacherClasses.SelectMany(a => a.StudentEnrolls))
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

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}
