using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.TeacherClass.DTO.Query;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class GetStudentClassesQueryHandler : IRequestHandler<GetStudentClassesQuery, ICommitResults<StudentClassResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly IHttpContextAccessor? _httpContextAccessor;
    private readonly CurriculumClient _curriculumClient;
    private readonly IdentityClient _IdentityClient;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetStudentClassesQueryHandler(IHttpContextAccessor httpContextAccessor,
                                         TeacherDbContext dbContext,
                                         CurriculumClient curriculumClient,
                                         IdentityClient identityClient,
                                         IWebHostEnvironment configuration)
    {
        _httpContextAccessor = httpContextAccessor;
        _dbContext = dbContext;
        _curriculumClient = curriculumClient;
        _IdentityClient = identityClient;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<ICommitResults<StudentClassResponse>> Handle(GetStudentClassesQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<TeacherClass> TeacherClasses = await _dbContext.Set<ClassEnrollee>()
                       .Where(a => a.StudentId.Equals(request.StudentId ?? _httpContextAccessor.GetIdentityUserId()) && a.IsActive)
                       .Include(a => a.TeacherClassFK)
                       .Select(a => a.TeacherClassFK)
                       .ToListAsync(cancellationToken);

        if (!TeacherClasses.Any())
        {
            return ResultType.NotFound.GetValueCommitResults<StudentClassResponse>(default, "X0007", _resourceJsonManager["X0007"]);
        }


        ICommitResults<LimitedProfileResponse>? teacherLimitedProfiles = await _IdentityClient.GetIdentityLimitedProfilesAsync(TeacherClasses.Select(a => a.TeacherId), cancellationToken);
        if (!teacherLimitedProfiles.IsSuccess)
        {
            return teacherLimitedProfiles.ResultType.GetValueCommitResults(Array.Empty<StudentClassResponse>(), teacherLimitedProfiles.ErrorCode, teacherLimitedProfiles.ErrorMessage);
        }

        ICommitResults<SubjectBriefResponse>? subjectBriefResponses = await _curriculumClient.GetSubjectsBriefAsync(TeacherClasses.Select(a => a.SubjectId), cancellationToken);
        if (!subjectBriefResponses.IsSuccess)
        {
            return subjectBriefResponses.ResultType.GetValueCommitResults(Array.Empty<StudentClassResponse>(), subjectBriefResponses.ErrorCode, subjectBriefResponses.ErrorMessage);
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

        return ResultType.Ok.GetValueCommitResults(Mapper());
    }
}
