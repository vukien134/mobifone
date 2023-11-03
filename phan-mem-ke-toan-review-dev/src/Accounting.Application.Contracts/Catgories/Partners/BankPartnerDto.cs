using Accounting.BaseDtos;

namespace Accounting.Catgories.Partners
{
    public class BankPartnerDto : TenantOrgDto
    {
        public string Ord0 { get; set; }
        public string PartnerId { get; set; }
        public string PartnerCode { get; set; }
        public string BankAccNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string Note { get; set; }
    }
}
