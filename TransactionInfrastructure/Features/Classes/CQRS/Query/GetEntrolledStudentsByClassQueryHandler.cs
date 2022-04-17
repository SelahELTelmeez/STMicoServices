using Mapster;
using Microsoft.EntityFrameworkCore;
using TransactionDomain.Features.Classes.CQRS.Query;
using TransactionDomain.Features.Classes.DTO.Query;
using TransactionDomain.Features.Shared.DTO;
using TransactionEntites.Entities;
using TransactionEntites.Entities.TeacherClasses;
using TransactionInfrastructure.HttpClients;

namespace TransactionInfrastructure.Features.Classes.CQRS.Query
{
    public class GetEntrolledStudentsByClassQueryHandler : IRequestHandler<GetEntrolledStudentsByClassQuery, CommitResults<EnrolledStudentResponse>>
    {
        private readonly TrackerDbContext _dbContext;
        private readonly IdentityClient? _IdentityClient;

        public GetEntrolledStudentsByClassQueryHandler(TrackerDbContext dbContext, IdentityClient? identityClient)
        {
            _dbContext = dbContext;
            _IdentityClient = identityClient;
        }
        public async Task<CommitResults<EnrolledStudentResponse>> Handle(GetEntrolledStudentsByClassQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<StudentEnrollClass> studentEnrolls = await _dbContext.Set<StudentEnrollClass>()
                .Where(a => a.IsActive && a.ClassId.Equals(request.ClassId)).ToListAsync(cancellationToken);

            CommitResults<LimitedProfileResponse>? profileResponses = await _IdentityClient.GetIdentityLimitedProfilesAsync(studentEnrolls.Select(a => a.StudentId), cancellationToken);

            if (!profileResponses.IsSuccess)
            {
                return profileResponses.Adapt<CommitResults<EnrolledStudentResponse>>();
            }

            IEnumerable<EnrolledStudentResponse> Mapper()
            {
                foreach (StudentEnrollClass studentEnroll in studentEnrolls)
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
}
