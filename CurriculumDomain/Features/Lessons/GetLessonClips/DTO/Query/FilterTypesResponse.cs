namespace CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
public record FilterTypesResponse
{
    public int? Value { get; set; }
    public string? Name { get; set; }
    public string? ImageUrl { get; set; }
}
