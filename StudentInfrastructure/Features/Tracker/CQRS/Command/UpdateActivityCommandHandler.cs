using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.IdentityScores.IdentitySubjectScore.CQRS;
using StudentDomain.Features.IdentityScores.IdentitySubjectScore.DTO;
using StudentDomain.Features.Tracker.CQRS.Command;
using StudentDomain.Models;
using StudentDomain.Services;
using StudentEntities.Entities.Rewards;
using StudentEntities.Entities.Shared;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.Tracker.CQRS.Command;
public class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand, ICommitResult>
{
    private readonly StudentDbContext _dbContext;
    private readonly string? _userId;
    private readonly CurriculumClient _CurriculumClient;
    private readonly IdentityClient _identityClient;
    private readonly IMediator _mediator;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly INotificationService _notificationService;
    private readonly IHttpClientFactory _httpClientFactory;

    public UpdateActivityCommandHandler(StudentDbContext dbContext,
                                        CurriculumClient curriculumClient,
                                        IWebHostEnvironment configuration,
                                        IHttpContextAccessor httpContextAccessor,
                                        IMediator mediator,
                                        IHttpClientFactory httpClientFactory,
                                        INotificationService notificationService,
                                        IdentityClient identityClient)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _mediator = mediator;
        _CurriculumClient = curriculumClient;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _notificationService = notificationService;
        _httpClientFactory = httpClientFactory;
        _identityClient = identityClient;
    }

    public async Task<ICommitResult> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
    {
        // =========== update student Activity ================ Check Here
        ActivityTracker? studentActivityTracker = await _dbContext.Set<ActivityTracker>()
                                                                  .FirstOrDefaultAsync(a => a.Id.Equals(request.ActivityRequest.ActivityId), cancellationToken);
        if (studentActivityTracker == null)
        {
            return ResultType.NotFound.GetCommitResult("XSTU0002", _resourceJsonManager["XSTU0002"]);
        }

        if (request.ActivityRequest.Code != 0)
        {
            if (request.ActivityRequest.Code <= studentActivityTracker.Code)
            {
                return ResultType.Ok.GetCommitResult();
            }
        }
        else
        {
            if (request.ActivityRequest.StudentPoints <= studentActivityTracker.StudentPoints)
            {
                return ResultType.Ok.GetCommitResult();
            }
        }

        studentActivityTracker.Code = request.ActivityRequest.Code;
        studentActivityTracker.LearningDurationInSec = request.ActivityRequest.LearningDurationInSec;
        studentActivityTracker.StudentPoints = request.ActivityRequest.StudentPoints;
        studentActivityTracker.CreatedOn = DateTime.UtcNow;
        studentActivityTracker.LearningObjectAsJson = request.ActivityRequest.LearningObjectAsJson;

        _dbContext.Set<ActivityTracker>().Update(studentActivityTracker);

        await _dbContext.SaveChangesAsync(cancellationToken);

        // =========== Get progress and set student rewards ================
        await SetStudentRewards(studentActivityTracker.SubjectId, cancellationToken);

        // =========== Get Response Id ================
        return ResultType.Ok.GetCommitResult();
    }

    // =========== Get progress of subject and set student rewards ================
    private async Task SetStudentRewards(string subjectId, CancellationToken cancellationToken)
    {
        // =========== get subject information Details================
        ICommitResult<SubjectBriefResponse>? subjectDetails = await _CurriculumClient.GetSubjectBriefAsync(subjectId, cancellationToken);

        //======= get the heighest MedalLevel of student to this subject before this activity update ==================
        int LevelBeforeActivity = 0;

        Reward? reward = await _dbContext.Set<Reward>()
                                         .Where(a => a.StudentId.Equals(_userId) && a.SubjectId.Equals(subjectId) && a.Type == 1)
                                         .OrderByDescending(a => a.MedalLevel)
                                         .FirstOrDefaultAsync(cancellationToken);

        if (reward != null)
            LevelBeforeActivity = (int)reward.MedalLevel;

        // ===========Calculate Progress for subject After Activity================
        // =========== Get sumation of student point in subject ================
        ICommitResult<IdentitySubjectScoreResponse> subjectScore = await _mediator.Send(new GetIdentitySubjectScoreQuery(subjectId), cancellationToken);


        int progresslevel = 0;

        if (subjectScore.Value.StudentScore != 0 || subjectScore.Value.SubjectScore != 0)
        {
            progresslevel = Convert.ToInt32(subjectScore.Value.StudentScore * 100 / subjectScore.Value.SubjectScore);
        }

        //type 1 equals for a subject

        RewardDetails? rewardDetails = GetRewardDetails(1, progresslevel, subjectDetails.Value);

        // incase of reward wasn't found before, and the reward details got a value, then add that value into a new reward.
        if (reward == null && rewardDetails != null)
        {
            // =========== Save rewards Changes================
            Reward rewardChanges = new Reward
            {
                Type = 1,  //reward to specific subject
                MedalLevel = (MedalLevel)rewardDetails.Id,
                SubjectId = subjectDetails.Value.Id,
                StudentId = _userId,
                IsNew = true,
                Title = rewardDetails.Title,
                Description = rewardDetails.Description,
                Image = rewardDetails.Image
            };

            _dbContext.Set<Reward>().Add(rewardChanges);

            await _dbContext.SaveChangesAsync(cancellationToken);
        }
        // =========== Get studen Level After this Activity ================
        // =========== Get student rewrad of subject ===================

        if (reward != null && rewardDetails != null)
        {
            //====================Insert Student Reward after check medal level before and after update===============================
            if (rewardDetails.Id > LevelBeforeActivity)
            {
                // =========== Update the current reward to the new achievement ================
                reward.MedalLevel = (MedalLevel)rewardDetails.Id;

                _dbContext.Set<Reward>().Update(reward);

                //====================After Update Activity check How many medals this Student have===============================
                // =================== if the student readed 3 medal of the same medal type, he gots a reward of the medal, just this happens once for each medal============

                MedalLevel medalLevel = (MedalLevel)rewardDetails.Id;

                int MedalNo = await _dbContext.Set<Reward>()
                                          .Where(a => a.StudentId.Equals(_userId) && a.MedalLevel.Equals(medalLevel))
                                          .CountAsync(cancellationToken);

                if (MedalNo == 3)
                {
                    // =========== Getstudent  rewrad of all subjects ================
                    rewardDetails = GetRewardDetails(2, progresslevel);

                    //====================Insert Student reward of all subject =============================
                    // =========== Save rewards Changes================
                    _dbContext.Set<Reward>().Add(new Reward
                    {
                        Type = 2, //reward to all subjects for this student
                        MedalLevel = medalLevel,
                        StudentId = _userId,
                        IsNew = true,
                        Title = rewardDetails.Title,
                        Description = rewardDetails.Description,
                        Image = rewardDetails.Image
                    });


                    ICommitResult<LimitedProfileResponse>? limitedProfileResponse = await _identityClient.GetIdentityLimitedProfileAsync(_userId, cancellationToken);

                    if (limitedProfileResponse.IsSuccess)
                    {
                        await _notificationService.PushNotificationAsync(_httpClientFactory.CreateClient("FCMClient"),
                        new NotificationModel
                        {
                            Token = limitedProfileResponse.Value.NotificationToken,
                            Title = rewardDetails.Title,
                            Body = rewardDetails.Description,
                            Type = 10, // this type for reward
                        }, cancellationToken);
                    }
                }

                await _dbContext.SaveChangesAsync(cancellationToken);
            }
        }
    }

    // =========== Get student  rewrad of subject or all subject and return rewardDetails model  ================
    private static RewardDetails? GetRewardDetails(int type, double progresslevel, SubjectBriefResponse subject = default)
    {
        if (progresslevel >= 20 && progresslevel < 40)
            return new RewardDetails
            {
                Id = 1,
                Title = type == 1 ? "ميدالية برنزية" : "برنز",
                Description = type == 1 ? $"{subject.Name} مبروك لقد حصلت على الميدالية البرنزية في مادة" : "مبروك لقد وصلت إلى المنصة البرنزية ",
                Image = string.Concat(subject.Id.AsSpan(0, 4), "Pronz.png")
            };
        else if (progresslevel >= 40 && progresslevel < 60)
            return new RewardDetails
            {
                Id = 2,
                Title = type == 1 ? "ميدالية فضية" : "فضة",
                Description = type == 1 ? $"{subject.Name} مبروك لقد حصلت على الميدالية الفضية في مادة" : "مبروك لقد وصلت إلى المنصة الفضية ",
                Image = string.Concat(subject.Id.AsSpan(0, 4), "Silver.png")
            };
        else if (progresslevel >= 60 && progresslevel < 80)
            return new RewardDetails
            {
                Id = 3,
                Title = type == 1 ? "ميدالية ذهبية" : "ذهب",
                Description = type == 1 ? $"{subject.Name} مبروك لقد حصلت على الميدالية الذهبية في مادة" : "مبروك لقد وصلت إلى المنصة الذهبية ",
                Image = string.Concat(subject.Id.AsSpan(0, 4), "Gold.png")
            };
        else if (progresslevel >= 80 && progresslevel < 95)
            return new RewardDetails
            {
                Id = 4,
                Title = type == 1 ? "ميدالية بلاتينية" : "بلاتين",
                Description = type == 1 ? $"{subject.Name} مبروك لقد حصلت على الميدالية البلاتينية في مادة" : "مبروك لقد وصلت إلى المنصة البلاتينية ",
                Image = string.Concat(subject.Id.AsSpan(0, 4), "platin.png")
            };
        else if (progresslevel >= 95)
            return new RewardDetails
            {
                Id = 5,
                Title = type == 1 ? "الكأس" : "البطولة",
                Description = type == 1 ? $"{subject.Name} مبروك لقد حصلت على الكأس في مادة" : "مبروك لقد وصلت إلى منصة البطولة ",
                Image = string.Concat(subject.Id.AsSpan(0, 4), "Cub.png")
            };
        else return default;
    }
}