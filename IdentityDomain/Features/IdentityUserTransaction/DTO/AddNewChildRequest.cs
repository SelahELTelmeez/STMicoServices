﻿using IdentityEntities.Entities.Shared;
namespace IdentityDomain.Features.IdentityUserTransaction.DTO
{
    public class AddNewChildRequest
    {
        public Guid? ChildId { get; set; }
        public string FullName { get; set; }
        public int? GradeId { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public Gender? Gender { get; set; }
        public int IdentityRoleId { get; set; }
    }
}