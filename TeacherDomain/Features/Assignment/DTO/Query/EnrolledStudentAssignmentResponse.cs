using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherDomain.Features.Classes.DTO.Query;

namespace TeacherDomain.Features.Assignment.DTO.Query
{
    public class EnrolledStudentAssignmentResponse : EnrolledStudentResponse
    {
        public string? ReplyComment { get; set; }
        public Guid? ReplyAttachmentId { get; set; }
    }
}
