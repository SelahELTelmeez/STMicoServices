namespace SharedModule.DTO
{
    public class SubjectDetailedResponse : SubjectResponse
    {
        public DateTime? ActivityTime { get; set; }

        public IEnumerable<UnitSubjectBriefResponse> UnitResponses { get; set; }
    }
}
