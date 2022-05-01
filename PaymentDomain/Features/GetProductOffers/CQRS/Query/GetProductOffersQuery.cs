using PaymentDomain.Features.GetProductOffers.DTO.Query;

namespace PaymentDomain.Features.GetProductOffers.CQRS.Query;

public record GetProductOffersQuery(ProductOfferRequest ProductOfferRequest) : IRequest<CommitResult<ProductOfferResponse>>;


