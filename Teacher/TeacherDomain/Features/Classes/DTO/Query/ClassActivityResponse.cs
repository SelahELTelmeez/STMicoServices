namespace TeacherDomain.Features.Classes.DTO.Query;
public class ClassActivityResponse
{
    public int Id { get; set; }
    public string Title { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int TypeValue { get; set; }
    public string TypeName { get; set; }
    public int EnrolledCounter { get; set; }
}