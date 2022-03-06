using CurriculumEntites.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace CurriculumEntites.Entities.Curriculums;


public class CurriculumGroup : BaseEntity
{
    public string Name { get; set; }

    public virtual ICollection<Curriculum> Curriculums { get; set; }
}
