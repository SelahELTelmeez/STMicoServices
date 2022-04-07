using CurriculumDomain.Features.Lessons.GetClipsBrief.DTO.Query;

namespace CurriculumDomain.Features.Lessons.GetClipsBrief.CQRS.Query;
public record GetClipsBriefByLessonIdQuery(int LessonId) : IRequest<CommitResults<ClipBriefResponse>>;