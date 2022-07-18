using CurriculumEntites.Entities.Shared;

namespace CurriculumEntites.Entities.Subjects;


public class SubjectGroup : BaseEntity
{
    public string Name { get; set; }

    public virtual ICollection<Subject> Subjects { get; set; }
}
