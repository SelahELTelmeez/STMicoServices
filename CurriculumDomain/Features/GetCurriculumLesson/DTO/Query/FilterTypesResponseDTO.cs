namespace CurriculumDomain.Features.GetCurriculumLesson.DTO.Query;

public record FilterTypesResponseDTO
{
    public int? Value { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
}
