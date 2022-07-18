using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;

public class SearchClassBySubjectQueryHandler : IRequestHandler<SearchClassBySubjectQuery, ICommitResults<ClassResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly IdentityClient _identityClient;
    private readonly NotifierClient _notifierClient;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public SearchClassBySubjectQueryHandler(TeacherDbContext dbContext, IdentityClient identityClient, IHttpContextAccessor httpContextAccessor, NotifierClient notifierClient, IWebHostEnvironment configuration)
    {
        _dbContext = dbContext;
        _identityClient = identityClient;
        _notifierClient = notifierClient;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());

    }
    public async Task<ICommitResults<ClassResponse>> Handle(SearchClassBySubjectQuery request, CancellationToken cancellationToken)
    {

        IEnumerable<TeacherClass> teacherClasses = await _dbContext.Set<TeacherClass>()
                                                                   .Where(a => a.SubjectId.Equals(request.SubjectId) && a.IsActive)
                                                                   .ToListAsync(cancellationToken);

        if (!teacherClasses.Any())
        {
            return ResultType.Ok.GetValueCommitResults(Array.Empty<ClassResponse>());
        }

        ICommitResults<LimitedProfileResponse>? limitedProfiles = await _identityClient.GetIdentityLimitedProfilesAsync(teacherClasses.Select(a => a.TeacherId), cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return limitedProfiles.ResultType.GetValueCommitResults(Array.Empty<ClassResponse>(), limitedProfiles.ErrorCode, limitedProfiles.ErrorMessage);
        }

        ICommitResults<ClassStatusResponse>? classStatuses = await _notifierClient.GetClassesStatusAsync(teacherClasses.Select(a => a.Id), cancellationToken);

        if (!classStatuses.IsSuccess)
        {
            return classStatuses.ResultType.GetValueCommitResults(Array.Empty<ClassResponse>(), classStatuses.ErrorCode, classStatuses.ErrorMessage);
        }

        IEnumerable<ClassResponse> Mapper()
        {
            foreach (TeacherClass teacherClass in teacherClasses)
            {
                LimitedProfileResponse? profileResponse = limitedProfiles?.Value?.FirstOrDefault(a => a.UserId.Equals(teacherClass.TeacherId));

                ClassStatusResponse? classStatusResponse = classStatuses.Value?.FirstOrDefault(a => a.ClassId == teacherClass.Id);

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

        return ResultType.Ok.GetValueCommitResults(Mapper());
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