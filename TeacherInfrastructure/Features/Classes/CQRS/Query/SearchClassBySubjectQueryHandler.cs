using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class SearchClassBySubjectQueryHandler : IRequestHandler<SearchClassBySubjectQuery, CommitResults<ClassResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly IdentityClient _identityClient;
    private readonly Guid? _userId;
    private readonly NotifierClient _notifierClient;

    public SearchClassBySubjectQueryHandler(TeacherDbContext dbContext, IdentityClient identityClient, IHttpContextAccessor httpContextAccessor, NotifierClient notifierClient = null)
    {
        _dbContext = dbContext;
        _identityClient = identityClient;
        _userId = httpContextAccessor.GetIdentityUserId();
        _notifierClient = notifierClient;
    }
    public async Task<CommitResults<ClassResponse>> Handle(SearchClassBySubjectQuery request, CancellationToken cancellationToken)
    {

        IEnumerable<TeacherClass> teacherClasses = await _dbContext.Set<TeacherClass>()
                                                                   .Where(a => a.SubjectId.Equals(request.SubjectId) && a.IsActive)
                                                                   .ToListAsync(cancellationToken);

        CommitResults<LimitedProfileResponse>? limitedProfiles = await _identityClient.GetIdentityLimitedProfilesAsync(teacherClasses.Select(a => a.TeacherId), cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return new CommitResults<ClassResponse>
            {
                ErrorCode = limitedProfiles.ErrorCode,
                ResultType = limitedProfiles.ResultType,
                ErrorMessage = limitedProfiles.ErrorMessage
            };
        }


        CommitResults<ClassStatusResponse>? classStatuses = await _notifierClient.GetClassesStatusAsync(teacherClasses.Select(a => a.Id), cancellationToken);

        if (!classStatuses.IsSuccess)
        {
            return new CommitResults<ClassResponse>
            {
                ErrorCode = classStatuses.ErrorCode,
                ResultType = classStatuses.ResultType,
                ErrorMessage = classStatuses.ErrorMessage,
            };
        }


        IEnumerable<ClassResponse> Mapper()
        {
            foreach (TeacherClass teacherClass in teacherClasses)
            {
                LimitedProfileResponse? profileResponse = limitedProfiles?.Value?.SingleOrDefault(a => a.UserId.Equals(teacherClass.TeacherId));
                ClassStatusResponse? classStatusResponse = classStatuses.Value.SingleOrDefault(a => a.ClassId == teacherClass.Id);

                yield return new ClassResponse
                {
                    ClassId = teacherClass.Id,
                    Description = teacherClass.Description,
                    Name = teacherClass.Name,
                    SubjectId = teacherClass.SubjectId,
                    TeacherId = teacherClass.TeacherId,
                    TeacherName = profileResponse?.FullName,
                    AvatarUrl = profileResponse.AvatarImage,
                    IsEnrolled = IsEnrollerMapper(classStatusResponse),
                };
            }
            yield break;
        }

        return new CommitResults<ClassResponse>
        {
            ResultType = ResultType.Ok,
            Value = Mapper()
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