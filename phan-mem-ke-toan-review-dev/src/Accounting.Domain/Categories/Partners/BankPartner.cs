using Accounting.TenantEntities;

namespace Accounting.Categories.Partners
{
    public class BankPartner : TenantOrgEntity
    {
        public string Ord0 { get; set; }
        public string PartnerId { get; set; }
        public string PartnerCode { get; set; }
        public string BankAccNumber { get; set; }
        public string Name { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string Note { get; set; }
        public AccPartner AccPartner { get; set; }
    }
}
