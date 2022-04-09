using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using TransactionDomain.Features.IdentityScores.IdentitySubjectScore.CQRS;
using TransactionDomain.Features.IdentityScores.IdentitySubjectScore.DTO;
using TransactionDomain.Features.Tracker.CQRS.Command;
using TransactionDomain.Models;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Rewards;
using TransactionEntites.Entities.Shared;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Features.Tracker.DTO.Command;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Tracker.CQRS.Command;
public class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand, CommitResult>
{
    private readonly TrackerDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly HttpClient _CurriculumClient;
    private readonly IMediator _mediator;

    public UpdateActivityCommandHandler(TrackerDbContext dbContext, IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, IMediator mediator)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _CurriculumClient = factory.CreateClient("CurriculumClient");
        _mediator = mediator;
    }

    public async Task<CommitResult> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
    {
        // =========== update student Activity ================ Check Here
        StudentActivityTracker? studentActivityTracker = await _dbContext.Set<StudentActivityTracker>().FindAsync(request.ActivityRequest.ActivityId);

        if (studentActivityTracker == null)
        {
            return new CommitResult
            {
                ErrorCode = "X000X",
                ErrorMessage = "X000X"
            };
        }

        studentActivityTracker.Code = request.ActivityRequest.Code;
        studentActivityTracker.LearningDurationInSec = request.ActivityRequest.LearningDurationInSec;
        studentActivityTracker.StudentPoints = request.ActivityRequest.StudentPoints;

        _dbContext.Set<StudentActivityTracker>().Update(studentActivityTracker);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========== Get progress and set student rewards ================

        await SetStudentRewards(request, cancellationToken);

        // =========== Get Response ActivityId ================
        return new CommitResult
        {
            ResultType = ResultType.Ok, // : ResultType.PartialOk,
        };
    }

    // =========== Get progress of subject and set student rewards ================
    private async Task SetStudentRewards(UpdateActivityCommand request, CancellationToken cancellationToken)
    {
        // =========== get subject information Details================
        CommitResult<SubjectDetailsResponse>? subjectDetails = await _CurriculumClient.GetFromJsonAsync<CommitResult<SubjectDetailsResponse>>($"/Curriculum/GetSubjectDetailsQuery?SubjectId={request.ActivityRequest.SubjectId}");

        //    //======= get the heighest MedalLevel of student to this subject before this activity update ==================
        int LevelBeforeActivity = 0;

        Reward? reward = await _dbContext.Set<Reward>()
                                       .Where(a => a.StudentIdentityId.Equals(_userId) && a.SubjectId.Equals(request.ActivityRequest.SubjectId))
                                       .OrderByDescending(a => a.MedalLevel)
                                       .FirstOrDefaultAsync(cancellationToken);

        if (reward != null)
            LevelBeforeActivity = (int)reward.MedalLevel;

        // ===========Calculate Progress for subject After Activity================
        // =========== Get sumation of student point in subject ================

        CommitResult<IdentitySubjectScoreResponse> subjectScore = await _mediator.Send(new GetIdentitySubjectScoreQuery(request.ActivityRequest.SubjectId.ToString()), cancellationToken);

        float progresslevel = subjectScore.Value.Progress * 100;

        // =========== Getstudent  Level After this Activity ================
        // =========== Getstudent  rewrad of subject ================
        RewardDetails RewardDetails = GetMedalType(1, progresslevel);

        //    float progresslevel = subjectScore.Result.Value.Progress * 100;
        //    // =========== Getstudent  Level After this Activity ================

        //    // =========== Getstudent  rewrad of subject ================
        //    RewardDetails RewardDetails = getMedalType(1, progresslevel);

        //====================Insert Student Reward after check medal level before and after update===============================
        if (RewardDetails.Id > LevelBeforeActivity)
        {
            // =========== Save rewards Changes================
            _dbContext.Set<Reward>().Add(new Reward
            {
                Type = 1,  //reward to specific subject
                MedalLevel = (MedalLevel)RewardDetails.Id,
                SubjectId = subjectDetails.Value.Id.ToString(),
                StudentIdentityId = _userId.GetValueOrDefault(),
                IsNew = true,
                Title = RewardDetails.Title,
                Description = RewardDetails.Description + subjectDetails.Value.Name,
                Image = subjectDetails.Value.Id.ToString().Substring(0, 4) + RewardDetails.Image
            });
            await _dbContext.SaveChangesAsync(cancellationToken);

            //====================After Update Activity check How many medals this Student have===============================
            int MedalNo = await _dbContext.Set<Reward>()
                                      .Where(a => a.StudentIdentityId.Equals(_userId) && a.MedalLevel.Equals((MedalLevel)RewardDetails.Id))
                                      .CountAsync(cancellationToken);

            if (MedalNo == 3)
            {
                // =========== Getstudent  rewrad of subject ================
                RewardDetails = GetMedalType(2, progresslevel);
                //====================Insert Student reward of all subject =============================
                // =========== Save rewards Changes================
                _dbContext.Set<Reward>().Add(new Reward
                {
                    Type = 2, //reward to all subjects for this student
                    MedalLevel = (MedalLevel)RewardDetails.Id,
                    StudentIdentityId = _userId.GetValueOrDefault(),
                    IsNew = true,
                    Title = RewardDetails.Title,
                    Description = RewardDetails.Description,
                    Image = RewardDetails.Image
                });
                await _dbContext.SaveChangesAsync(cancellationToken);
                // =========== Get student  rewrad of subject or all subject and return RewardDetails model  ================
            }
        }
    }
    private static RewardDetails GetMedalType(int type, float progresslevel)
    {
        if (progresslevel >= 20 && progresslevel < 40)
            return new RewardDetails
            {
                Id = 1,
                Title = type == 1 ? "ميدالية برنزية" : "برنز",
                Description = type == 1 ? "مبروك لقد حصلت على الميدالية البرنزية في مادة " : "مبروك لقد وصلت إلى المنصة البرنزية ",
                Image = type == 1 ? "Pronz.png" : ""
            };
        else if (progresslevel >= 40 && progresslevel < 60)
            return new RewardDetails
            {
                Id = 2,
                Title = type == 1 ? "ميدالية فضية" : "فضة",
                Description = type == 1 ? "مبروك لقد حصلت على الميدالية الفضية في مادة " : "مبروك لقد وصلت إلى المنصة الفضية ",
                Image = type == 1 ? "Silver.png" : ""
            };
        else if (progresslevel >= 60 && progresslevel < 80)
            return new RewardDetails
            {
                Id = 3,
                Title = type == 1 ? "ميدالية ذهبية" : "ذهب",
                Description = type == 1 ? "مبروك لقد حصلت على الميدالية الذهبية في مادة " : "مبروك لقد وصلت إلى المنصة الذهبية ",
                Image = type == 1 ? "Gold.png" : ""
            };
        else if (progresslevel >= 80 && progresslevel < 95)
            return new RewardDetails
            {
                Id = 4,
                Title = type == 1 ? "ميدالية بلاتينية" : "بلاتين",
                Description = type == 1 ? "مبروك لقد حصلت على الميدالية البلاتينية في مادة " : "مبروك لقد وصلت إلى المنصة البلاتينية ",
                Image = type == 1 ? "platin.png" : ""
            };
        else
            return new RewardDetails
            {
                Id = 5,
                Title = type == 1 ? "الكأس" : "البطولة",
                Description = type == 1 ? "مبروك لقد حصلت على الكأس في مادة " : "مبروك لقد وصلت إلى منصة البطولة ",
                Image = type == 1 ? "Cub.png" : ""
            };
    }
}