namespace PaymentDomain.Features.TPay.DTO.Command;

public class TPayConfirmPaymentRequest
{
    public string PinCode { get; set; }
    public int PurchaseContractId { get; set; }
}
