namespace CurriculumDomain.Features.GetCurriculumLesson.DTO.Query;
public class CurriculumLessonClipResponseDTO
{
    public List<int> Types { get; set; }
    public List<CurriculumClipResponseDTO> Clips { get; set; }   
}

public class CurriculumClipResponseDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int? Type { get; set; }
}