using IdentityDomain.DTO.Identity.IdentityRegister.IdentityRegisterUser.Command;

namespace IdentityDomain.DTO.Identity.IdentityRegister.IdentityRegisterWithOffice.Command;
public class IdentityRegisterWithOfficeDTO : IdentityRegisterUserDTO
{
    public string OfficeId { get; set; }
}
