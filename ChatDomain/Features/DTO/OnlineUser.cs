namespace ChatDomain.Features.DTO
{
    public class OnlineUser
    {
        public Guid UserId { get; set; }
        public string AvatarImage { get; set; }
        public string UserName { get; set; }
        public string ConnectionId { get; set; }
    }
}
