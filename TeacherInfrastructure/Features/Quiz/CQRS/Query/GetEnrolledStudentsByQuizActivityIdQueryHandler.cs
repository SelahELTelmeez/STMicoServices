using SharedModule.DTO;
using TeacherDomain.Features.Quiz.CQRS.Query;
using TeacherDomain.Features.Quiz.DTO.Command;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.Trackers;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Quiz.CQRS.Query
{
    public class GetEnrolledStudentsByQuizActivityIdQueryHandler : IRequestHandler<GetEnrolledStudentsByQuizActivityIdQuery, ICommitResults<EnrolledStudentQuizResponse>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly IdentityClient? _IdentityClient;
        private readonly StudentClient? _StudentClient;
        private readonly JsonLocalizerManager _resourceJsonManager;

        public GetEnrolledStudentsByQuizActivityIdQueryHandler(TeacherDbContext dbContext,
                                                               IdentityClient? identityClient,
                                                               StudentClient? studentClient,
                                                               IHttpContextAccessor httpContextAccessor,
                                                               IWebHostEnvironment configuration)
        {
            _dbContext = dbContext;
            _IdentityClient = identityClient;
            _StudentClient = studentClient;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());

        }


        public async Task<ICommitResults<EnrolledStudentQuizResponse>> Handle(GetEnrolledStudentsByQuizActivityIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<ClassEnrollee> classEnrollees = await _dbContext.Set<ClassEnrollee>()
                                   .Where(a => a.IsActive && a.ClassId.Equals(request.ClassId)).ToListAsync(cancellationToken);


            if (!classEnrollees.Any())
            {
                return ResultType.Ok.GetValueCommitResults(Array.Empty<EnrolledStudentQuizResponse>());
            }


            IEnumerable<TeacherQuizActivityTracker>? teacherQuizActivityTrackers = await _dbContext.Set<TeacherQuizActivityTracker>()
                                                                                                   .Where(a => a.TeacherQuizId == request.QuizId)
                                                                                                   .Include(a => a.TeacherQuizFK)
                                                                                                   .ToListAsync(cancellationToken);

            if (teacherQuizActivityTrackers == null)
            {
                return ResultType.Ok.GetValueCommitResults(Array.Empty<EnrolledStudentQuizResponse>());
            }

            ICommitResults<LimitedProfileResponse>? profileResponses = await _IdentityClient.GetIdentityLimitedProfilesAsync(classEnrollees.Select(a => a.StudentId), cancellationToken);

            if (!profileResponses.IsSuccess)
            {
                return profileResponses.ResultType.GetValueCommitResults(Array.Empty<EnrolledStudentQuizResponse>(), profileResponses.ErrorCode, profileResponses.ErrorMessage);
            }

            ICommitResults<StudentQuizResultResponse> quizResults = await _StudentClient.GetQuizzesResultAsync(classEnrollees.Select(a => a.StudentId), teacherQuizActivityTrackers.Where(a => a.ActivityStatus == TeacherEntites.Entities.Shared.ActivityStatus.Finished).Select(a => a.TeacherQuizFK.QuizId), cancellationToken);

            if (!quizResults.IsSuccess)
            {
                return quizResults.ResultType.GetValueCommitResults(Array.Empty<EnrolledStudentQuizResponse>(), quizResults.ErrorCode, quizResults.ErrorMessage);
            }

            IEnumerable<EnrolledStudentQuizResponse> Mapper()
            {
                foreach (ClassEnrollee studentEnroll in classEnrollees)
                {
                    LimitedProfileResponse? profileResponse = profileResponses.Value.FirstOrDefault(a => a.UserId.Equals(studentEnroll.StudentId));
                    StudentQuizResultResponse? studentQuizResultResponse = quizResults.Value.FirstOrDefault(a => a.StudentId == studentEnroll.StudentId);
                    yield return new EnrolledStudentQuizResponse
                    {
                        ClassId = studentEnroll.ClassId,
                        JoinedDate = studentEnroll.CreatedOn.GetValueOrDefault(),
                        StudentId = studentEnroll.StudentId,
                        AvatarUrl = profileResponse.AvatarImage,
                        GradeValue = profileResponse.GradeId,
                        GradeName = profileResponse.GradeName,
                        StudentName = profileResponse.FullName,
                        QuizScore = studentQuizResultResponse?.QuizScore,
                        StudentScore = studentQuizResultResponse?.StudentScore
                    };
                }
            }

            return ResultType.Ok.GetValueCommitResults(Mapper());
        }
    }
}
