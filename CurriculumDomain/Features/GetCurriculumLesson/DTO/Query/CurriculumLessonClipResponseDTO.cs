namespace CurriculumDomain.Features.GetCurriculumLesson.DTO.Query;
public class CurriculumLessonClipResponseDTO
{
    public List<FilterTypesResponseDTO> Types { get; set; }
    public List<CurriculumClipResponseDTO> Clips { get; set; }
}

public class CurriculumClipResponseDTO
{
    public int Id { get; set; }
    public string? ClipName { get; set; }
    public int? ClipType { get; set; }
    public string Thumbnail { get; set; }
    public int ClipScore { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string GameObjectUrl { get; set; }
    public bool IsPremiumOnly { get; set; }
}