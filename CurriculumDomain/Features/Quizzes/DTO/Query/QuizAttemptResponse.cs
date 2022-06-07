namespace CurriculumDomain.Features.Quizzes.DTO.Query
{
    public class QuizAttemptResponse
    {
        public int Id { get; set; }
        public string? SubjectId { get; set; }
        public int? UnitId { get; set; }
        public int? LessonId { get; set; }
        public List<QuizFormAttemptResponse> QuizFormsAttempts { get; set; }
        public int Duration { get; set; }
    }

    public class QuizFormAttemptResponse
    {
        public int Id { get; set; }
        public int Type { get; set; }
        public string Value { get; set; }
        public string? Hint { get; set; }
        public List<QuizAnswerResponse> AnswerResponses { get; set; }
        public int? AttemptAnswerId { get; set; }
    }
}
