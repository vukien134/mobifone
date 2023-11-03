using Accounting.Permissions;
using Accounting.TenantEntities;
using System.Collections;
using System.Collections.Generic;

namespace Accounting.Categories.OrgUnits
{
    public class OrgUnit : TenantAuditedEntity<string>
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
        public ICollection<OrgUnitPermission> OrgUnitPermissions { get; set; }
    }
}
