namespace IdentityEntities.Entities.Grades;
public class Grade : BaseEntity
{
    public string Name { get; set; }
    public string ShortName { get; set; }
    public bool IsEnabled { get; set; }
}