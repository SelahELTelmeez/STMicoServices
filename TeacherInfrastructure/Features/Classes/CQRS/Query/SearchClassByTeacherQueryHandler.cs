using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class SearchClassByTeacherQueryHandler : IRequestHandler<SearchClassByTeacherQuery, CommitResults<ClassResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly IdentityClient _identityClient;
    private readonly NotifierClient _notifierClient;

    public SearchClassByTeacherQueryHandler(TeacherDbContext dbContext, IdentityClient identityClient, NotifierClient notifierClient)
    {
        _dbContext = dbContext;
        _identityClient = identityClient;
        _notifierClient = notifierClient;
    }
    public async Task<CommitResults<ClassResponse>> Handle(SearchClassByTeacherQuery request, CancellationToken cancellationToken)
    {
        CommitResults<LimitedProfileResponse>? limitedProfileResponse = await _identityClient.GetTeacherLimitedProfilesByNameOrMobileNumberAsync(request.NameOrMobile, cancellationToken);

        if (!limitedProfileResponse.IsSuccess)
        {
            return new CommitResults<ClassResponse>
            {
                ErrorCode = limitedProfileResponse.ErrorCode,
                ResultType = limitedProfileResponse.ResultType,
                ErrorMessage = limitedProfileResponse.ErrorMessage
            };
        }

        IEnumerable<TeacherClass>? teacherClasses = await _dbContext.Set<TeacherClass>()
                                                                    .Where(a => limitedProfileResponse.Value.Select(b => b.UserId).Contains(a.TeacherId))
                                                                    .ToListAsync(cancellationToken);


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
                LimitedProfileResponse? profileResponse = limitedProfileResponse?.Value?.SingleOrDefault(a => a.UserId.Equals(teacherClass.TeacherId));
                ClassStatusResponse? classStatusResponse = classStatuses.Value.SingleOrDefault(a => a.ClassId == teacherClass.Id);

                yield return new ClassResponse
                {
                    ClassId = teacherClass.Id,
                    Description = teacherClass.Description,
                    Name = teacherClass.Name,
                    SubjectId = teacherClass.SubjectId,
                    TeacherId = teacherClass.TeacherId,
                    TeacherName = profileResponse?.FullName,
                    AvatarUrl = profileResponse?.AvatarImage,
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