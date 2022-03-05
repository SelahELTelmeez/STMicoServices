using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.Curriculums;
public class Curriculum
{
    [Key]
    public string Id { get; set; }
    public string? Title { get; set; }
    public Stage Stage { get; set; } 
    public int Grade { get; set; }
    public int Term { get; set; }    
    public bool IsAppShow { get; set; }
    public int RewardPoints { get; set; }
    public string? TeacherGuide { get; set; }
    public string? FullyQualifiedName { get; set; }
    public string? ShortName { get; set; }
    public int? CurriculumLanguageId { get; set; } [ForeignKey(nameof(CurriculumLanguageId))] public CurriculumLanguage CurriculumLanguageFK { get; set; }
    public int? CurriculumGroupId { get; set; } [ForeignKey(nameof(CurriculumGroupId))] public CurriculumGroup CurriculumGroupFK { get; set; }
}