using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class SearchClassQueryHandler : IRequestHandler<SearchClassQuery, ICommitResult<ClassResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly IdentityClient _identityClient;
    private readonly CurriculumClient _curriculumClient;
    private readonly NotifierClient _notifierClient;
    private readonly Guid? _userId;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public SearchClassQueryHandler(IHttpContextAccessor httpContextAccessor,
                                   TeacherDbContext dbContext,
                                   IWebHostEnvironment configuration,
                                   IdentityClient identityClient,
                                   CurriculumClient curriculumClient,
                                   NotifierClient notifierClient)
    {
        _dbContext = dbContext;
        _identityClient = identityClient;
        _userId = httpContextAccessor.GetIdentityUserId();
        _curriculumClient = curriculumClient;
        _notifierClient = notifierClient;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());

    }
    public async Task<ICommitResult<ClassResponse>> Handle(SearchClassQuery request, CancellationToken cancellationToken)
    {
        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>()
                                      .Where(a => a.Id.Equals(request.ClassId) && a.IsActive)
                                      .SingleOrDefaultAsync(cancellationToken);

        if (teacherClass == null)
        {
            return ResultType.NotFound.GetValueCommitResult<ClassResponse>(default, "X0001", _resourceJsonManager["X0001"]);
        }

        ICommitResult<LimitedProfileResponse>? studentLimitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(_userId.GetValueOrDefault(), cancellationToken);

        if (!studentLimitedProfile.IsSuccess)
        {
            return studentLimitedProfile.ResultType.GetValueCommitResult((ClassResponse)null, studentLimitedProfile.ErrorCode, studentLimitedProfile.ErrorMessage);
        }

        ICommitResult<bool>? subjectMaching = await _curriculumClient.VerifySubjectGradeMatchingAsync(teacherClass.SubjectId, studentLimitedProfile.Value.GradeId, cancellationToken);

        if (!subjectMaching.IsSuccess)
        {
            return subjectMaching.ResultType.GetValueCommitResult((ClassResponse)null, subjectMaching.ErrorCode, subjectMaching.ErrorMessage);
        }

        ICommitResult<LimitedProfileResponse>? teacherLimitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(teacherClass.TeacherId, cancellationToken);

        if (!teacherLimitedProfile.IsSuccess)
        {
            return teacherLimitedProfile.ResultType.GetValueCommitResult((ClassResponse)null, teacherLimitedProfile.ErrorCode, teacherLimitedProfile.ErrorMessage);
        }

        ICommitResults<ClassStatusResponse>? classStatus = await _notifierClient.GetClassesStatusAsync(new int[] { teacherClass.Id }, cancellationToken);

        if (!classStatus.IsSuccess)
        {
            return classStatus.ResultType.GetValueCommitResult((ClassResponse)null, classStatus.ErrorCode, classStatus.ErrorMessage);
        }

        return ResultType.Ok.GetValueCommitResult(new ClassResponse
        {
            TeacherId = teacherClass.TeacherId,
            AvatarUrl = teacherLimitedProfile.Value.AvatarImage,
            Description = teacherClass.Description,
            ClassId = teacherClass.Id,
            Name = teacherClass.Name,
            SubjectId = teacherClass.SubjectId,
            TeacherName = teacherLimitedProfile.Value.FullName,
            IsEnrolled = IsEnrollerMapper(classStatus.Value.FirstOrDefault()),
        });
    }

    private bool? IsEnrollerMapper(ClassStatusResponse? classStatus)
    {
        if (classStatus == null)
            return false;
        else if (classStatus.Status == 0) // None
            return false;
        else if (classStatus.Status == 1) // Accepted
            return true;
        else if (classStatus.Status == 2) // Declined
            return false;
        else if (classStatus.Status == 3)
            return null;
        else
            return false;
    }
}