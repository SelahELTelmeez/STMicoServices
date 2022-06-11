using CurriculumDomain.Features.Quizzes.CQRS.Command;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.MCQS;
using CurriculumEntites.Entities.Quizzes;
using JsonLocalizer;
using Mapster;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using DomainEntities = CurriculumEntites.Entities.Quizzes;

namespace CurriculumInfrastructure.Features.Quizzes.Quiz.CQRS.Command;
public class CreateQuizCommandHandler : IRequestHandler<CreateQuizCommand, CommitResult<int>>
{
    private readonly CurriculumDbContext _dbContext;
    private readonly JsonLocalizerManager _resourceJsonManager;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CreateQuizCommandHandler(CurriculumDbContext dbContext,
                                         IWebHostEnvironment configuration,
                                         IHttpContextAccessor httpContextAccessor)
    {
        _dbContext = dbContext;
        _resourceJsonManager = new JsonLocalizerManager(configuration.WebRootPath, httpContextAccessor.GetAcceptLanguage());
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>
    /// Here We will create new Quiz with list of Quiz Form and List of User Quizs
    /// </summary>
    /// <param name="request"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<CommitResult<int>> Handle(CreateQuizCommand request, CancellationToken cancellationToken)
    {
        // Here we get the id of user to create quiz for this user. 
        Guid? IdentityUserId = _httpContextAccessor.GetIdentityUserId();


        Clip? clip = await _dbContext.Set<Clip>().Where(a => a.Id.Equals(request.ClipId))
                                                 .Include(a => a.LessonFK)
                                                 .ThenInclude(a => a.UnitFK)
                                                 .ThenInclude(a => a.SubjectFK)
                                                 .SingleOrDefaultAsync(cancellationToken);

        if (clip == null)
        {
            return new CommitResult<int>
            {
                ResultType = ResultType.NotFound,
                ErrorCode = "X0002",
                ErrorMessage = _resourceJsonManager["X0002"]
            };
        }

        // Here we set data of quiz then add this data to table of quiz then save changes
        DomainEntities.Quiz quiz = new DomainEntities.Quiz
        {
            Creator = IdentityUserId.Value,
            LessonId = clip?.LessonId,
            SubjectId = clip?.LessonFK?.UnitFK?.SubjectId,
            UnitId = clip?.LessonFK?.UnitId,
            QuizForms = await GetMCQAsync(clip, cancellationToken)
        };

        _dbContext.Set<DomainEntities.Quiz>().Add(quiz);

        await _dbContext.SaveChangesAsync(cancellationToken);

        // Here we'll return id of quiz
        return new CommitResult<int>
        {
            ResultType = ResultType.Ok,
            Value = quiz.Id
        };
    }

    private async Task<ICollection<QuizForm>> GetMCQAsync(Clip clip, CancellationToken cancellationToken)
    {
        try
        {
            if (clip.LessonFK.Type.GetValueOrDefault() == 2)
            {
                return await _dbContext.Set<MCQ>()
                                       .Include(a => a.LessonFK)
                                       .Where(a => a.LessonFK.UnitId.Equals(clip.LessonFK.UnitId))
                                       .Include(a => a.Answers)
                                       .Include(a => a.Question)
                                       .ProjectToType<QuizForm>()
                                       .ToListAsync(cancellationToken);
            }
            else if (clip.LessonFK.Type.GetValueOrDefault() == 3)
            {
                return await _dbContext.Set<MCQ>()
                                       .Include(a => a.Answers)
                                       .Include(a => a.Question)
                                       .Include(a => a.LessonFK)
                                       .ThenInclude(a => a.UnitFK)
                                       .Where(a => a.LessonFK.UnitFK.SubjectId.Equals(clip.LessonFK.UnitFK.SubjectId))
                                       .Take(clip.PageNo)
                                       .ProjectToType<QuizForm>()
                                       .ToListAsync(cancellationToken);
            }
            else
            {
                return await _dbContext.Set<MCQ>()
                                       .Where(a => a.LessonId.Equals(clip.LessonId))
                                       .Include(a => a.Answers)
                                       .Include(a => a.Question)
                                       .Take(clip.PageNo)
                                       .ProjectToType<QuizForm>()
                                       .ToListAsync(cancellationToken);
            }
        }
        catch (Exception ex)
        {
            throw ex;
        }
    }
}