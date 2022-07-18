using SharedModule.DTO;

namespace CurriculumDomain.Features.Lessons.GetClipsBrief.CQRS.Query;
public record GetClipsBriefByLessonIdQuery(int LessonId) : IRequest<CommitResults<ClipBriefResponse>>;