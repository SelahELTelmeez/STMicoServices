namespace SharedModule.DTO
{
    public class UnitSubjectBriefResponse
    {
        public int UnitId { get; set; }
        public string UnitName { get; set; }
        public IEnumerable<LessonSubjectBriefResponse> Lessons { get; set; }
    }

    public class LessonSubjectBriefResponse
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; }

        public IEnumerable<ClipSubjectBreifResponse> Clips { get; set; }
    }

    public class ClipSubjectBreifResponse
    {
        public int ClipId { get; set; }
        public string ClipName { get; set; }
    }
}
