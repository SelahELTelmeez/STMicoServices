using CurriculumDomain.Features.GetStudentCurriculumDetails.DTO.Query;
using ResultHandler;

namespace CurriculumDomain.Features.GetCurriculumUnit.CQRS.Query;
public record GetCurriculumUnitQuery(string CurriculumId) : IRequest<CommitResult<List<GetCurriculumUnitResponseDTO>>>;