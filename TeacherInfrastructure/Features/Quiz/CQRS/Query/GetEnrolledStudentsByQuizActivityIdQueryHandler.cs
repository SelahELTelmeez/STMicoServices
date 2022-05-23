using SharedModule.DTO;
using TeacherDomain.Features.Quiz.CQRS.Query;
using TeacherDomain.Features.Quiz.DTO.Command;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.Trackers;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Quiz.CQRS.Query
{
    public class GetEnrolledStudentsByQuizActivityIdQueryHandler : IRequestHandler<GetEnrolledStudentsByQuizActivityIdQuery, CommitResults<EnrolledStudentQuizResponse>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly IdentityClient? _IdentityClient;
        private readonly StudentClient? _StudentClient;

        public GetEnrolledStudentsByQuizActivityIdQueryHandler(TeacherDbContext dbContext, IdentityClient? identityClient, StudentClient? studentClient)
        {
            _dbContext = dbContext;
            _IdentityClient = identityClient;
            _StudentClient = studentClient;
        }


        public async Task<CommitResults<EnrolledStudentQuizResponse>> Handle(GetEnrolledStudentsByQuizActivityIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<ClassEnrollee> classEnrollees = await _dbContext.Set<ClassEnrollee>()
                                   .Where(a => a.IsActive && a.ClassId.Equals(request.ClassId)).ToListAsync(cancellationToken);

            IEnumerable<TeacherQuizActivityTracker>? teacherQuizActivityTrackers = await _dbContext.Set<TeacherQuizActivityTracker>()
                                                                                                   .Where(a => a.TeacherQuizId == request.QuizId)
                                                                                                   .Include(a => a.TeacherQuizFK)
                                                                                                   .ToListAsync(cancellationToken);

            if (teacherQuizActivityTrackers == null)
            {
                return new CommitResults<EnrolledStudentQuizResponse>
                {
                    ResultType = ResultType.NotFound,
                    ErrorCode = "0000",
                    ErrorMessage = "X0000"
                };
            }
            CommitResults<LimitedProfileResponse>? profileResponses = await _IdentityClient.GetIdentityLimitedProfilesAsync(classEnrollees.Select(a => a.StudentId), cancellationToken);

            if (!profileResponses.IsSuccess)
            {
                return new CommitResults<EnrolledStudentQuizResponse>
                {
                    ErrorCode = profileResponses.ErrorCode,
                    ResultType = profileResponses.ResultType,
                    ErrorMessage = profileResponses.ErrorMessage
                };
            }


            CommitResults<StudentQuizResultResponse> quizResults = await _StudentClient.GetQuizzesResultAsync(classEnrollees.Select(a => a.StudentId), teacherQuizActivityTrackers.Where(a => a.ActivityStatus == TeacherEntites.Entities.Shared.ActivityStatus.Finished).Select(a => a.TeacherQuizFK.QuizId), cancellationToken);

            if (!quizResults.IsSuccess)
            {
                return new CommitResults<EnrolledStudentQuizResponse>
                {
                    ErrorCode = quizResults.ErrorCode,
                    ResultType = quizResults.ResultType,
                    ErrorMessage = quizResults.ErrorMessage
                };
            }

            IEnumerable<EnrolledStudentQuizResponse> Mapper()
            {
                foreach (ClassEnrollee studentEnroll in classEnrollees)
                {
                    LimitedProfileResponse profileResponse = profileResponses.Value.Single(a => a.UserId.Equals(studentEnroll.StudentId));
                    StudentQuizResultResponse studentQuizResultResponse = quizResults.Value.SingleOrDefault(a => a.StudentId == studentEnroll.StudentId);


                    yield return new EnrolledStudentQuizResponse
                    {
                        ClassId = studentEnroll.ClassId,
                        JoinedDate = studentEnroll.CreatedOn.GetValueOrDefault(),
                        StudentId = studentEnroll.StudentId,
                        AvatarUrl = profileResponse.AvatarImage,
                        GradeValue = profileResponse.GradeId,
                        GradeName = profileResponse.GradeName,
                        StudentName = profileResponse.FullName,
                        QuizScore = studentQuizResultResponse.QuizScore,
                        StudentScore = studentQuizResultResponse.StudentScore
                    };
                }
            }

            return new CommitResults<EnrolledStudentQuizResponse>
            {
                ResultType = ResultType.Ok,
                Value = Mapper()
            };
        }
    }
}
