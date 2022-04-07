namespace CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
public class ClipActivityResponse
{
    public int ClipId { get; set; }
    public int StudentScore { get; set; }
    public int? ActivityId { get; set; }
    public int? GameObjectCode { get; set; }
    public int? GameObjectProgress { get; set; }
    public int? GameObjectLearningDurationInSec { get; set; }
}