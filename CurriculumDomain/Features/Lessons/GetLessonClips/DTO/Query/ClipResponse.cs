namespace CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
public class ClipResponse
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string? ClipName { get; set; }
    public int? ClipType { get; set; }
    public string Thumbnail { get; set; }
    public int ClipScore { get; set; }
    public int StudentScore { get; set; }
    public int? ActivityId { get; set; }
    public int? GameObjectCode { get; set; }
    public int? GameObjectProgress { get; set; }
    public int? GameObjectLearningDurationInSec { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string GameObjectUrl { get; set; }
    public int Orientation { get; set; }
    public bool? IsPremiumOnly { get; set; }
}