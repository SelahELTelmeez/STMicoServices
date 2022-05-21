namespace PaymentDomain.Features.TPay.DTO.Command
{
    public class TPayConfirmPaymentResponse
    {
        public int AmountCharged { get; set; }
        public string CurrencyCode { get; set; }
    }
}
