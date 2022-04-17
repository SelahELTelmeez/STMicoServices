using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.CQRS.Command;
using TransactionEntites.Entities;
using TransactionEntites.Entities.TeacherClasses;
using TransactionInfrastructure.Utilities;
namespace TransactionInfrastructure.Features.Classes.CQRS.Command;

public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, CommitResult<int>>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;

    private readonly JsonLocalizerManager _resourceJsonManager;

    public CreateClassCommandHandler(TrackerDbContext dbContext,
                                     IWebHostEnvironment configuration,
                                     IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<CommitResult<int>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {
        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>().SingleOrDefaultAsync(a => a.Name.Equals(request.CreateClassRequest.Name) && a.TeacherId.Equals(_userId), cancellationToken);
        if (teacherClass != null)
        {
            return new CommitResult<int>
            {
                ResultType = ResultType.Duplicated,
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0000"]
            };
        }

        teacherClass = new TeacherClass
        {
            Description = request.CreateClassRequest.Description,
            Name = request.CreateClassRequest.Name,
            SubjectId = request.CreateClassRequest.SubjectId,
            TeacherId = _userId.GetValueOrDefault(),
            IsActive = true
        };

        _dbContext.Set<TeacherClass>().Add(teacherClass);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return new CommitResult<int>
        {
            ResultType = ResultType.Ok,
            Value = teacherClass.Id
        };
    }
}
