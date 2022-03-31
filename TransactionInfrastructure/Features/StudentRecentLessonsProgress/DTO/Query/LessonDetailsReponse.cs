namespace CurriculumDomain.Features.LessonDetails.DTO.Query;

public class LessonDetailsReponse
{
    public string? Name { get; set; }
    public string? Title { get; set; }
    public string? Sort { get; set; }
    public string? Type { get; set; }
    public string? ShortName { get; set; }
    public int? StartUnit { get; set; }
    public int? EndUnit { get; set; }
    public bool? IsShow { get; set; }
    public int? Ponits { get; set; }
    public DateTime? ScheduleDate { get; set; }
}
