using CurriculumEntites.Entities.Shared;
using CurriculumEntites.Entities.Units;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Subjects;
public class Subject
{
    [Key]
    public string Id { get; set; }
    public string? Title { get; set; }
    public CurriculumStage Stage { get; set; }
    public int Grade { get; set; }
    public int Term { get; set; }
    public bool? IsAppShow { get; set; }
    public int? RewardPoints { get; set; }
    public string? TeacherGuide { get; set; }
    public string? FullyQualifiedName { get; set; }
    public string? ShortName { get; set; }
    public int? SubjectLanguageId { get; set; }
    public int? SubjectGroupId { get; set; }
    [ForeignKey(nameof(SubjectLanguageId))] public SubjectLanguage? SubjectLanguageFK { get; set; }
    [ForeignKey(nameof(SubjectGroupId))] public SubjectGroup? SubjectGroupFK { get; set; }
    public virtual ICollection<Unit> Units { get; set; }
}