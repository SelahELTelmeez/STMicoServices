namespace CurriculumDomain.Features.Quizzes.DTO.Query;
public class QuizClipResponse
{
    public int Id { get; set; }
    public int LessonId { get; set; }
    public string? ClipName { get; set; }
    public int? ClipType { get; set; }
    public string Thumbnail { get; set; }
    public int ClipScore { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string GameObjectUrl { get; set; }
    public int Orientation { get; set; }
    public bool? IsPremiumOnly { get; set; }
}