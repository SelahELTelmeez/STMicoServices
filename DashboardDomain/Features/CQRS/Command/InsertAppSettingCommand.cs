using DashboardDomain.Features.DTO.Command;
using Flaminco.CommitResult;
using MediatR;

namespace DashboardDomain.Features.CQRS.Command;

public record InsertAppSettingCommand(InsertAppSettingRequest InsertAppSettingRequest) : IRequest<ICommitResult>;


