namespace PaymentDomain.Features.TPay.DTO.Command;

public class TPayEndpointResendPinCodeResponse
{
    public int OperationStatusCode { get; set; }

    public string ErrorMessage { get; set; }

}
