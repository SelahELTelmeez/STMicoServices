using DashboardDomain.Features.DTO.Query;
using Flaminco.CommitResult;
using MediatR;

namespace DashboardDomain.Features.CQRS.Query;

public record GetAllGroupSectionQuery : IRequest<ICommitResults<SectionGroupResponse>>;


