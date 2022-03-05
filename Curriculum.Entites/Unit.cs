using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites
{
    public class Unit : BaseEntity
    {
        public string? Title { get; set; }
        public int Sort { get; set; }
        public string? FullyQualifiedName { get; set; }
        public string? ShortName { get; set; }
        public bool IsShow { get; set; }
        public int Type { get; set; }
        public int UnitNumber { get; set; }
        public DateOnly? ScheduleDate { get; set; }
        public string Curriculum { get; set; }
        [ForeignKey(nameof(Curriculum))] public Curriculum CurriculumFK { get; set; }
    }
}
