using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherDomain.Features.Shared.DTO;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class SearchClassBySubjectQueryHandler : IRequestHandler<SearchClassBySubjectQuery, CommitResults<ClassResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly IdentityClient _identityClient;
    private readonly Guid? _userId;

    public SearchClassBySubjectQueryHandler(TeacherDbContext dbContext, IdentityClient identityClient, IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _identityClient = identityClient;
        _userId = httpContextAccessor.GetIdentityUserId();
    }
    public async Task<CommitResults<ClassResponse>> Handle(SearchClassBySubjectQuery request, CancellationToken cancellationToken)
    {

        IEnumerable<TeacherClass> teacherClasses = await _dbContext.Set<TeacherClass>()
                                                                   .Where(a => a.SubjectId.Equals(request.SubjectId) && a.IsActive)
                                                                   .Include(a => a.ClassEnrollees)
                                                                   .ToListAsync(cancellationToken);

        CommitResults<LimitedProfileResponse>? limitedProfiles = await _identityClient.GetIdentityLimitedProfilesAsync(teacherClasses.Select(a => a.TeacherId), cancellationToken);

        if (!limitedProfiles.IsSuccess)
        {
            return limitedProfiles.Adapt<CommitResults<ClassResponse>>();
        }

        IEnumerable<ClassResponse> Mapper()
        {
            foreach (TeacherClass teacherClass in teacherClasses)
            {
                LimitedProfileResponse? profileResponse = limitedProfiles?.Value?.SingleOrDefault(a => a.UserId.Equals(teacherClass.TeacherId));
                yield return new ClassResponse
                {
                    ClassId = teacherClass.Id,
                    Description = teacherClass.Description,
                    Name = teacherClass.Name,
                    SubjectId = teacherClass.SubjectId,
                    TeacherId = teacherClass.TeacherId,
                    TeacherName = profileResponse?.FullName,
                    AvatarUrl = profileResponse.AvatarImage,
                    IsEnrolled = teacherClass.ClassEnrollees.Any(a => a.StudentId.Equals(_userId)),
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
}