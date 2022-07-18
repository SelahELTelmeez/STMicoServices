namespace SharedModule.DTO;

public class LimitedProfileResponse
{
    public string FullName { get; set; }
    public string UserId { get; set; }
    public string GradeName { get; set; }
    public int GradeId { get; set; }
    public string AvatarImage { get; set; }
    public string NotificationToken { get; set; }
    public bool IsPremium { get; set; }
}
