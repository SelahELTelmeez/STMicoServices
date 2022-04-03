namespace TransactionDomain.Features.IdentitySubjectScore.DTO
{
    public class IdentitySubjectScoreResponse
    {
        public int SubjectScore { get; set; }
        public float StudentScore { get; set; }
        public float Progress { get => (StudentScore / (float)SubjectScore); }
    }
}
