using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
public class RequestEnrollToClassCommandHandler : IRequestHandler<RequestEnrollToClassCommand, ICommitResult>
{
    private readonly NotifierClient _notifierClient;
    private readonly TeacherDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly string? _studentId;

    public RequestEnrollToClassCommandHandler(
            TeacherDbContext dbContext,
            NotifierClient notifierClient,
            IWebHostEnvironment configuration,
            IHttpContextAccessor httpContextAccessor
            )
    {
        _notifierClient = notifierClient;
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _studentId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<ICommitResult> Handle(RequestEnrollToClassCommand request, CancellationToken cancellationToken)
    {

        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>().FirstOrDefaultAsync(a => a.Id.Equals(request.ClassId), cancellationToken);

        if (teacherClass == null)
        {
            return ResultType.NotFound.GetCommitResult("XTEC0001", _resourceJsonManager["XTEC0001"]);
        }

        await _notifierClient.SendInvitationAsync(new InvitationRequest
        {
            InviterId = _studentId,
            InvitedId = teacherClass.TeacherId,
            Argument = request.ClassId.ToString(),
            InvitationTypeId = 4,
            IsActive = true,
            AppenedMessage = teacherClass.Name
        }, cancellationToken);

        return ResultType.Ok.GetCommitResult();
    }
}
