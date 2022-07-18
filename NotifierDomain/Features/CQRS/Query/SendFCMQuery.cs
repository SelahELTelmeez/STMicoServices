namespace NotifierDomain.Features.CQRS.Query;

public record SendFCMQuery(string Token) : IRequest<ICommitResult>;


