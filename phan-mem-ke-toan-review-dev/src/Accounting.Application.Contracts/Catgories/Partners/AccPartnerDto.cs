using Accounting.BaseDtos;
using System.Collections.Generic;

namespace Accounting.Catgories.Partners
{
    public class AccPartnerDto : TenantOrgDto
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string PartnerGroupId { get; set; }
        public int PartnerType { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string Email { get; set; }
        public string OtherContact { get; set; }
        public string ContactPerson { get; set; }
        public string Fax { get; set; }
        public string TaxCode { get; set; }
        //Đại diện
        public string Representative { get; set; }
        //Hạn mức nợ, nợ trần
        public decimal DebtCeiling { get; set; }
        //Hạn mức nợ, nợ trần ngoại tệ
        public decimal DebtCeilingCur { get; set; }
        public string Note { get; set; }
        public string InfoFilter { get; set; }
        //Link tra cứu hóa đơn của đối tượng nhà cung cấp
        public string InvoiceSearchLink { get; set; }  
        public string PartnerGroupCode { get; set; }
        public string PartnerGroupName { get; set; }
    }
}
