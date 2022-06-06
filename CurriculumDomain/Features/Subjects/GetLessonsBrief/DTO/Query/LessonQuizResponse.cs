namespace CurriculumDomain.Features.Subjects.GetLessonsBrief.DTO.Query
{
    public class LessonQuizResponse
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; }
        public int QuizClipId { get; set; }
        public int Points { get; set; }
    }
}
