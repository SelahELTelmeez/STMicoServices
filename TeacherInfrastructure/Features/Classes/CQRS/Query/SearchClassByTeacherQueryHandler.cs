using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherDomain.Features.Shared.DTO;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class SearchClassByTeacherQueryHandler : IRequestHandler<SearchClassByTeacherQuery, CommitResults<ClassResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly IdentityClient _identityClient;
    public SearchClassByTeacherQueryHandler(IHttpContextAccessor httpContextAccessor, TeacherDbContext dbContext, IdentityClient identityClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _identityClient = identityClient;
    }
    public async Task<CommitResults<ClassResponse>> Handle(SearchClassByTeacherQuery request, CancellationToken cancellationToken)
    {
        CommitResults<LimitedProfileResponse>? limitedProfileResponse = await _identityClient.GetTeacherLimitedProfilesByNameOrMobileNumberAsync(request.NameOrMobile, cancellationToken);
        if (!limitedProfileResponse.IsSuccess)
        {
            return limitedProfileResponse.Adapt<CommitResults<ClassResponse>>();
        }

        IEnumerable<TeacherClass>? teacherClasses = await _dbContext.Set<TeacherClass>()
                                                                    .Where(a => limitedProfileResponse.Value.Select(b => b.UserId).Contains(a.TeacherId))
                                                                    .Include(a => a.ClassEnrollees)
                                                                    .ToListAsync(cancellationToken);

        IEnumerable<ClassResponse> Mapper()
        {
            foreach (TeacherClass teacherClass in teacherClasses)
            {
                LimitedProfileResponse? profileResponse = limitedProfileResponse?.Value?.SingleOrDefault(a => a.UserId.Equals(teacherClass.TeacherId));

                yield return new ClassResponse
                {
                    ClassId = teacherClass.Id,
                    Description = teacherClass.Description,
                    Name = teacherClass.Name,
                    SubjectId = teacherClass.SubjectId,
                    TeacherId = teacherClass.TeacherId,
                    TeacherName = profileResponse?.FullName,
                    AvatarUrl = profileResponse?.AvatarImage,
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