using JsonLocalizer;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.TeacherClass.CQRS.Command;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;
namespace TransactionInfrastructure.Features.TeacherClass.CQRS.Command;

public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, CommitResult>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;

    private readonly JsonLocalizerManager _resourceJsonManager;

    public CreateClassCommandHandler(TrackerDbContext dbContext, IHttpContextAccessor httpContextAccessor, JsonLocalizerManager jsonLocalizerManager)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = jsonLocalizerManager;
    }

    public async Task<CommitResult> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        DomainEntities.TeacherClass? teacherClass = await _dbContext.Set<DomainEntities.TeacherClass>().SingleOrDefaultAsync(a => a.Name.Equals(request.CreateClassRequest.Name) && a.TeacherId.Equals(_userId), cancellationToken);
        if (teacherClass != null)
        {
            return new CommitResult
            {
                ResultType = ResultType.Duplicated,
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0000"]
            };
        }

        _dbContext.Set<DomainEntities.TeacherClass>().Add(new DomainEntities.TeacherClass
        {
            Description = request.CreateClassRequest.Description,
            Name = request.CreateClassRequest.Name,
            SubjectId = request.CreateClassRequest.SubjectId,
            TeacherId = _userId.GetValueOrDefault(),
            IsActive = true
        });

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}
