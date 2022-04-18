﻿using MediatR;
using ResultHandler;
using TeacherDomain.Features.Assignment.DTO.Query;

namespace TeacherDomain.Features.Assignment.CQRS.Query;
public record class GetAssignmentQuery() : IRequest<CommitResults<AssignmentResponse>>;

