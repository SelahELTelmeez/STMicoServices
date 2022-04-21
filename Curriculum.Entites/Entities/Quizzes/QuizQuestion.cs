using CurriculumEntites.Entities.Shared;

namespace CurriculumEntites.Entities.Quizzes;

public class QuizQuestion : BaseEntity
{
    public FormType Type { get; set; }
    public string Value { get; set; }
}
