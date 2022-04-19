using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherDomain.Features.Shared.DTO;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class SearchClassQueryHandler : IRequestHandler<SearchClassQuery, CommitResult<ClassResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly IdentityClient _identityClient;
    private readonly CurriculumClient _curriculumClient;
    private readonly Guid? _userId;
    public SearchClassQueryHandler(IHttpContextAccessor httpContextAccessor, TeacherDbContext dbContext, IdentityClient identityClient, CurriculumClient curriculumClient)
    {
        _dbContext = dbContext;
        _identityClient = identityClient;
        _userId = httpContextAccessor.GetIdentityUserId();
        _curriculumClient = curriculumClient;
    }
    public async Task<CommitResult<ClassResponse>> Handle(SearchClassQuery request, CancellationToken cancellationToken)
    {
        TeacherClass? teacherClass = await _dbContext.Set<TeacherClass>()
                                      .Where(a => a.Id.Equals(request.ClassId) && a.IsActive)
                                      .Include(a => a.ClassEnrollees)
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
            return studentLimitedProfile.Adapt<CommitResult<ClassResponse>>();
        }

        CommitResult<bool>? subjectMaching = await _curriculumClient.VerifySubjectGradeMatchingAsync(teacherClass.SubjectId, studentLimitedProfile.Value.GradeId, cancellationToken);

        if (!subjectMaching.IsSuccess)
        {
            return subjectMaching.Adapt<CommitResult<ClassResponse>>();
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
            return teacherLimitedProfile.Adapt<CommitResult<ClassResponse>>();
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
                IsEntrolled = teacherClass.ClassEnrollees.Any(a => a.StudentId.Equals(_userId)),
            }
        };
    }
}