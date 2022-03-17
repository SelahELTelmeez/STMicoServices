using CurriculumEntites.Entities.Curriculums;
using CurriculumEntites.Entities.Lessons;
using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Units;
public class Unit : BaseEntity
{
    public string? Title { get; set; }
    public int? Sort { get; set; }
    public string? FullyQualifiedName { get; set; }
    public string? ShortName { get; set; }
    public bool? IsShow { get; set; }
    public int? Type { get; set; }
    public int? UnitNumber { get; set; }
    public DateTime? ScheduleDate { get; set; }
    public string CurriculumId { get; set; }
    [ForeignKey(nameof(CurriculumId))] public Curriculum CurriculumFK { get; set; }
    public virtual ICollection<Lesson> Lessons { get; set; }
}