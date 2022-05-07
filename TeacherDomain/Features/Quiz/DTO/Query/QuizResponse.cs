namespace TeacherDomain.Features.Quiz.DTO.Query;

public class QuizResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime EndDate { get; set; }
    public int EntrolledCounter { get; set; }

}
