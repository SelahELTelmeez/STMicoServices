namespace PaymentDomain.Features.TPay.DTO.Command
{
    public class TPayEndpointInitializerRequest
    {
        public string ProductName { get; set; }
        public decimal ProductPrice { get; set; }
        public string OperatorCode { get; set; }
        public string MSISDN { get; set; }
        public int Language { get; set; }
        public string OrderInfo { get; set; }
        public string Signature { get; set; }
    }
}
