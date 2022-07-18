namespace PaymentDomain.Features.GetProductOffers.DTO.Query;

public class ProductOfferResponse
{
    public bool IsTPayEnabled { get; set; }
    public bool IsFawryEnabled { get; set; }
    public IEnumerable<MobileOperatorResponse> MobileOperators { get; set; }
    public IEnumerable<ProductResponse> ProductResponses { get; set; }
}

public class MobileOperatorResponse
{
    public string Name { get; set; }
    public string MCC { get; set; }
    public string MNC { get; set; }
}

public class ProductResponse
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string DescriptionAR { get; set; }
    public string DescriptionEN { get; set; }
    public decimal Price { get; set; }
}
