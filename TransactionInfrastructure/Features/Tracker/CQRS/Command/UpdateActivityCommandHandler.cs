using Mapster;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ResultHandler;
using System.Net.Http.Json;
//using CurriculumDomain.Features.LessonDetails.DTO;
using TransactionDomain.Features.IdentitySubjectScore.CQRS;
using TransactionDomain.Features.IdentitySubjectScore.DTO;
using TransactionDomain.Features.Tracker.CQRS.Command;
using TransactionDomain.Models;
using TransactionEntites.Entities;
using TransactionEntites.Entities.Rewards;
using TransactionEntites.Entities.Shared;
using TransactionEntites.Entities.Trackers;
using TransactionInfrastructure.Utilities;

namespace TransactionInfrastructure.Features.Tracker.CQRS.Command
{
    public class UpdateActivityCommandHandler : IRequestHandler<UpdateActivityCommand, CommitResult>
    {
        private readonly StudentTrackerDbContext _dbContext;
        private readonly Guid? _userId;
        private readonly HttpClient _CurriculumClient;
        private readonly IMediator _mediator;

        public UpdateActivityCommandHandler(StudentTrackerDbContext dbContext, IHttpClientFactory factory, IHttpContextAccessor httpContextAccessor, IMediator mediator)
        {
            _dbContext = dbContext;
            _userId = httpContextAccessor.GetIdentityUserId();
            _CurriculumClient = factory.CreateClient("CurriculumClient");
            _mediator = mediator;
        }
        public async Task<CommitResult> Handle(UpdateActivityCommand request, CancellationToken cancellationToken)
        {
            // =========== update student Activity ================
            EntityEntry<StudentActivityTracker> studentActivityTracker = _dbContext.Set<StudentActivityTracker>().Update(request.ActivityRequest.Adapt<StudentActivityTracker>());
            await _dbContext.SaveChangesAsync(cancellationToken);

            // =========== Get progress and set student rewards ================

            setStudentRewards(request, cancellationToken);

            // =========== Get Response ActivityId ================
            return new CommitResult
            {
                ResultType = ResultType.Ok, // : ResultType.PartialOk,
            };
        }

        // =========== Get progress of subject and set student rewards ================
        public async void setStudentRewards(UpdateActivityCommand request, CancellationToken cancellationToken)
        {
            // =========== get subject information Details================
            CommitResult<SubjectUnitResponse>? subjectDetails = await _CurriculumClient.GetFromJsonAsync<CommitResult<SubjectUnitResponse>>($"/Curriculum/GetSubjectUnitsQuery?SubjectId={request.ActivityRequest.SubjectId}");

            //======= get the heighest MedalLevel of student to this subject before this activity update ==================
            int LevelBeforeActivity = 0;

            List<MedalLevel> MedalLevel = await _dbContext.Set<Reward>()
                                           .Where(a => a.StudentIdentityId.Equals(_userId)
                                                       && a.SubjectId.Equals(subjectDetails.Value.Id))
                                           .OrderByDescending(a => a.MedalLevel)
                                           .Select(a => a.MedalLevel)
                                           .ToListAsync(cancellationToken);
            if (MedalLevel.Count() > 0)
                LevelBeforeActivity = (int)MedalLevel.FirstOrDefault();

            // ===========Calculate Progress for subject After Activity================
            // =========== Get sumation of student point in subject ================
            Task<CommitResult<IdentitySubjectScoreResponse>> subjectScore = _mediator.Send(new GetIdentitySubjectScoreQuery(request.ActivityRequest.SubjectId.ToString()), cancellationToken);

            float progresslevel = subjectScore.Result.Value.Progress * 100;
            // =========== Getstudent  Level After this Activity ================

            // =========== Getstudent  rewrad of subject ================
            RewardDetails RewardDetails = getMedalType(1, progresslevel);

            //====================Insert Student Reward after check medal level before and after update===============================
            if (RewardDetails.Id > LevelBeforeActivity)
            {
                Reward reward = new Reward();
                reward.Type = 1;  //reward to specific subject
                reward.MedalLevel = (MedalLevel)RewardDetails.Id;
                reward.SubjectId = subjectDetails.Value.Id.ToString();
                reward.StudentIdentityId = (Guid)_userId;
                reward.IsNew = true;
                reward.Title = RewardDetails.Title;
                reward.Description = RewardDetails.Description + subjectDetails.Value.Name;
                reward.Image = subjectDetails.Value.Id.ToString().Substring(0, 4) + RewardDetails.Image;

                // =========== Save rewards Changes================
                _dbContext.Set<Reward>().Add(reward);
                await _dbContext.SaveChangesAsync(cancellationToken);

                //====================After Update Activity check How many medals this Student have===============================
                int MedalNo = await _dbContext.Set<Reward>()
                                          .Where(a => a.StudentIdentityId.Equals(_userId)
                                                   && a.MedalLevel.Equals(reward.MedalLevel))
                                          .CountAsync(cancellationToken);


                if (MedalNo == 3)
                {
                    // =========== Getstudent  rewrad of subject ================
                    RewardDetails = getMedalType(2, progresslevel);

                    //====================Insert Student reward of all subject =============================
                    reward = new Reward();
                    reward.Type = 2; //reward to all subjects for this student
                    reward.MedalLevel = (MedalLevel)RewardDetails.Id;
                    reward.StudentIdentityId = (Guid)_userId;
                    reward.IsNew = true;
                    reward.Title = RewardDetails.Title;
                    reward.Description = RewardDetails.Description;
                    reward.Image = RewardDetails.Image;

                    // =========== Save rewards Changes================
                    _dbContext.Set<Reward>().Add(reward);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
        }

        // =========== Get student  rewrad of subject or all subject and return RewardDetails model  ================
        public RewardDetails getMedalType(int type, float progresslevel)
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
}