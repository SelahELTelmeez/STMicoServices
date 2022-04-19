using JsonLocalizer;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using TeacherDomain.Features.Classes.CQRS.Command;
using DomainTeacherEntities = TeacherEntities.Entities.TeacherSubjects;
using TeacherEntities.Entities.TeacherClasses;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
public class CreateClassCommandHandler : IRequestHandler<CreateClassCommand, CommitResult<int>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly Guid? _teacherId;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public CreateClassCommandHandler(TeacherDbContext dbContext,
                                     IWebHostEnvironment configuration,
                                     IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _teacherId = httpContextAccessor.GetIdentityUserId();
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<CommitResult<int>> Handle(CreateClassCommand request, CancellationToken cancellationToken)
    {

        DomainTeacherEntities.TeacherSubject? teacherSubject = await _dbContext.Set<DomainTeacherEntities.TeacherSubject>().SingleOrDefaultAsync(a => a.TeacherId.Equals(_teacherId) && a.SubjectId.Equals(request.CreateClassRequest.SubjectId),cancellationToken);

        if(teacherSubject == null)
        {
            return new CommitResult<int>
            {
                ResultType = ResultType.Invalid,
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0000"]
            };
        }
        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>().SingleOrDefaultAsync(a => a.Name.Equals(request.CreateClassRequest.Name) && a.TeacherId.Equals(_teacherId), cancellationToken);
       
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
            TeacherId = _teacherId.GetValueOrDefault(),
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
