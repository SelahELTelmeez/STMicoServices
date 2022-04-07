namespace CurriculumDomain.Features.Lessons.GetLessonClips.DTO.Query;
public class LessonClipResponse
{
    public IEnumerable<FilterTypesResponse> Types { get; set; }
    public IEnumerable<ClipResponse> Clips { get; set; }
}
