using IdentityEntities.Entities.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IdentityEntities.Entities.Identities
{
    public class IdentitySchool : BaseEntity
    {
        public string Name { get; set; }
        public virtual ICollection<IdentityUser> IdentityUsers { get; set; }
    }
}
