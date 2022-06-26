using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query
{
    public class GetTeachersByStudentIdQueryHandler : IRequestHandler<GetTeachersByStudentIdQuery, ICommitResults<LimitedTeacherProfileResponse>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly IdentityClient _IdentityClient;
        private readonly CurriculumClient _CurriculumClient;
        private readonly JsonLocalizerManager _resourceJsonManager;

        public GetTeachersByStudentIdQueryHandler(TeacherDbContext dbContext,
                                                  IdentityClient identityClient,
                                                  CurriculumClient curriculumClient,
                                                  IWebHostEnvironment configuration,
                                                  IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _IdentityClient = identityClient;
            _CurriculumClient = curriculumClient;
            _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        }
        public async Task<ICommitResults<LimitedTeacherProfileResponse>?> Handle(GetTeachersByStudentIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<TeacherClass> TeacherClasses = await _dbContext.Set<ClassEnrollee>()
                                                           .Where(a => a.StudentId == request.StudentId && a.IsActive)
                                                           .Include(a => a.TeacherClassFK)
                                                           .Select(a => a.TeacherClassFK)
                                                           .ToListAsync(cancellationToken);

            if (!TeacherClasses.Any())
            {
                return ResultType.Ok.GetValueCommitResults(Array.Empty<LimitedTeacherProfileResponse>());
            }

            ICommitResults<LimitedProfileResponse>? teacherLimitedProfiles = await _IdentityClient.GetIdentityLimitedProfilesAsync(TeacherClasses.Select(a => a.TeacherId), cancellationToken);
            if (!teacherLimitedProfiles.IsSuccess)
            {
                return teacherLimitedProfiles.ResultType.GetValueCommitResults(Array.Empty<LimitedTeacherProfileResponse>(), teacherLimitedProfiles.ErrorCode, teacherLimitedProfiles.ErrorMessage);
            }

            ICommitResults<SubjectBriefResponse>? subjectBriefResponses = await _CurriculumClient.GetSubjectsBriefAsync(TeacherClasses.Select(a => a.SubjectId), cancellationToken);
            if (!subjectBriefResponses.IsSuccess)
            {
                return subjectBriefResponses.ResultType.GetValueCommitResults(Array.Empty<LimitedTeacherProfileResponse>(), subjectBriefResponses.ErrorCode, subjectBriefResponses.ErrorMessage);
            }

            IEnumerable<LimitedTeacherProfileResponse> Mapper()
            {
                foreach (TeacherClass teacherClass in TeacherClasses)
                {
                    SubjectBriefResponse subjectBrief = subjectBriefResponses.Value.FirstOrDefault(a => a.Id == teacherClass.SubjectId);
                    LimitedProfileResponse limitedProfile = teacherLimitedProfiles.Value.FirstOrDefault(a => a.UserId == teacherClass.TeacherId);
                    yield return new LimitedTeacherProfileResponse
                    {
                        SubjectId = teacherClass.SubjectId,
                        SubjectName = subjectBrief.Name,
                        UserId = limitedProfile.UserId,
                        AvatarImage = limitedProfile.AvatarImage,
                        FullName = limitedProfile.FullName,
                        GradeId = limitedProfile.GradeId,
                        GradeName = limitedProfile.GradeName,
                        IsPremium = limitedProfile.IsPremium,
                        NotificationToken = limitedProfile.NotificationToken
                    };
                }
            }

            return ResultType.Ok.GetValueCommitResults(Mapper());



        }
    }
}
