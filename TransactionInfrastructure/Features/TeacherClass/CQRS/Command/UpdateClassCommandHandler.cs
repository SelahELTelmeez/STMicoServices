using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.TeacherClass.CQRS.Command;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;

namespace TransactionInfrastructure.Features.TeacherClass.CQRS.Command;

public class UpdateClassCommandHandler : IRequestHandler<UpdateClassCommand, CommitResult>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public UpdateClassCommandHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor, JsonLocalizerManager jsonLocalizerManager)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = jsonLocalizerManager;
    }

    public async Task<CommitResult> Handle(UpdateClassCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.TeacherClass? teacherClass = await _dbContext.Set<DomainEntities.TeacherClass>().SingleOrDefaultAsync(a => a.Id.Equals(request.UpdateClassRequest.ClassId) && a.TeacherId.Equals(_userId), cancellationToken);
        if (teacherClass == null)
        {
            return new CommitResult
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "X0005",
                ErrorMessage = _resourceJsonManager["X0005"]
            };
        }
        teacherClass.Name = teacherClass.Name;
        teacherClass.SubjectId = teacherClass.SubjectId;
        teacherClass.Description = teacherClass.Description;
        _dbContext.Set<DomainEntities.TeacherClass>().Update(teacherClass);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new CommitResult
        {
            ResultType = ResultType.Ok
        };

    }
}
