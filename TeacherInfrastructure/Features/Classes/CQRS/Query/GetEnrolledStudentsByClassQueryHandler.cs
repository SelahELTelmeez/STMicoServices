using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherDomain.Features.Classes.DTO.Query;
using TeacherEntites.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query;
public class GetEnrolledStudentsByClassQueryHandler : IRequestHandler<GetEnrolledStudentsByClassQuery, CommitResults<EnrolledStudentResponse>>
{
    private readonly TeacherDbContext _dbContext;
    private readonly IdentityClient? _IdentityClient;

    public GetEnrolledStudentsByClassQueryHandler(TeacherDbContext dbContext, IdentityClient? identityClient)
    {
        _dbContext = dbContext;
        _IdentityClient = identityClient;
    }
    public async Task<CommitResults<EnrolledStudentResponse>> Handle(GetEnrolledStudentsByClassQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<ClassEnrollee> classEnrollees = await _dbContext.Set<ClassEnrollee>()
            .Where(a => a.IsActive && a.ClassId.Equals(request.ClassId)).ToListAsync(cancellationToken);

        CommitResults<LimitedProfileResponse>? profileResponses = await _IdentityClient.GetIdentityLimitedProfilesAsync(classEnrollees.Select(a => a.StudentId), cancellationToken);

        if (!profileResponses.IsSuccess)
        {
            return profileResponses.Adapt<CommitResults<EnrolledStudentResponse>>();
        }

        IEnumerable<EnrolledStudentResponse> Mapper()
        {
            foreach (ClassEnrollee studentEnroll in classEnrollees)
            {
                LimitedProfileResponse profileResponse = profileResponses.Value.Single(a => a.UserId.Equals(studentEnroll.StudentId));

                yield return new EnrolledStudentResponse
                {
                    ClassId = studentEnroll.ClassId,
                    JoinedDate = studentEnroll.CreatedOn.GetValueOrDefault(),
                    StudentId = studentEnroll.StudentId,
                    AvatarUrl = profileResponse.AvatarImage,
                    GradeValue = profileResponse.GradeId,
                    GradeName = profileResponse.GradeName,
                    StudentName = profileResponse.FullName,
                };
            }
        }

        return new CommitResults<EnrolledStudentResponse>
        {
            ResultType = ResultType.Ok,
            Value = Mapper()
        };
    }
}