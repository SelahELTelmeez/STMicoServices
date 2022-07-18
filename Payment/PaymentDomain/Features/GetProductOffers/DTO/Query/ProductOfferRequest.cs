namespace PaymentDomain.Features.GetProductOffers.DTO.Query;

public class ProductOfferRequest
{
    public int? Grade { get; set; }
    public string? Promocode { get; set; }
}
