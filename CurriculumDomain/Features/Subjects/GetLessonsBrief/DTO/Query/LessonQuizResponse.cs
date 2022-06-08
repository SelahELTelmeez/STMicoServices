namespace CurriculumDomain.Features.Subjects.GetLessonsBrief.DTO.Query
{
    public class LessonQuizResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int QuizClipId { get; set; }
        public int Points { get; set; }
    }
}
