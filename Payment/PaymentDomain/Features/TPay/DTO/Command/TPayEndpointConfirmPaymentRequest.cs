namespace PaymentDomain.Features.TPay.DTO.Command;

public class TPayEndpointConfirmPaymentRequest
{
    public string Signature { get; set; }
    public string PinCode { get; set; }
    public string TransactionId { get; set; }
}
