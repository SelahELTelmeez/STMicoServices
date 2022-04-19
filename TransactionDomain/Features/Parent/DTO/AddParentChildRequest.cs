﻿namespace TransactionDomain.Features.Parent.DTO;
    public class AddParentChildRequest
    {
    public Guid? ChildId { get; set; }
    public string FullName { get; set; }
    public int? GradeId { get; set; }
    public DateTime? DateOfBirth { get; set; }
    public int? Gender { get; set; }
    public int IdentityRoleId { get; set; }
}