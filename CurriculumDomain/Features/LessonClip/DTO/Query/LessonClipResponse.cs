namespace CurriculumDomain.Features.LessonClip.DTO.Query;
public class LessonClipResponse
{
    public List<FilterTypesResponse> Types { get; set; }
    public List<ClipResponse> Clips { get; set; }
}

public class ClipResponse
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