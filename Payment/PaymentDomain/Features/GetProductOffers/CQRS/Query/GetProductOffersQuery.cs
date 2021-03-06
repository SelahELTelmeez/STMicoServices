using PaymentDomain.Features.GetProductOffers.DTO.Query;

namespace PaymentDomain.Features.GetProductOffers.CQRS.Query;

public record GetProductOffersQuery(int? Grade, string? Promocode) : IRequest<ICommitResult<ProductOfferResponse>>;


