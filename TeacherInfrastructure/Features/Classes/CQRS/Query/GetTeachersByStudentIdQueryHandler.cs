using SharedModule.DTO;
using TeacherDomain.Features.Classes.CQRS.Query;
using TeacherEntites.Entities.TeacherClasses;
using TeacherEntities.Entities.TeacherClasses;
using TeacherInfrastructure.HttpClients;

namespace TeacherInfrastructure.Features.Classes.CQRS.Query
{
    public class GetTeachersByStudentIdQueryHandler : IRequestHandler<GetTeachersByStudentIdQuery, CommitResults<LimitedTeacherProfileResponse>>
    {
        private readonly TeacherDbContext _dbContext;
        private readonly IdentityClient _IdentityClient;
        private readonly CurriculumClient _CurriculumClient;
        public GetTeachersByStudentIdQueryHandler(TeacherDbContext dbContext, IdentityClient identityClient, CurriculumClient curriculumClient)
        {
            _dbContext = dbContext;
            _IdentityClient = identityClient;
            _CurriculumClient = curriculumClient;
        }
        public async Task<CommitResults<LimitedTeacherProfileResponse>?> Handle(GetTeachersByStudentIdQuery request, CancellationToken cancellationToken)
        {
            IEnumerable<TeacherClass> TeacherClasses = await _dbContext.Set<ClassEnrollee>()
                                                           .Where(a => a.StudentId == request.StudentId && a.IsActive)
                                                           .Include(a => a.TeacherClassFK)
                                                           .Select(a => a.TeacherClassFK)
                                                           .ToListAsync(cancellationToken);
            if (TeacherClasses.Any())
            {
                CommitResults<LimitedProfileResponse>? teacherLimitedProfiles = await _IdentityClient.GetIdentityLimitedProfilesAsync(TeacherClasses.Select(a => a.TeacherId), cancellationToken);
                if (!teacherLimitedProfiles.IsSuccess)
                {
                    return new CommitResults<LimitedTeacherProfileResponse>
                    {
                        ErrorCode = teacherLimitedProfiles.ErrorCode,
                        ResultType = teacherLimitedProfiles.ResultType,
                        ErrorMessage = teacherLimitedProfiles.ErrorMessage
                    };
                }

                CommitResults<SubjectBriefResponse>? subjectBriefResponses = await _CurriculumClient.GetSubjectsBriefAsync(TeacherClasses.Select(a => a.SubjectId), cancellationToken);
                if (!subjectBriefResponses.IsSuccess)
                {
                    return new CommitResults<LimitedTeacherProfileResponse>
                    {
                        ErrorCode = subjectBriefResponses.ErrorCode,
                        ResultType = subjectBriefResponses.ResultType,
                        ErrorMessage = subjectBriefResponses.ErrorMessage
                    };
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

                return new CommitResults<LimitedTeacherProfileResponse>
                {
                    Value = Mapper(),
                    ResultType = ResultType.Ok
                };
            }
            else
            {
                return new CommitResults<LimitedTeacherProfileResponse>
                {
                    ResultType = ResultType.Empty,
                    Value = Array.Empty<LimitedTeacherProfileResponse>()
                };
            }
        }
    }
}
