using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class GetEnrolledStudentsByClassQueryHandler : IRequestHandler<GetEnrolledStudentsByClassQuery, ICommitResults<EnrolledStudentResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly IdentityClient? _IdentityClient;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public GetEnrolledStudentsByClassQueryHandler(TeacherDbContext dbContext,
                                                  IdentityClient? identityClient,
                                                  IWebHostEnvironment configuration,
                                                  IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _IdentityClient = identityClient;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }
    public async Task<ICommitResults<EnrolledStudentResponse>> Handle(GetEnrolledStudentsByClassQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ClassEnrollee> classEnrollees = await _dbContext.Set<ClassEnrollee>()
                                                                    .Where(a => a.IsActive && a.ClassId.Equals(request.ClassId))
                                                                    .ToListAsync(cancellationToken);

        if (!classEnrollees.Any())
        {
            return ResultType.Ok.GetValueCommitResults(Array.Empty<EnrolledStudentResponse>());
        }
        ICommitResults<LimitedProfileResponse>? profileResponses = await _IdentityClient.GetIdentityLimitedProfilesAsync(classEnrollees.Select(a => a.StudentId), cancellationToken);

        if (!profileResponses.IsSuccess)
        {
            return profileResponses.ResultType.GetValueCommitResults(Array.Empty<EnrolledStudentResponse>(), profileResponses.ErrorCode, profileResponses.ErrorMessage);
        }

        IEnumerable<EnrolledStudentResponse> Mapper()
        {
            foreach (ClassEnrollee studentEnroll in classEnrollees)
            {
                LimitedProfileResponse? profileResponse = profileResponses.Value.FirstOrDefault(a => a.UserId.Equals(studentEnroll.StudentId));

                yield return new EnrolledStudentResponse
                {
                    ClassId = studentEnroll.ClassId,
                    JoinedDate = studentEnroll.CreatedOn.GetValueOrDefault(),
                    StudentId = studentEnroll.StudentId,
                    AvatarUrl = profileResponse?.AvatarImage,
                    GradeValue = profileResponse?.GradeId ?? 0,
                    GradeName = profileResponse?.GradeName,
                    StudentName = profileResponse?.FullName,
                };
            }
        }

        return ResultType.Ok.GetValueCommitResults(Mapper());
    }
}