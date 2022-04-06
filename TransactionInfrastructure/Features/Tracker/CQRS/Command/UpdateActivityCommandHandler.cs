using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using ResultHandler;
using TransactionDomain.Features.Tracker.CQRS.Command;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Rewards;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Tracker.CQRS.Command
{
    public class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand, CommitResult>
    {
        private readonly StudentTrackerDbContext _dbContext;
        private readonly Guid? _userId;
        private readonly HttpClient _CurriculumClient;

        public UpdateActivityCommandHandler(StudentTrackerDbContext dbContext, IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
            _CurriculumClient = factory.CreateClient("CurriculumClient");
        }
        public async Task<CommitResult> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
        {
            // =========== update student Activity ================
            var studentActivityTracker = _dbContext.Set<StudentActivityTracker>().Update(request.ActivityRequest.Adapt<StudentActivityTracker>());
            await _dbContext.SaveChangesAsync(cancellationToken);

            // =========== get Lesson information Details================
            // CommitResult<Lesson>? LessonDetails = await _CurriculumClient.GetFromJsonAsync<CommitResult<Lesson>>($"/Curriculum/GetLessonDetails?LessonId={request.ActivityRequest.LessonId}");

            //======= Calculate the user level of this subject before this activity update ==================

            int LevelBeforeActivity = 0;

            //var MedalLevel = await _dbContext.Set<Reward>()
            //                                .Where(a => a.StudentIdentityId.Equals(_userId)
            //                                            && a.Grade.Equals(LessonDetails.Value.UnitFK.SubjectFK.Grade)
            //                                            && a.SubjectId.Equals(LessonDetails.Value.UnitFK.SubjectId))
            //                                .OrderByDescending(a => a.MedalLevel)
            //                                .Select(a => a.MedalLevel)
            //                                .ToListAsync(cancellationToken);
            //if (MedalLevel.Count() > 0)
            //    LevelBeforeActivity = (int)MedalLevel.FirstOrDefault();
            //// =========== Get student Lesson Progress ================

            //// =========== Get sumation of student point in all Lesson activity ================

            //var totalStudentPointInLesson = await _dbContext.Set<StudentActivityTracker>()
            //                                .Where(a => a.ClipFK.LessonId.Equals(request.ActivityRequest.LessonId))
            //                                .Select(a => a.StudentPoints).SumAsync();

            //var progress = (totalStudentPointInLesson / LessonDetails.Value.Ponits) * 100;

            // ===========Calculate Progress for subject After Activity================
            // =========== Get sumation of student point in subject ================

            //var totalStudentPointInSubject = await _dbContext.Set<StudentActivityTracker>()
            //                                .Where(a => a.ClipFK.LessonFK.UnitFK.SubjectId.Equals(LessonDetails.Value.UnitFK.SubjectId))
            //                                .Select(a => a.StudentPoints).SumAsync();

            //var totalPointInSubject = await _dbContext.Set<Lesson>()
            //                                .Where(a => a.UnitFK.SubjectId.Equals(LessonDetails.Value.UnitFK.SubjectId))
            //                                .Select(a => a.Ponits).SumAsync();

            var progresslevel = 0;// Convert.ToInt32(totalStudentPointInSubject / totalPointInSubject) * 100;
            // =========== Getstudent  Level After this Activity ================

            int LevelAfterActivity = 0;

            if (progresslevel >= 20 && progresslevel < 40)
                LevelAfterActivity = 1;
            else if (progresslevel >= 40 && progresslevel < 60)
                LevelAfterActivity = 2;
            else if (progresslevel >= 60 && progresslevel < 80)
                LevelAfterActivity = 3;
            else if (progresslevel >= 80 && progresslevel < 95)
                LevelAfterActivity = 4;
            else if (progresslevel >= 95)
                LevelAfterActivity = 5;

            //====================Insert Student Reward===============================
            if (LevelAfterActivity > LevelBeforeActivity)
            {
                Reward reward = new Reward();
                //reward.myDate = System.DateTime.Now;
                //reward.Type = 1;
                //reward.MedalLevel = (MedalLevel)LevelAfterActivity;
                //reward.Grade = LessonDetails.Value.UnitFK.SubjectFK.Grade;
                //reward.Term = LessonDetails.Value.UnitFK.SubjectFK.Term;
                //reward.SubjectId = LessonDetails.Value.UnitFK.SubjectId;
                //reward.StudentIdentityId = (Guid)_userId;
                //reward.IsNew = true;
                //reward.Image = LessonDetails.Value.UnitFK.SubjectId.Substring(0, 4);
                //switch (LevelAfterActivity)
                //{
                //    case 1:
                //        reward.Title = "ميدالية برنزية";
                //        reward.Description = "مبروك لقد حصلت على الميدالية البرنزية في مادة " + reward.SubjectId;
                //        reward.Image += "Pronz.png";
                //        break;
                //    case 2:
                //        reward.Title = "ميدالية فضية";
                //        reward.Description = "مبروك لقد حصلت على الميدالية الفضية في مادة " + reward.Subject;
                //        reward.Image += "Silver.png";
                //        break;
                //    case 3:
                //        reward.Title = "ميدالية ذهبية";
                //        reward.Description = "مبروك لقد حصلت على الميدالية الذهبية في مادة " + reward.Subject;
                //        reward.Image += "Gold.png";
                //        break;
                //    case 4:
                //        reward.Title = "ميدالية بلاتينية";
                //        reward.Description = "مبروك لقد حصلت على الميدالية البلاتينية في مادة " + reward.Subject;
                //        reward.Image += "platin.png";
                //        break;
                //    case 5:
                //        reward.Title = "الكأس";
                //        reward.Description = "مبروك لقد حصلت على الكأس في مادة " + reward.Subject;
                //        reward.Image += "Cub.png";
                //        break;
                //    default:
                //        break;
                //}
            }
            // =========== Get Response ActivityId ================
            return new CommitResult
            {
                ResultType = ResultType.Ok, // : ResultType.PartialOk,
            };
        }
    }
}
