using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.IdentityScores.IdentitySubjectScore.CQRS;
using StudentDomain.Features.IdentityScores.IdentitySubjectScore.DTO;
using StudentDomain.Features.Tracker.CQRS.Command;
using StudentDomain.Models;
using StudentEntities.Entities.Rewards;
using StudentEntities.Entities.Shared;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.Tracker.CQRS.Command;
public class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand, ICommitResult>
{
    private readonly StudentDbContext _dbContext;
    private readonly Guid? _userId;
    private readonly CurriculumClient _CurriculumClient;
    private readonly IMediator _mediator;
    private readonly JsonLocalizerManager _resourceJsonManager;

    public UpdateActivityCommandHandler(StudentDbContext dbContext,
                                        CurriculumClient curriculumClient,
                                        IWebHostEnvironment configuration,
                                        IHttpContextAccessor httpContextAccessor,
                                        IMediator mediator)
    {
        _dbContext = dbContext;
        _userId = httpContextAccessor.GetIdentityUserId();
        _mediator = mediator;
        _CurriculumClient = curriculumClient;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
    }

    public async Task<ICommitResult> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
    {
        // =========== update student Activity ================ Check Here
        ActivityTracker? studentActivityTracker = await _dbContext.Set<ActivityTracker>()
                                                                         .SingleOrDefaultAsync(a => a.Id.Equals(request.ActivityRequest.ActivityId), cancellationToken);
        if (studentActivityTracker == null)
        {
            return ResultType.NotFound.GetCommitResult("X0002", _resourceJsonManager["X0002"]);
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
                                       .Where(a => a.StudentIdentityId.Equals(_userId) && a.SubjectId.Equals(subjectId))
                                       .OrderByDescending(a => a.MedalLevel)
                                       .FirstOrDefaultAsync(cancellationToken);

        if (reward != null)
            LevelBeforeActivity = (int)reward.MedalLevel;

        // ===========Calculate Progress for subject After Activity================
        // =========== Get sumation of student point in subject ================
        ICommitResult<IdentitySubjectScoreResponse> subjectScore = await _mediator.Send(new GetIdentitySubjectScoreQuery(subjectId), cancellationToken);

        double progresslevel = 0;
        if (subjectScore.Value.StudentScore != 0 || subjectScore.Value.SubjectScore != 0)
        {
            progresslevel = (subjectScore.Value.StudentScore / subjectScore.Value.SubjectScore) * 100;
        }


        // =========== Get studen Level After this Activity ================
        // =========== Get student rewrad of subject ===================
        RewardDetails RewardDetails = GetMedalType(1, progresslevel);

        //====================Insert Student Reward after check medal level before and after update===============================
        if (RewardDetails.Id > LevelBeforeActivity)
        {
            // =========== Save rewards Changes================
            _dbContext.Set<Reward>().Add(new Reward
            {
                Type = 1,  //reward to specific subject
                MedalLevel = (MedalLevel)RewardDetails.Id,
                SubjectId = subjectDetails.Value.Id,
                StudentIdentityId = _userId.GetValueOrDefault(),
                IsNew = true,
                Title = RewardDetails.Title,
                Description = RewardDetails.Description + subjectDetails.Value.Name,
                Image = string.Concat(subjectDetails.Value.Id.AsSpan(0, 4), RewardDetails.Image)
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
            }
        }
    }

    // =========== Get student  rewrad of subject or all subject and return RewardDetails model  ================
    private static RewardDetails GetMedalType(int type, double progresslevel)
    {
        if (progresslevel >= 20 && progresslevel < 40)
            return new RewardDetails
            {
                Id = 1,
                Title = type == 1 ? "ميدالية برنزية" : "برنز",
                Description = type == 1 ? "مبروك لقد حصلت على الميدالية البرنزية في مادة " : "مبروك لقد وصلت إلى المنصة البرنزية ",
                Image = "Pronz.png"
            };
        else if (progresslevel >= 40 && progresslevel < 60)
            return new RewardDetails
            {
                Id = 2,
                Title = type == 1 ? "ميدالية فضية" : "فضة",
                Description = type == 1 ? "مبروك لقد حصلت على الميدالية الفضية في مادة " : "مبروك لقد وصلت إلى المنصة الفضية ",
                Image = "Silver.png"
            };
        else if (progresslevel >= 60 && progresslevel < 80)
            return new RewardDetails
            {
                Id = 3,
                Title = type == 1 ? "ميدالية ذهبية" : "ذهب",
                Description = type == 1 ? "مبروك لقد حصلت على الميدالية الذهبية في مادة " : "مبروك لقد وصلت إلى المنصة الذهبية ",
                Image = "Gold.png"
            };
        else if (progresslevel >= 80 && progresslevel < 95)
            return new RewardDetails
            {
                Id = 4,
                Title = type == 1 ? "ميدالية بلاتينية" : "بلاتين",
                Description = type == 1 ? "مبروك لقد حصلت على الميدالية البلاتينية في مادة " : "مبروك لقد وصلت إلى المنصة البلاتينية ",
                Image = "platin.png"
            };
        else
            return new RewardDetails
            {
                Id = 5,
                Title = type == 1 ? "الكأس" : "البطولة",
                Description = type == 1 ? "مبروك لقد حصلت على الكأس في مادة " : "مبروك لقد وصلت إلى منصة البطولة ",
                Image = "Cub.png"
            };
    }
}