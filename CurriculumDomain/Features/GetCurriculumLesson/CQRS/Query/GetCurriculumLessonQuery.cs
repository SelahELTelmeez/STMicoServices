using CurriculumDomain.Features.GetCurriculumLesson.DTO.Query;
using ResultHandler;

namespace CurriculumDomain.Features.GetCurriculumLesson.CQRS.Query;
public record GetCurriculumLessonQuery(int LessonId) : IRequest<CommitResult<List<CurriculumLessonClipResponseDTO>>>;