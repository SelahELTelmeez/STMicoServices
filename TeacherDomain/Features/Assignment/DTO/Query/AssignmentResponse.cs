namespace TeacherDomain.Features.Assignment.DTO.Query;
public class AssignmentResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public DateTime CreatedOn { get; set; }
    public DateTime EndDate { get; set; }
    public int EntrolledCounter { get; set; }
}
