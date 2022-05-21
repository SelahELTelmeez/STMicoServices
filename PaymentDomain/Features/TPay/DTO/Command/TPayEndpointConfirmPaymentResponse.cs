namespace PaymentDomain.Features.TPay.DTO.Command;

public class TPayEndpointConfirmPaymentResponse
{
    public int OperationStatusCode { get; set; }
    public int AmountCharged { get; set; }
    public string CurrencyCode { get; set; }
    public string ErrorMessage { get; set; }
}
