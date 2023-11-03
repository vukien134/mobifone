using Accounting.BaseDtos;

namespace Accounting.Catgories.OrgUnits
{
    public class OrgUnitDto : TenantAuditedEntityDto<string>
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public string NameE { get; set; }
        public string Address { get; set; }
        public string TaxCode { get; set; }
        //Mã cơ quan thuế
        public string TaxAuthorityCode { get; set; }
        public string TaxAuthorityName { get; set; }
        //Người ký
        public string Signee { get; set; }
        //Đơn vị nộp
        public string SubmittingOrganiztion { get; set; }
        public string SubmittingAddress { get; set; }
        //Xã, phường
        public string Wards { get; set; }
        public string District { get; set; }
        public string Province { get; set; }
        //Người lập
        public string Producer { get; set; }
        //Kế toán trưởng
        public string ChiefAccountant { get; set; }
        public string Director { get; set; }
        public string Description { get; set; }
        //Mã ngành nghề
        public string CareerId { get; set; }
        public int Year { get; set; }
    }
}
