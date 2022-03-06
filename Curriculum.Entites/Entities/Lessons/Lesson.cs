using CurriculumEntites.Entities.Clips;
using CurriculumEntites.Entities.Shared;
using CurriculumEntites.Entities.Units;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Lessons;
public class Lesson : BaseEntity
{
    public string? Name { get; set; }
    public string? Title { get; set; }
    public string? Sort { get; set; }
    public string? Type { get; set; }
    public string? ShortName { get; set; }
    public int? StartUnit { get; set; }
    public int? EndUnit { get; set; }
    public bool IsShow { get; set; }
    public int Ponit { get; set; }
    public DateTime? ScheduleDate { get; set; }
    public int UnitId { get; set; }
    [ForeignKey(nameof(UnitId))] public Unit UnitFK { get; set; }
    public virtual ICollection<Clip> Clips { get; set; }
}
