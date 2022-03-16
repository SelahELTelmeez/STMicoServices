﻿using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities
{
    public class IdentityActivation : BaseEntity
    {
        public DateTime CreatedOn { get; set; } = DateTime.UtcNow;
        public string Code { get; set; }
        public ActivationType ActivationType { get; set; }
        public bool IsVerified { get; set; }
        public Guid IdentityUserId { get; set; }
        public DateTime ExpiredOn { get => CreatedOn.AddMinutes(30); }
        public DateTime? RevokedOn { get; set; }
        public bool IsActive { get => DateTime.UtcNow < ExpiredOn && !IsVerified && !RevokedOn.HasValue; }
        [ForeignKey(nameof(IdentityUserId))] public IdentityUser IdentityUserFK { get; set; }
    }
}
