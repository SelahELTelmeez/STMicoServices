using SharedModule.DTO;
using TeacherDomain.Features.Assignment.CQRS.Query;
using TeacherDomain.Features.Assignment.DTO.Query;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.Trackers;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Assignment.CQRS.Query
{
    public class GetEnrolledStudentsByAssignmentActivityIdQueryHandler : IRequestHandler<GetEnrolledStudentsByAssignmentActivityIdQuery, ICommitResults<EnrolledStudentAssignmentResponse>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly IdentityClient? _IdentityClient;

        public GetEnrolledStudentsByAssignmentActivityIdQueryHandler(TeacherDbContext dbContext, IdentityClient? identityClient)
        {
            _dbContext = dbContext;
            _IdentityClient = identityClient;
        }


        public async Task<ICommitResults<EnrolledStudentAssignmentResponse>> Handle(GetEnrolledStudentsByAssignmentActivityIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<ClassEnrollee> classEnrollees = await _dbContext.Set<ClassEnrollee>()
                        .Where(a => a.IsActive && a.ClassId.Equals(request.ClassId)).ToListAsync(cancellationToken);

            IEnumerable<TeacherAssignmentActivityTracker>? teacherAssignmentActivityTrackers = await _dbContext.Set<TeacherAssignmentActivityTracker>().Where(a => a.TeacherAssignmentId == request.AssingmentId).ToListAsync(cancellationToken);

            if (teacherAssignmentActivityTrackers == null)
            {
                return ResultType.NotFound.GetValueCommitResults(Array.Empty<EnrolledStudentAssignmentResponse>(),"0000","X0000");
            }
            ICommitResults<LimitedProfileResponse>? profileResponses = await _IdentityClient.GetIdentityLimitedProfilesAsync(classEnrollees.Select(a => a.StudentId), cancellationToken);

            if (!profileResponses.IsSuccess)
            {
                return new CommitResults<EnrolledStudentAssignmentResponse>
                {
                    ErrorCode = profileResponses.ErrorCode,
                    ResultType = profileResponses.ResultType,
                    ErrorMessage = profileResponses.ErrorMessage
                };
            }


            IEnumerable<EnrolledStudentAssignmentResponse> Mapper()
            {
                foreach (ClassEnrollee studentEnroll in classEnrollees)
                {
                    LimitedProfileResponse profileResponse = profileResponses.Value.Single(a => a.UserId.Equals(studentEnroll.StudentId));
                    TeacherAssignmentActivityTracker? teacherAssignmentActivityTracker = teacherAssignmentActivityTrackers.SingleOrDefault(a => a.StudentId == studentEnroll.StudentId && a.ActivityStatus == TeacherEntites.Entities.Shared.ActivityStatus.Finished);

                    yield return new EnrolledStudentAssignmentResponse
                    {
                        ClassId = studentEnroll.ClassId,
                        JoinedDate = studentEnroll.CreatedOn.GetValueOrDefault(),
                        StudentId = studentEnroll.StudentId,
                        AvatarUrl = profileResponse.AvatarImage,
                        GradeValue = profileResponse.GradeId,
                        GradeName = profileResponse.GradeName,
                        StudentName = profileResponse.FullName,
                        ReplyComment = teacherAssignmentActivityTracker?.ReplyComment,
                        ReplyAttachmentId = teacherAssignmentActivityTracker?.ReplyAttachmentId
                    };
                }
            }

            return ResultType.Ok.GetValueCommitResults(Mapper());
        }
    }
}
