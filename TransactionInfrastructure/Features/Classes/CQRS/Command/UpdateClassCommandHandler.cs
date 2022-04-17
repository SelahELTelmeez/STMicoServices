using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.CQRS.Command;
using TransactionEntites.Entities;
using TransactionInfrastructure.Utilities;
using DomainEntities = TransactionEntites.Entities.TeacherClasses;

namespace TransactionInfrastructure.Features.Classes.CQRS.Command;

public class UpdateClassCommandHandler : IRequestHandler<UpdateClassCommand, CommitResult>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public UpdateClassCommandHandler(TrackerDbContext dbContext, IWebHostEnvironment configuration,
                                         IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
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
        teacherClass.Name = request.UpdateClassRequest.Name;
        teacherClass.SubjectId = request.UpdateClassRequest.SubjectId;
        teacherClass.Description = request.UpdateClassRequest.Description;
        teacherClass.IsActive = request.UpdateClassRequest.IsActive;
        _dbContext.Set<DomainEntities.TeacherClass>().Update(teacherClass);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return new CommitResult
        {
            ResultType = ResultType.Ok
        };

    }
}
