using CurriculumDomain.Features.Subjects.GetLessonsBrief.CQRS.Query;
using CurriculumDomain.Features.Subjects.GetLessonsBrief.DTO.Query;
using CurriculumEntites.Entities;
using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.Lessons;
using Microsoft.EntityFrameworkCore;
using DomainEntitiesUnits = CurriculumEntites.Entities.Units;

namespace CurriculumInfrastructure.Features.Subjects.GetLessonsBrief.CQRS.Query;
public class GetLessonsBriefBySubjectIdQueryHandler : IRequestHandler<GetLessonsBriefBySubjectIdQuery, CommitResults<LessonQuizResponse>>
{
    private readonly CurriculumDbContext _dbContext;

    public GetLessonsBriefBySubjectIdQueryHandler(CurriculumDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    // GetLessonsBriefBySubjectIdQuery
    public async Task<CommitResults<LessonQuizResponse>> Handle(GetLessonsBriefBySubjectIdQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Lesson> lessons = await _dbContext.Set<DomainEntitiesUnits.Unit>()
                                                      .Where(a => a.SubjectId.Equals(request.SubjectId))
                                                      .Include(a => a.Lessons)
                                                      .ThenInclude(a => a.Clips)
                                                      .SelectMany(a => a.Lessons)
                                                      .ToListAsync(cancellationToken);

        IEnumerable<LessonQuizResponse> Mapper()
        {
            foreach (Lesson lesson in lessons)
            {
                Clip? quizClip = lesson.Clips.SingleOrDefault(a => a.Type == CurriculumEntites.Entities.Shared.ClipType.Quiz);
                if (quizClip != null)
                {
                    yield return new LessonQuizResponse
                    {
                        LessonId = lesson.Id,
                        LessonName = lesson.Name,
                        QuizClipId = quizClip.Id
                    };
                }

            }
            yield break;
        }

        return new CommitResults<LessonQuizResponse+->
        {
            ResultType = ResultType.Ok,
            Value = Mapper()
        };
    }
}