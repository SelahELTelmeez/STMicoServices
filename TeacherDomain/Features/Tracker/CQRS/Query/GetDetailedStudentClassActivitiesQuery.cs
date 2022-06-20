using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TeacherDomain.Features.Tracker.DTO.Query;

namespace TeacherDomain.Features.Tracker.CQRS.Query;

public record GetDetailedStudentClassActivitiesQuery(int ClassId) : IRequest<ICommitResults<DetailedClassActivity>>;


