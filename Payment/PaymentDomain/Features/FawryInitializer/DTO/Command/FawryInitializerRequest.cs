namespace PaymentDomain.Features.FawryInitializer.DTO.Command;

public class FawryInitializerRequest
{
    public int ProductId { get; set; }
    public int? Grade { get; set; }
    public Guid UserId { get; set; }
    public string MobileNumber { get; set; }
    public string? EmailAddress { get; set; }
    public string CustomerName { get; set; }
}
