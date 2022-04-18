namespace TeacherDomain.Features.Quiz.DTO;
public class CreateQuizRequest
{
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int ClipId { get; set; }
    public List<int> Classes { get; set; }
}
