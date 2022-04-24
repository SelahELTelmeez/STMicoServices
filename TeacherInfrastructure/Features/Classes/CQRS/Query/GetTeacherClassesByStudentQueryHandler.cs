
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherDomain.Features.Shared.DTO;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class GetTeacherClassesByStudentQueryHandler : IRequestHandler<GetTeacherClassesByStudentQuery, CommitResult<TeacherClassesByStudentResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly CurriculumClient _curriculumClient;
    private readonly IdentityClient _identityClient;
    public GetTeacherClassesByStudentQueryHandler(TeacherDbContext dbContext, IdentityClient identityClient, CurriculumClient curriculumClient)
    {
        _dbContext = dbContext;
        _identityClient = identityClient;
        _curriculumClient = curriculumClient;
    }

    public async Task<CommitResult<TeacherClassesByStudentResponse>> Handle(GetTeacherClassesByStudentQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ClassEnrollee> enrolleeClasses = await _dbContext.Set<ClassEnrollee>()
                                                                            .Where(a => a.StudentId.Equals(request.Request.StudenId)
                                                                                      && a.TeacherClassFK.TeacherId.Equals(request.Request.TeacherId)
                                                                                      && a.IsActive)
                                                                            .Include(a => a.TeacherClassFK)
                                                                            .ToListAsync(cancellationToken);

        IEnumerable<ClassEnrollee> teacherClasses = await _dbContext.Set<ClassEnrollee>()
                                                                          .Where(a => a.TeacherClassFK.TeacherId.Equals(request.Request.TeacherId)
                                                                                   && a.IsActive)
                                                                          .Include(a => a.TeacherClassFK)
                                                                          .ToListAsync(cancellationToken);


        if (enrolleeClasses.Any())
        {
            CommitResult<LimitedProfileResponse>? limitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(request.Request.TeacherId, cancellationToken);
            if (!limitedProfile.IsSuccess)
            {
                return limitedProfile.Adapt<CommitResult<TeacherClassesByStudentResponse>>();
            }
            CommitResults<SubjectResponse>? subjects = await _curriculumClient.GetSubjectsDetailsAsync(teacherClasses.Select(a => a.TeacherClassFK).Select(a => a.SubjectId), cancellationToken);
            if (!subjects.IsSuccess)
            {
                return subjects.Adapt<CommitResult<TeacherClassesByStudentResponse>>();
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
                        Title = subject.Title,
                        InternalIcon = subject.InternalIcon,
                    };
                }
                yield break;
            }

            return new CommitResult<TeacherClassesByStudentResponse>
            {
                ResultType = ResultType.Ok,
                Value = new TeacherClassesByStudentResponse
                {
                    TeacherName = limitedProfile.Value.FullName,
                    Classes = ClassMapper(),
                    Subjects = SubjectBriefMapper(),
                    ClassCounter = teacherClasses.Count()
                }
            };
        }
        else
        {
            return new CommitResult<TeacherClassesByStudentResponse>
            {
                ResultType = ResultType.Empty
            };
        }
    }
}