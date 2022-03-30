namespace CurriculumDomain.Features.SubjectUnit.DTO.Query;
public class SubjectUnitResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<SubjectLessonsResponse> Lessons { get; set; }
}

public class SubjectLessonsResponse
{
    public int Id { get; set; }
    public string? Name { get; set; }
}