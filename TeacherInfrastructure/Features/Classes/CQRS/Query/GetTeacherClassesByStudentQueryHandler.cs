
using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class GetTeacherClassesByStudentQueryHandler : IRequestHandler<GetTeacherClassesByStudentQuery, ICommitResult<TeacherClassesByStudentResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly CurriculumClient _curriculumClient;
    private readonly IdentityClient _identityClient;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetTeacherClassesByStudentQueryHandler(TeacherDbContext dbContext,
                                                  IdentityClient identityClient,
                                                  CurriculumClient curriculumClient,
                                                  IWebHostEnvironment configuration,
                                                  IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _identityClient = identityClient;
        _curriculumClient = curriculumClient;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());

    }

    public async Task<ICommitResult<TeacherClassesByStudentResponse>> Handle(GetTeacherClassesByStudentQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ClassEnrollee> enrolleeClasses = await _dbContext.Set<ClassEnrollee>()
                                                                            .Where(a => a.StudentId.Equals(request.Request.StudenId)
                                                                                        && a.TeacherClassFK.TeacherId.Equals(request.Request.TeacherId)
                                                                                        && a.IsActive)
                                                                            .Include(a => a.TeacherClassFK)
                                                                            .ToListAsync(cancellationToken);
        if (!enrolleeClasses.Any())
        {
            return ResultType.Empty.GetValueCommitResult<TeacherClassesByStudentResponse>(default, "X0007", _resourceJsonManager["X0007"]);
        }
        IEnumerable<ClassEnrollee> teacherClasses = await _dbContext.Set<ClassEnrollee>()
                                                                          .Where(a => a.TeacherClassFK.TeacherId.Equals(request.Request.TeacherId)
                                                                                      && a.IsActive)
                                                                          .Include(a => a.TeacherClassFK)
                                                                          .ToListAsync(cancellationToken);
        if (!teacherClasses.Any())
        {
            return ResultType.Empty.GetValueCommitResult<TeacherClassesByStudentResponse>(default, "X0007", _resourceJsonManager["X0007"]);
        }

        ICommitResult<LimitedProfileResponse>? limitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(request.Request.TeacherId, cancellationToken);

        if (!limitedProfile.IsSuccess)
        {
            return new CommitResult<TeacherClassesByStudentResponse>
            {
                ErrorCode = limitedProfile.ErrorCode,
                ResultType = limitedProfile.ResultType,
                ErrorMessage = limitedProfile.ErrorMessage
            };
        }
        ICommitResults<SubjectResponse>? subjects = await _curriculumClient.GetSubjectsDetailsAsync(teacherClasses.Select(a => a.TeacherClassFK).Select(a => a.SubjectId), cancellationToken);

        if (!subjects.IsSuccess)
        {
            return subjects.ResultType.GetValueCommitResult((TeacherClassesByStudentResponse)null, subjects.ErrorCode, subjects.ErrorMessage);
        }

        IEnumerable<ClassBriefResponse> ClassMapper()
        {
            foreach (ClassEnrollee classDetails in enrolleeClasses)
            {
                yield return new ClassBriefResponse
                {
                    Id = classDetails.Id,
                    Name = classDetails.TeacherClassFK.Name
                    //subject??
                };
            }
            yield break;
        }

        IEnumerable<SubjectBriefResponse> SubjectBriefMapper()
        {

            foreach (SubjectResponse subject in subjects.Value)
            {
                yield return new SubjectBriefResponse
                {
                    Id = subject.Id,
                    Name = subject.ShortName,
                    InternalIcon = subject.InternalIcon,
                };
            }
            yield break;
        }

        return ResultType.Ok.GetValueCommitResult(new TeacherClassesByStudentResponse
        {
            TeacherName = limitedProfile.Value.FullName,
            Classes = ClassMapper(),
            Subjects = SubjectBriefMapper(),
            ClassCounter = teacherClasses.Count()
        });


    }
}