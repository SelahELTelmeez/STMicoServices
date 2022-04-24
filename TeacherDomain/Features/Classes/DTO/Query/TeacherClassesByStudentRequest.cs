using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TeacherDomain.Features.Classes.DTO.Query
{
    public class TeacherClassesByStudentRequest
    {
       public Guid StudenId { get; set; }
       public Guid TeacherId { get; set; }
    }
}
