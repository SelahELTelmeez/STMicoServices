using CurriculumDomain.Features.Lessons.GetLessonsBrief.DTO.Query;

namespace CurriculumDomain.Features.Lessons.GetLessonsBrief.CQRS.Query;

public record GetLessonsBriefQuery(List<int> LessonIds) : IRequest<CommitResults<LessonBriefResponse>>;

