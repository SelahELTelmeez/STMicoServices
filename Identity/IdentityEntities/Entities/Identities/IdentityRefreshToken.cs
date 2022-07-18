using JWTGenerator.JWTModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace IdentityEntities.Entities.Identities;

public class IdentityRefreshToken : RefreshToken
{
    public string IdentityUserId { get; set; }
    [ForeignKey(nameof(IdentityUserId))] public IdentityUser IdentityUserFK { get; set; }
}
