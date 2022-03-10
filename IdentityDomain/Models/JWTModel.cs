namespace IdentityDomain.Models;
public class JWTModel
{
    public Guid IdentityUserId { get; set; }
    public string Email { get; set; }
    public string Secret { get; set; }
    public string Scope { get; set; }
}
