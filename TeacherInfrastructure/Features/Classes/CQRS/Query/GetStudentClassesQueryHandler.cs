using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.TeacherClass.DTO.Query;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class GetStudentClassesQueryHandler : IRequestHandler<GetStudentClassesQuery, CommitResults<StudentClassResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly CurriculumClient _curriculumClient;
    private readonly IdentityClient _IdentityClient;
    public GetStudentClassesQueryHandler(IHttpContextAccessor httpContextAccessor, TeacherDbContext dbContext, CurriculumClient curriculumClient, IdentityClient identityClient)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
        _curriculumClient = curriculumClient;
        _IdentityClient = identityClient;
    }
    public async Task<CommitResults<StudentClassResponse>> Handle(GetStudentClassesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<TeacherClass> TeacherClasses = await _dbContext.Set<ClassEnrollee>()
                       .Where(a => a.StudentId.Equals(request.StudentId ?? _httpContextAccessor.GetIdentityUserId()) && a.IsActive)
                       .Include(a => a.TeacherClassFK)
                       .Select(a => a.TeacherClassFK)
                       .ToListAsync(cancellationToken);


        CommitResults<LimitedProfileResponse>? teacherLimitedProfiles = await _IdentityClient.GetIdentityLimitedProfilesAsync(TeacherClasses.Select(a => a.TeacherId), cancellationToken);
        if (!teacherLimitedProfiles.IsSuccess)
        {
            return new CommitResults<StudentClassResponse>
            {
                ErrorCode = teacherLimitedProfiles.ErrorCode,
                ResultType = teacherLimitedProfiles.ResultType,
                ErrorMessage = teacherLimitedProfiles.ErrorMessage
            };
        }

        CommitResults<SubjectBriefResponse>? subjectBriefResponses = await _curriculumClient.GetSubjectsBriefAsync(TeacherClasses.Select(a => a.SubjectId), cancellationToken);
        if (!subjectBriefResponses.IsSuccess)
        {
            return new CommitResults<StudentClassResponse>
            {
                ErrorCode = subjectBriefResponses.ErrorCode,
                ResultType = subjectBriefResponses.ResultType,
                ErrorMessage = subjectBriefResponses.ErrorMessage
            };
        }


        IEnumerable<StudentClassResponse> Mapper()
        {
            foreach (TeacherClass teacherClass in TeacherClasses)
            {
                SubjectBriefResponse? subjectBrief = subjectBriefResponses.Value.FirstOrDefault(a => a.Id == teacherClass.SubjectId);
                LimitedProfileResponse? limitedProfile = teacherLimitedProfiles.Value.FirstOrDefault(a => a.UserId == teacherClass.TeacherId);
                yield return new StudentClassResponse
                {
                    Description = teacherClass.Description,
                    SubjectId = teacherClass.SubjectId,
                    Name = teacherClass.Name,
                    IsActive = teacherClass.IsActive,
                    Id = teacherClass.Id,
                    SubjectName = subjectBrief.Name,
                    TeacherName = limitedProfile.FullName
                };
            }
            yield break;
        }

        return new CommitResults<StudentClassResponse>
        {
            ResultType = ResultType.Ok,
            Value = Mapper()
        };
    }
}
