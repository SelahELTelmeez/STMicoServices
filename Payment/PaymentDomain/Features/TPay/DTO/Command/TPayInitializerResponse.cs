namespace PaymentDomain.Features.TPay.DTO.Command;

public class TPayInitializerResponse
{
    public int OperationStatusCode { get; set; }
    public string ErrorMessage { get; set; }
    public string TransactionId { get; set; }
    public string Msisdn { get; set; }
    public string VerificationMethod { get; set; }
}
