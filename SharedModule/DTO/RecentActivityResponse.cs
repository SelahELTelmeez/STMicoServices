namespace SharedModule.DTO
{
    public class RecentActivityResponse
    {
        public string SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string SubjectImage { get; set; }
        public string LessonName { get; set; }
        public string ClipName { get; set; }
        public DateTime ActivityTime { get; set; }
    }
}
