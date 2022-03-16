using CurriculumDomain.Features.GetStudentCurriculum.DTO.Query;
using ResultHandler;
namespace CurriculumDomain.Features.GetStudentCurriculum.CQRS.Query;
public record GetStudentCurriculumQuery() : IRequest<CommitResult<List<StudentCurriculumResponseDTO>>>;