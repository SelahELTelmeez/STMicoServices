namespace PaymentDomain.Features.TPay.DTO.Command
{
    public class TPayInitializerRequest
    {
        public int ProductId { get; set; }
        public string MobileNumber { get; set; }
        public string OperatorCode { get; set; }
        public string MSISDN { get => string.Format("{0}{1}", "2", MobileNumber); }
        public int Language { get; set; } = 2;
    }
}
