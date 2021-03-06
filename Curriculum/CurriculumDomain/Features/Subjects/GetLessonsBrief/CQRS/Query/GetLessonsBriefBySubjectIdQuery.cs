using CurriculumDomain.Features.Subjects.GetLessonsBrief.DTO.Query;

namespace CurriculumDomain.Features.Subjects.GetLessonsBrief.CQRS.Query;
public record GetLessonsBriefBySubjectIdQuery(string SubjectId) : IRequest<CommitResults<LessonQuizResponse>>;