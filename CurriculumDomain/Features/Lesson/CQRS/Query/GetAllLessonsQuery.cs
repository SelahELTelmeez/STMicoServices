using CurriculumDomain.Features.Lesson.DTO.Query;

namespace CurriculumDomain.Features.Lesson.CQRS.Query;
public record GetAllLessonsQuery(string SubjectId) : IRequest<List<LessonResponse>>;