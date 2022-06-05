using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class SearchClassQueryHandler : IRequestHandler<SearchClassQuery, CommitResult<ClassResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly IdentityClient _identityClient;
    private readonly CurriculumClient _curriculumClient;
    private readonly NotifierClient _notifierClient;
    private readonly Guid? _userId;
    public SearchClassQueryHandler(IHttpContextAccessor httpContextAccessor, TeacherDbContext dbContext, IdentityClient identityClient, CurriculumClient curriculumClient, NotifierClient notifierClient)
    {
        _dbContext = dbContext;
        _identityClient = identityClient;
        _userId = httpContextAccessor.GetIdentityUserId();
        _curriculumClient = curriculumClient;
        _notifierClient = notifierClient;
    }
    public async Task<CommitResult<ClassResponse>> Handle(SearchClassQuery request, CancellationToken cancellationToken)
    {
        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>()
                                      .Where(a => a.Id.Equals(request.ClassId) && a.IsActive)
                                      .SingleOrDefaultAsync(cancellationToken);

        if (teacherClass == null)
        {
            return new CommitResult<ClassResponse>
            {
                ResultType = ResultType.NotFound
            };
        }

        CommitResult<LimitedProfileResponse>? studentLimitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(_userId.GetValueOrDefault(), cancellationToken);

        if (!studentLimitedProfile.IsSuccess)
        {
            return new CommitResult<ClassResponse>
            {
                ErrorCode = studentLimitedProfile.ErrorCode,
                ResultType = studentLimitedProfile.ResultType,
                ErrorMessage = studentLimitedProfile.ErrorMessage
            };
        }

        CommitResult<bool>? subjectMaching = await _curriculumClient.VerifySubjectGradeMatchingAsync(teacherClass.SubjectId, studentLimitedProfile.Value.GradeId, cancellationToken);

        if (!subjectMaching.IsSuccess)
        {
            return new CommitResult<ClassResponse>
            {
                ErrorCode = subjectMaching.ErrorCode,
                ResultType = subjectMaching.ResultType,
                ErrorMessage = subjectMaching.ErrorMessage
            };
        }

        if (!subjectMaching.Value)
        {
            return new CommitResult<ClassResponse>
            {
                ResultType = ResultType.NotFound
            };
        }

        CommitResult<LimitedProfileResponse>? teacherLimitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(teacherClass.TeacherId, cancellationToken);

        if (!teacherLimitedProfile.IsSuccess)
        {
            return new CommitResult<ClassResponse>
            {
                ErrorCode = teacherLimitedProfile.ErrorCode,
                ResultType = teacherLimitedProfile.ResultType,
                ErrorMessage = teacherLimitedProfile.ErrorMessage
            };
        }

        CommitResults<ClassStatusResponse>? classStatus = await _notifierClient.GetClassesStatusAsync(new int[] { teacherClass.Id }, cancellationToken);

        if (!classStatus.IsSuccess)
        {
            return new CommitResult<ClassResponse>
            {
                ErrorCode = classStatus.ErrorCode,
                ResultType = classStatus.ResultType,
                ErrorMessage = classStatus.ErrorMessage,
            };
        }

        return new CommitResult<ClassResponse>
        {
            ResultType = ResultType.Ok,
            Value = new ClassResponse
            {
                TeacherId = teacherClass.TeacherId,
                AvatarUrl = teacherLimitedProfile.Value.AvatarImage,
                Description = teacherClass.Description,
                ClassId = teacherClass.Id,
                Name = teacherClass.Name,
                SubjectId = teacherClass.SubjectId,
                TeacherName = teacherLimitedProfile.Value.FullName,
                IsEnrolled = IsEnrollerMapper(classStatus.Value.FirstOrDefault()),
            }
        };
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