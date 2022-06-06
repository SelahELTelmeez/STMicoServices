using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Command;
using TeacherDomain.Features.Shared.DTO;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Command;
public class RequestEnrollToClassCommandHandler : IRequestHandler<RequestEnrollToClassCommand, ICommitResult>
{
    private readonly NotifierClient _notifierClient;
    private readonly IdentityClient _identityClient;
    private readonly TeacherDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly Guid? _studentId;

    public RequestEnrollToClassCommandHandler(
            TeacherDbContext dbContext,
            NotifierClient notifierClient,
            IWebHostEnvironment configuration,
            IHttpContextAccessor httpContextAccessor,
            IdentityClient identityClient)
    {
        _notifierClient = notifierClient;
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _studentId = httpContextAccessor.GetIdentityUserId();
        _identityClient = identityClient;
    }
    public async Task<ICommitResult> Handle(RequestEnrollToClassCommand request, CancellationToken cancellationToken)
    {

        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>().SingleOrDefaultAsync(a => a.Id.Equals(request.ClassId), cancellationToken);
        if (teacherClass == null)
        {
            return ResultType.NotFound.GetCommitResult("X0000", _resourceJsonManager["X0000"]);
        }

        ICommitResult<LimitedProfileResponse>? profileResult = await _identityClient.GetIdentityLimitedProfileAsync(_studentId.GetValueOrDefault(), cancellationToken);

        if (!profileResult.IsSuccess)
        {
            return ResultType.NotFound.GetCommitResult("X0000", _resourceJsonManager["X0000"]);
        }

        await _notifierClient.SendInvitationAsync(new InvitationRequest
        {
            InviterId = _studentId.GetValueOrDefault(),
            InvitedId = teacherClass.TeacherId,
            Argument = request.ClassId.ToString(),
            InvitationTypeId = 4,
            IsActive = true,
            AppenedMessage = profileResult.Value.FullName
        }, cancellationToken);

        return ResultType.Ok.GetCommitResult();
    }
}
