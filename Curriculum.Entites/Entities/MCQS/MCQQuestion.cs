using CurriculumEntites.Entities.Shared;
using System.ComponentModel.DataAnnotations.Schema;

namespace CurriculumEntites.Entities.MCQS;
public class MCQQuestion : BaseEntity
{
    public FormType QuestionType { get; set; }
    public string Value { get; set; }
    public int MCQId { get; set; }
    [ForeignKey(nameof(MCQId))] public MCQ MCQFK { get; set; }
}

