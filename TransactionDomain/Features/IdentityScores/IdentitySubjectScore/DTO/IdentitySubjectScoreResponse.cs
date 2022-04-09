namespace TransactionDomain.Features.IdentityScores.IdentitySubjectScore.DTO
{
    public class IdentitySubjectScoreResponse
    {
        public int SubjectScore { get; set; }
        public double StudentScore { get; set; }
        public double Progress { get => (StudentScore / SubjectScore); }
    }
}
