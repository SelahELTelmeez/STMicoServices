﻿using SharedModule.DTO;

namespace StudentDomain.Features.Reports.CQRS.Query;

public record RecentActivityQuery(int Term, Guid? StudentId) : IRequest<CommitResults<RecentActivityResponse>>;
