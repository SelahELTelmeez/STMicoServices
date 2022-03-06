using CurriculumEntites.Entities.Shared;
using Microsoft.EntityFrameworkCore;

namespace CurriculumEntites.Entities.Curriculums;

[Owned]
public class CurriculumLanguage : BaseEntity
{
    public string Name { get; set; }
    public virtual ICollection<Curriculum> Curriculums { get; set; }
}
