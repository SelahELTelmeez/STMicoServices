using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherDomain.Features.Shared.DTO;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
public class RequestEnrollToClassCommandHandler : IRequestHandler<RequestEnrollToClassCommand, CommitResult>
{
    private readonly NotifierClient _notifierClient;
    private readonly TeacherDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly Guid? _studentId;


    public RequestEnrollToClassCommandHandler(
            TeacherDbContext dbContext,
            NotifierClient notifierClient,
            IWebHostEnvironment configuration,
            IHttpContextAccessor httpContextAccessor)
    {
        _notifierClient = notifierClient;
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _studentId = httpContextAccessor.GetIdentityUserId();

    }
    public async Task<CommitResult> Handle(RequestEnrollToClassCommand request, CancellationToken cancellationToken)
    {

        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>().SingleOrDefaultAsync(a => a.Id.Equals(request.ClassId), cancellationToken);
        if (teacherClass == null)
        {
            return new CommitResult
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "X0000",
                ErrorMessage = _resourceJsonManager["X0000"]
            };
        }

        await _notifierClient.SendInvitationAsync(new InvitationRequest
        {
            InviterId = _studentId.GetValueOrDefault(),
            InvitedId = teacherClass.TeacherId,
            Argument = request.ClassId.ToString(),
            InvitationTypeId = 4,
            IsActive = true,
            AppenedMessage = teacherClass.Name
        }, cancellationToken);

        return new CommitResult
        {
            ResultType = ResultType.Ok
        };
    }
}
