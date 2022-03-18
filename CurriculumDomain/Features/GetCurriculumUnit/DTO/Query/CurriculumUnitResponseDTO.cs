namespace CurriculumDomain.Features.GetStudentCurriculumDetails.DTO.Query;
public class CurriculumUnitResponseDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public List<CurriculumLessonsResponseDTO> Lessons { get; set; }
}

public class CurriculumLessonsResponseDTO
{
    public int Id { get; set; }
    public string? Name { get; set; }
}