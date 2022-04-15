﻿namespace IdentityDomain.Features.Shared.DTO
{
    public class LimitedProfileResponse
    {
        public Guid UserId { get; set; }
        public string FullName { get; set; }
        public string GradeName { get; set; }
        public string AvatarImage { get; set; }
        public string NotificationToken { get; set; }
    }
}
