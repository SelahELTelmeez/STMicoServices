using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites
{
    public class Curriculum
    {
        [Key]
        public string Id { get; set; }
        public string? Title { get; set; }
        public int Stage { get; set; } // ENUM
        public int Grade { get; set; }
        public int Term { get; set; }
        public int CurriculumLanguage { get; set; }
        public int CurriculumGroup { get; set; }
        public bool IsAppShow { get; set; }
        public int RewardPoints { get; set; }
        public string? TeacherGuide { get; set; }
        public string FullyQualifiedName { get; set; }
        public string ShortName { get; set; }
        [ForeignKey(nameof(CurriculumLanguage))] public CurriculumLanguage CurriculumLanguageFK { get; set; }
        [ForeignKey(nameof(CurriculumGroup))] public CurriculumGroup CurriculumGroupFK { get; set; }
    }

    public class CurriculumLanguage : BaseEntity
    {
        public string Name { get; set; }
    }

    public class CurriculumGroup : BaseEntity
    {
        public string Name { get; set; }
    }
}
