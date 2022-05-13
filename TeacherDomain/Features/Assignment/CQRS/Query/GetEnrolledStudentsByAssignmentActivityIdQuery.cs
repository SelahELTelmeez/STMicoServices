using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherDomain.Features.Assignment.DTO.Query;

namespace TeacherDomain.Features.Assignment.CQRS.Query;

public record GetEnrolledStudentsByAssignmentActivityIdQuery(int ClassId, int AssingmentId) : IRequest<CommitResults<EnrolledStudentAssignmentResponse>>;
    
