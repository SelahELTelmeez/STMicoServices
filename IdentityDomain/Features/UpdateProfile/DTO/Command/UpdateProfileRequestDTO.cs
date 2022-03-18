﻿namespace IdentityDomain.Features.UpdateProfile.DTO.Command;
public class UpdateProfileRequestDTO
{
    public int? AvatarId { get; set; }
    public int? CountryId { get; set; }
    public int? GovernorateId { get; set; }
    public string? DateOfBirth { get; set; }
    public int? Gender { get; set; }
    public bool? IsEmailSubscribed { get; set; }
}
