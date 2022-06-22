﻿using SharedModule.DTO;
using SharedModule.Extensions;
using StudentDomain.Features.Tracker.CQRS.Query;
using StudentDomain.Features.Tracker.DTO.Query;
using StudentEntities.Entities.Rewards;
using StudentEntities.Entities.Trackers;
using StudentInfrastructure.HttpClients;

namespace StudentInfrastructure.Features.Tracker.CQRS.Query
{
    public class GetStudentRewardQueryHandler : IRequestHandler<GetStudentRewardQuery, ICommitResult<StudentRewardResponse>>
    {
        private readonly StudentDbContext _dbContext;
        private readonly CurriculumClient _curriculumClient;
        private readonly Guid? _studentId;
        public GetStudentRewardQueryHandler(StudentDbContext dbContext, IHttpContextAccessor httpContextAccessor, CurriculumClient curriculumClient)
        {
            _dbContext = dbContext;
            _studentId = httpContextAccessor.GetIdentityUserId();
            _curriculumClient = curriculumClient;
        }
        public async Task<ICommitResult<StudentRewardResponse>> Handle(GetStudentRewardQuery request, CancellationToken cancellationToken)
        {
            ICommitResults<SubjectBriefResponse>? identitySubjects = await _curriculumClient.GetSubjectsBriefByTermAsync(request.Grade, request.TermId, cancellationToken);

            if (!identitySubjects.IsSuccess)
            {
                return identitySubjects.ResultType.GetValueCommitResult<StudentRewardResponse>(default, identitySubjects.ErrorCode, identitySubjects.ErrorMessage);
            }

            List<Reward> rewards = await _dbContext.Set<Reward>().Where(a => a.StudentIdentityId == _studentId).ToListAsync(cancellationToken);

            if (!rewards.Where(a => identitySubjects.Value.Select(a => a.Id).Contains(a.SubjectId)).Any())
            {
                return new CommitResult<StudentRewardResponse>
                {
                    ResultType = ResultType.Ok,
                    Value = new StudentRewardResponse
                    {
                        StudentPoints = 0,
                        TotalPoints = identitySubjects.Value.Sum(a => a.RewardPoints) ?? 0,
                        Subjects = SubjectMapper()
                    }
                };
            }

            IEnumerable<Subject> SubjectMapper()
            {
                foreach (var subject in identitySubjects.Value)
                {
                    yield return new Subject
                    {
                        SubjectName = subject.Name,
                        PrizesList = new List<PrizesList>()
                        {
                            new PrizesList{ Sort = 5 , Source = $"https://www.selaheltelmeez.com/media/lmsApp/rewards/{string.Concat(subject.Id.AsSpan(0, 4),"cub.png")}", IsActive = false  },
                            new PrizesList{ Sort = 4 , Source = $"https://www.selaheltelmeez.com/media/lmsApp/rewards/{string.Concat(subject.Id.AsSpan(0, 4),"pronz.png")}", IsActive = false },
                            new PrizesList{ Sort = 3 , Source = $"https://www.selaheltelmeez.com/media/lmsApp/rewards/{string.Concat(subject.Id.AsSpan(0, 4),"silver.png")}",  IsActive  = false },
                            new PrizesList{ Sort = 2 , Source = $"https://www.selaheltelmeez.com/media/lmsApp/rewards/{string.Concat(subject.Id.AsSpan(0, 4),"gold.png")}",   IsActive = false},
                            new PrizesList{ Sort = 1 , Source = $"https://www.selaheltelmeez.com/media/lmsApp/rewards/{string.Concat(subject.Id.AsSpan(0, 4),"platin.png")}",  IsActive = false },
                        }
                    };
                }
                yield break;
            }

            IEnumerable<Subject> RewardMapper()
            {
                foreach (var reward in rewards.Where(a => identitySubjects.Value.Select(a => a.Id).Contains(a.SubjectId)))
                {
                    SubjectBriefResponse? subjectBrief = identitySubjects.Value.SingleOrDefault(a => a.Id == reward.SubjectId);

                    yield return new Subject
                    {
                        SubjectName = subjectBrief.Name,
                        PrizesList = new List<PrizesList>()
                        {
                            new PrizesList{ Sort = 5 , Source = $"https://www.selaheltelmeez.com/media/lmsApp/rewards/{string.Concat(subjectBrief.Id.AsSpan(0, 4),"cub.png")}", IsActive = reward.MedalLevel == StudentEntities.Entities.Shared.MedalLevel.Cub  },
                            new PrizesList{ Sort = 4 , Source = $"https://www.selaheltelmeez.com/media/lmsApp/rewards/{string.Concat(subjectBrief.Id.AsSpan(0, 4),"pronz.png")}", IsActive = reward.MedalLevel == StudentEntities.Entities.Shared.MedalLevel.Bronze },
                            new PrizesList{ Sort = 3 , Source = $"https://www.selaheltelmeez.com/media/lmsApp/rewards/{string.Concat(subjectBrief.Id.AsSpan(0, 4),"silver.png")}",  IsActive = reward.MedalLevel == StudentEntities.Entities.Shared.MedalLevel.Silver },
                            new PrizesList{ Sort = 2 , Source = $"https://www.selaheltelmeez.com/media/lmsApp/rewards/{string.Concat(subjectBrief.Id.AsSpan(0, 4),"gold.png")}",   IsActive = reward.MedalLevel == StudentEntities.Entities.Shared.MedalLevel.Gold},
                            new PrizesList{ Sort = 1 , Source = $"https://www.selaheltelmeez.com/media/lmsApp/rewards/{string.Concat(subjectBrief.Id.AsSpan(0, 4),"platin.png")}",  IsActive = reward.MedalLevel == StudentEntities.Entities.Shared.MedalLevel.Platinum },
                        }
                    };
                }
                yield break;
            }

            IEnumerable<ActivityTracker> activityTrackers = await _dbContext.Set<ActivityTracker>().Where(a => a.StudentId == _studentId && a.IsActive == true).ToListAsync(cancellationToken);

            return new CommitResult<StudentRewardResponse>
            {
                ResultType = ResultType.Ok,
                Value = new StudentRewardResponse
                {
                    StudentPoints = activityTrackers.Where(a => identitySubjects.Value.Select(a => a.Id).Contains(a.SubjectId)).Sum(a => a.StudentPoints),
                    TotalPoints = identitySubjects.Value.Sum(a => a.RewardPoints) ?? 0,
                    Subjects = RewardMapper()
                }
            };
        }
    }
}
