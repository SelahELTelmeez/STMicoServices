using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherDomain.Features.Shared.DTO;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;

public class GetEnrolleeDetailsQueryHandler : IRequestHandler<GetEnrolleeDetailsQuery, CommitResult<EnrolleeDetailsResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly CurriculumClient _curriculumClient;
    private readonly IdentityClient _identityClient;
    public GetEnrolleeDetailsQueryHandler(TeacherDbContext dbContext, IdentityClient identityClient, CurriculumClient curriculumClient)
    {
        _dbContext = dbContext;
        _identityClient = identityClient;
        _curriculumClient = curriculumClient;
    }
    public async Task<CommitResult<EnrolleeDetailsResponse>> Handle(GetEnrolleeDetailsQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ClassEnrollee> enrolleeClasses = await _dbContext.Set<ClassEnrollee>()
                                                                     .Where(a => a.StudentId.Equals(request.EnrolleeId) && a.IsActive)
                                                                     .Include(a => a.TeacherClassFK)
                                                                     .ToListAsync(cancellationToken);
        if (enrolleeClasses.Any())
        {
            CommitResult<LimitedProfileResponse>? limitedProfile = await _identityClient.GetIdentityLimitedProfileAsync(request.EnrolleeId, cancellationToken);
            if (!limitedProfile.IsSuccess)
            {
                return limitedProfile.Adapt<CommitResult<EnrolleeDetailsResponse>>();
            }
            CommitResults<SubjectResponse>? subjects = await _curriculumClient.GetSubjectsDetailsAsync(enrolleeClasses.Select(a => a.TeacherClassFK).Select(a => a.SubjectId), cancellationToken);
            if (!subjects.IsSuccess)
            {
                return subjects.Adapt<CommitResult<EnrolleeDetailsResponse>>();
            }

            IEnumerable<ClassBriefResponse> ClassMapper()
            {
                foreach (ClassEnrollee classDetails in enrolleeClasses)
                {
                    yield return new ClassBriefResponse
                    {
                        Id = classDetails.Id,
                        Name = classDetails.TeacherClassFK.Name
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


            return new CommitResult<EnrolleeDetailsResponse>
            {
                ResultType = ResultType.Ok,
                Value = new EnrolleeDetailsResponse
                {
                    EnrolleeName = limitedProfile.Value.FullName,
                    Classes = ClassMapper(),
                    Subjects = SubjectBriefMapper(),
                    ClassCounter = enrolleeClasses.Count()
                }
            };
        }
        else
        {
            return new CommitResult<EnrolleeDetailsResponse>
            {
                ResultType = ResultType.Empty,
            };
        }
    }
}
