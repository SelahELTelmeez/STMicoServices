namespace PaymentDomain.Features.FawryInitializer.DTO.Command
{
    public class FawryInitializerRespons
    {
        public string MerchantCode { get; set; }
        public string MerchantRefNum { get; set; }
        public string CustomerMobile { get; set; }
        public string? CustomerEmail { get; set; }
        public string CustomerName { get; set; }
        public DateTime? PaymentExpiry { get; set; }
        public Guid? CustomerProfileId { get; set; }
        public int ItemId { get; set; }
        public string Price { get; set; }
        public string? ItemDescription { get; set; }
        public int Quantity { get; set; }
        public string Image { get; set; }
        public string PaymentMethod { get; set; }
        public string ReturnUrl { get; set; }
        public string Signature { get; set; }
        public string Language { get; set; }
    }
}
