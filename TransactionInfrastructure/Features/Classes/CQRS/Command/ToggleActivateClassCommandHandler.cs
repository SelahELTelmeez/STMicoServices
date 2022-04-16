using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.CQRS.Command;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;

namespace TransactionInfrastructure.Features.Classes.CQRS.Command;

public class ToggleActivateClassCommandHandler : IRequestHandler<ToggleActivateClassCommand, CommitResult>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;

    private readonly JsonLocalizerManager _resourceJsonManager;

    public ToggleActivateClassCommandHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor, JsonLocalizerManager jsonLocalizerManager)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = jsonLocalizerManager;
    }


    public async Task<CommitResult> Handle(ToggleActivateClassCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.TeacherClass? teacherClass = await _dbContext.Set<DomainEntities.TeacherClass>().SingleOrDefaultAsync(a => a.Id.Equals(request.ClassId) && a.TeacherId.Equals(_userId), cancellationToken);
        if (teacherClass == null)
        {
            return new CommitResult
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "X0005",
                ErrorMessage = _resourceJsonManager["X0005"]
            };
        }
        teacherClass.IsActive = !teacherClass.IsActive;
        _dbContext.Set<DomainEntities.TeacherClass>().Update(teacherClass);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}
